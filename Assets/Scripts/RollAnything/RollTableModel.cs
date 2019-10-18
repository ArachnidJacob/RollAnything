using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using RollAnything;
using UnityEngine;

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
        int totWeight = _totalWeight;
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


    RollEntry Roll(List<RollEntry> rollContext) // where T : Entry
    {
//            Debug.Log(drollObjects.Count + " objects to roll against");
        if (rollContext.Count < 1) return null;
        if (rollContext.Count == 1)
        {
            RollEntry firstObject = rollContext.First();
            // Debug.Log(firstObject + "is the entry",firstObject);
            return firstObject;
        }

        int allWeight = _totalWeight; //_totalWeight(rollContext);

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
                if (currentObject.m_GuaranteeDrop != 0)
                    ResetGuarantee(rollContext);
                else
                    IncreaseGuarantee(rollContext);

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
    /// Increases the odds for all guaranteed drops in list to drop
    /// </summary>
    /// <param name="drollObjects"></param>
    void IncreaseGuarantee(List<RollEntry> drollObjects)
    {
        for (int i = 0; i < drollObjects.Count; i++)
        {
            RollEntry te = drollObjects[i];

            if (te == null) continue;
            if (te.m_GuaranteeDrop != 0)
            {
                int addWeight = (_totalWeight) / te.m_GuaranteeDrop;
                //Debug.Log("Adding " + addWeight);
                te.m_GuaranteeBonus += addWeight;
            }
            else
                te.m_GuaranteeBonus = 0;
        }
    }

    /// <summary>
    /// Resets the guarantees of the drops, used when a guaranteed drop has dropped
    /// </summary>
    /// <param name="drollObjects"></param>
    void ResetGuarantee(List<RollEntry> drollObjects)
    {
        for (int i = 0; i < drollObjects.Count; i++)
        {
            RollEntry te = drollObjects[i];
            te.m_GuaranteeBonus = 0;
        }
    }


    protected int _totalWeight
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
}