using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using RollAnything;
using UnityEngine;
using UnityEngine.Animations;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[System.Serializable]
public class RollTableModel : TreeModel<RollEntry>
{
    private int _lastRoll;
    private int _currentWeight;
    public RollEntry LastRolled;

    public RollTableModel(IList<RollEntry> rollEntries)
    {
        SetData(rollEntries);
        CalculateDropChance();
        modelChanged += CalculateDropChance;
    }


    public RollEntry TestRoll()
    {
        return LastRolled = Roll((List<RollEntry>) m_Data);
    }


    public void CalculateDropChance()
    {
        CalculateDropChance((List<RollEntry>) m_Data);
    }

    /// <summary>
    /// Takes the list of rollentries and calculates the drop chance of each item in the list in relation to each other item.
    /// </summary>
    /// <param name="entries"></param>
    public void CalculateDropChance(List<RollEntry> entries)
    {
        int totWeight = TotalWeight(entries);


        foreach (RollEntry re in entries)
        {
            re.SetLocalDropChance(totWeight);
        }

        //TreeElementUtility.TreeToList(root, m_Data);
    }

    public void UpdateModel()
    {
        Changed();
    }


    public int IndexOfItem(RollEntry re)
    {
        return m_Data.IndexOf(re);
    }


    /// <summary>
    /// Base Rollentry roll, will filter against the input Type
    /// </summary>
    /// <param name="rollContext"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public RollEntry Roll(List<RollEntry> rollContext = null, Type type = null) // where T : Entry
    {
//            Debug.Log(drollObjects.Count + " objects to roll against");
        if (rollContext == null)
            rollContext = m_Data.ToList();

        //Filter by types
        if (type != null)
        {
            List<RollEntry> filteredRollContext = new List<RollEntry>();

            foreach (RollEntry re in rollContext)
            {
                if (re.HasType(type))
                    filteredRollContext.Add(re);
            }

            rollContext = filteredRollContext;
        }


        if (rollContext.Count < 1) return null;
        if (rollContext.Count == 1)
        {
            RollEntry firstObject = rollContext.First();
            // Debug.Log(firstObject + "is the entry",firstObject);
            return firstObject;
        }


        //Weighted Rolling algorithm.
        int allWeight = TotalWeight(rollContext); //_totalWeight(rollContext);

        _lastRoll = Random.Range(1, allWeight + 1);
        _currentWeight = 0;
        for (int i = 0; i < rollContext.Count; i++)
        {
            RollEntry currentObject = rollContext[i];

            //Go through list, adding weights together, once we've surpassed the roll value, it gives us the weighted RollEntry
            _currentWeight += currentObject.TotalWeight;
            if (_lastRoll <= _currentWeight)
            {
//                if (currentObject.m_GuaranteeDrop != 0)
//                    ResetGuarantee(rollContext);
//                else
//                    IncreaseGuarantee(rollContext);

                currentObject.m_DropTimes++;
                //currentObject.totalResourcesSpent += currentObject.Value();
                return currentObject;
            }
        }

        Debug.LogError("Roll Returned nothing, something went wrong (Roll:" + _lastRoll + ", CurrentWeight:" +
                       _currentWeight + " / " + allWeight);
        return null;
    }

    /// <summary>
    /// Rolls for a type of object contained in the rollentries in the table
    /// </summary>
    /// <param name="rollContext"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Roll<T>(List<RollEntry> rollContext = null) where T : Object
    {
        RollEntry rollEntry = Roll(rollContext, typeof(T));
        if (rollEntry == null)
            return null;
        Object rolledObject = rollEntry.MyObject;
        Type storedType = rolledObject.GetType();

        //TODO optimise for the fact that we recieve a legal thing from the RollEntry Roll
        if (typeof(T) == typeof(Component))
        {
            if (storedType == typeof(GameObject))
            {
                GameObject rolledGameObject = (GameObject) rolledObject;
                var rolledGameObjectComponent = rolledGameObject.GetComponent<T>();
                if (rolledGameObjectComponent != null)
                {
                    return rolledGameObjectComponent;
                }
                Debug.LogError(
                    rolledGameObject.name + " does not have the component: " + typeof(T).ToString() + " attached.",
                    rolledGameObject);
                return null;
            }
            Debug.LogError(
                rolledObject.name + " is not a gameObject, and cannot have  " + typeof(T).ToString() + " attached.",
                rolledObject);
            return null;
        }


        return (T) rolledObject;
    }


