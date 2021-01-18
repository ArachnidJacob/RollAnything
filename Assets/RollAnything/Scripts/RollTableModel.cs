using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using RollAnything;
using RollAnything.TreeBaseClasses;
using UnityEditor.UI;
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


    private List<RollEntry> SafeRollEntries(List<RollEntry> entriesToRoll = null)
    {
        if (m_Data == null)
            return null;

        if (entriesToRoll == null)
            entriesToRoll = m_Data.ToList();
        List<RollEntry> safeList = new List<RollEntry>();
        foreach (RollEntry re in entriesToRoll)
        {
            if (re == null)
                continue;
            if (re == root)
                continue;

            if (re.myObject == null)
                continue;

            safeList.Add(re);
        }

        return safeList;
        //return m_Data.ToList().Where(re =>( re != root&&re!=null)).ToList();
    }


    public RollTableModel(IList<RollEntry> rollEntries)
    {
        SetData(rollEntries);
        CalculateDropChance();
        modelChanged += CalculateDropChance;
    }


    public RollEntry TestRoll()
    {
        return LastRolled = TableRoll(SafeRollEntries());
    }


    /// <summary>
    /// Overload to calculate the drop chance of every entry in the local list
    /// </summary>
    public void CalculateDropChance()
    {
        CalculateDropChance(SafeRollEntries());
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
    }

    /// <summary>
    /// Public call to re-process the entries
    /// </summary>
    public void UpdateModel()
    {
        Changed();
    }

    /// <summary>
    /// Get the index of the RollEntry in the table
    /// </summary>
    /// <param name="re"></param>
    /// <returns></returns>
    public int IndexOfItem(RollEntry re)
    {
        return m_Data.IndexOf(re);
    }


    /// <summary>
    /// Gets the Total Weight in the input entry list
    /// </summary>
    /// <param name="entries"></param>
    /// <returns></returns>
    public int TotalWeight(List<RollEntry> entries = null)
    {
        entries = SafeRollEntries(entries);

        int weightTotal = 0;
        foreach (RollEntry re in entries)
        {
            weightTotal += re.TotalWeight;
        }

        return weightTotal;
    }


    /// <summary>
    /// For adding a Unity Object to the tree directly
    /// </summary>
    /// <param name="objectToRoll"></param>
    /// <param name="parent"></param>
    /// <param name="index"></param>
    public void AddObjectToTree(Object objectToRoll, RollEntry parent = null, int index = 0, string overrideName = "")
    {
        if (parent == null)
            parent = root;
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
        if (objectToRemove == null)
            return;
        var toRemove = SafeRollEntries().Where(re => re.HasObject(objectToRemove)).ToList();

        RemoveElements(toRemove);
    }


    public void RemoveObjects(Object[] objectToRemove)
    {
        var toRemove = SafeRollEntries().Where(re => re.HasObject(objectToRemove)).ToList();

        RemoveElements(toRemove);
    }


    /// <summary>
    /// Base Rollentry roll, will filter against the input Type
    /// </summary>
    /// <param name="rollContext">the list of entries to roll against</param>
    /// <param name="type">Filters the list by Type, affecting the roll chances</param>
    /// <param name="rollFilter"></param>
    /// <param name="rollResult">Record of the rolling, how many RollTables did we pass by, entries seleted, etc</param>
    /// <param name="filteredEntry"></param>
    /// <returns>The Rollled entry</returns>
    public RollEntry TableRoll(List<RollEntry> rollContext, Type type = null,
        Func<RollEntry, bool> filteredEntry = null)
    {
        if (rollContext == null)
        {
            Debug.LogError("RollContext Cannot be null in the model.");
            return null;
        }

        List<RollEntry> safeRollContext = SafeRollEntries(rollContext); //Cleans the Root and other nulls from the list
        if (safeRollContext == null)
            return null;

        //Filter by types
        if (type != null || filteredEntry != null)
        {
            List<RollEntry> filteredRollContext = new List<RollEntry>();

            foreach (RollEntry re in safeRollContext)
            {
                if (filteredEntry != null)
                    if (filteredEntry(re))
                    {
                        filteredRollContext.Add(re);
                        continue;
                    }

                if (type == null) continue;
                if (re.HasType(type))
                    filteredRollContext.Add(re);
            }

            if (filteredRollContext.Count < 1)
            {
                Debug.LogError("Type filter returned nothing for : " + type.ToString());
            }
            else
            {
                safeRollContext = filteredRollContext;
            }
        }


        if (safeRollContext.Count < 1) return null;
        if (safeRollContext.Count == 1)
        {
            return safeRollContext.First();
        }


        //Weighted Rolling algorithm.
        int allWeight = TotalWeight(safeRollContext); //_totalWeight(rollContext);

        _lastRoll = Random.Range(1, allWeight + 1);
        _currentWeight = 0;

        for (int i = 0; i < safeRollContext.Count; i++)
        {
            RollEntry currentEntry = safeRollContext[i];

            //Go through list, adding weights together, once we've surpassed the roll value, it gives us the weighted RollEntry
            _currentWeight += currentEntry.TotalWeight;
            if (_lastRoll > _currentWeight) continue;
            //TODO PseuRandom
//                if (currentObject.m_GuaranteeDrop != 0)
//                    ResetGuarantee(rollContext);
//                else
//                    IncreaseGuarantee(rollContext);

            //Check to see if this Entry is a table


            currentEntry.mDropTimes++;

            return currentEntry;
        }

        Debug.LogError("TableRoll Returned nothing, something went wrong (TableRoll:" + _lastRoll + ", CurrentWeight:" +
                       _currentWeight + " / " + allWeight);
        return null;
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
}