using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using RollAnything;
using UnityEngine;
using UnityEngine.Animations;

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
        int totWeight = TotalWeight;
//        Debug.Log("Calculating Drop Chance " + totWeight);

        foreach (RollEntry re in m_Data)
        {
            re.SetLocalDropChance(totWeight);
        }

        //TreeElementUtility.TreeToList(root, m_Data);
    }


    public int IndexOfItem(RollEntry re)
    {
        return m_Data.IndexOf(re);
    }


    public RollEntry Roll(List<RollEntry> rollContext = null) // where T : Entry
    {
//            Debug.Log(drollObjects.Count + " objects to roll against");
        if (rollContext == null)
            rollContext = m_Data.ToList();
        if (rollContext.Count < 1) return null;
        if (rollContext.Count == 1)
        {
            RollEntry firstObject = rollContext.First();
            // Debug.Log(firstObject + "is the entry",firstObject);
            return firstObject;
        }

        int allWeight = TotalWeight; //_totalWeight(rollContext);

        _lastRoll = Random.Range(1, allWeight + 1);
        _currentWeight = 0;
        for (int i = 0; i < rollContext.Count; i++)
        {
//            if (rollContext[i] == null || rollContext[i].m_ObjectToRoll == null) // if its an empty entry, ignore it
//                continue;

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


    public int TotalWeight
    {
        get
        {
            int weightTotal = 0;
            foreach (RollEntry re in m_Data)
            {
                weightTotal += re.TotalWeight;
            }
            return weightTotal;
        }
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
    /// For Adding Unity Objects to the tree directly
    /// </summary>
    /// <param name="objects"></param>
    /// <param name="parent"></param>
    /// <param name="index"></param>
    public void AddObjectsToTree(Object[] objects, RollEntry parent = null, int index = 0)
    {
        List<RollEntry> rollEntries = new List<RollEntry>();

        if (parent == null)
            parent = root;

        foreach (Object objectToRoll in objects)
        {
            rollEntries.Add(NewEntry(objectToRoll, objectToRoll.name, parent));
        }
        AddElements(rollEntries, parent, index);
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