    public int TotalWeight(List<RollEntry> entries = null)
    {
        if (entries == null)
            entries = (List<RollEntry>) m_Data;

        int weightTotal = 0;
        foreach (RollEntry re in entries)
        {
            weightTotal += re.TotalWeight;
        }
        return weightTotal;
    }


    /// <summary>
    /// new Entry Constructor to Add to the TreeModel properly
    /// </summary>
    /// <param name="objectToRoll"></param>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    RollEntry NewEntry(Object objectToRoll, string name, RollEntry parent)
    {
        if (parent == null)
            parent = root;

        if (string.IsNullOrEmpty(name) && objectToRoll != null)
            name = objectToRoll.name;

        int weight = 1;
        if (objectToRoll is IRollWeighted)
        {
            IRollWeighted rollWeightedCast = (IRollWeighted) objectToRoll;
            weight = rollWeightedCast.GetRollWeight();
        }

        return new RollEntry(objectToRoll, name, parent.depth, GenerateUniqueID(), weight);
    }


    /// <summary>
    /// For adding a Unity Object to the tree directly
    /// </summary>
    /// <param name="objectToRoll"></param>
    /// <param name="parent"></param>
    /// <param name="index"></param>
    public void AddObjectToTree(Object objectToRoll, RollEntry parent = null, int index = 0, string overrideName = "")
    {
        var newEntry = NewEntry(objectToRoll, overrideName, parent);

        AddElement(newEntry, parent,
            index == -1 ? 0 : index);
    }

    /// <summary>
    /// For Adding Unity Objects to the tree directly
    /// </summary>
    /// <param name="objects"></param>
    /// <param name="parent"></param>
    /// <param name="index"></param>
    public void AddObjectsToTree(Object[] objects, RollEntry parent = null, int index = 0)
    {
        if (parent == null)
            parent = root;

        var rollEntries = objects.Select(objectToRoll => NewEntry(objectToRoll, objectToRoll.name, parent)).ToList();
        AddElements(rollEntries, parent, index);
    }


    public void RemoveObject(Object objectToRemove)
    {
        var toRemove = m_Data.Where(re => re.HasObject(objectToRemove)).ToList();

        RemoveElements(toRemove);
    }


    public void RemoveObjects(Object[] objectToRemove)
    {
        var toRemove = m_Data.Where(re => re.HasObject(objectToRemove)).ToList();

        RemoveElements(toRemove);
    }


    //    /// <summary>
//    /// Increases the odds for all guaranteed drops in list to drop
//    /// </summary>
//    /// <param name="drollObjects"></param>
//    void IncreaseGuarantee(List<RollEntry> drollObjects)
//    {
//        for (int i = 0; i < drollObjects.Count; i++)
//        {
//            RollEntry te = drollObjects[i];
//
//            if (te == null) continue;
//            if (te.m_GuaranteeDrop != 0)
//            {
//                int addWeight = (_totalWeight) / te.m_GuaranteeDrop;
//                //Debug.Log("Adding " + addWeight);
//                te.m_GuaranteeBonus += addWeight;
//            }
//            else
//                te.m_GuaranteeBonus = 0;
//        }
//    }
//
//    /// <summary>
//    /// Resets the guarantees of the drops, used when a guaranteed drop has dropped
//    /// </summary>
//    /// <param name="drollObjects"></param>
//    void ResetGuarantee(List<RollEntry> drollObjects)
//    {
//        for (int i = 0; i < drollObjects.Count; i++)
//        {
//            RollEntry te = drollObjects[i];
//            te.m_GuaranteeBonus = 0;
//        }
//    }
}