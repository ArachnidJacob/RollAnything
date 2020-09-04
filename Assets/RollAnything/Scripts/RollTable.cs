using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Tilemaps;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RollAnything
{
    /// <summary>
    /// Interface for putting on monobehaviours with RollTables to allow them to roll their own table in response to being rolled
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReRollable
    {
        RollTable NestedTable { get; }
    }

    /// <summary>
    /// Contains the aggregate results of a roll, including the path of the nested roll
    /// </summary>
    [Serializable]
    public class RollResult
    {
        public List<RollEntry> rolledEntries;
        public List<RollTable> rollTables;

        /// <summary>
        /// Empty TableRoll Result allows for the first table to be re-rolled once
        /// </summary>
        public RollResult()
        {
        }

        public RollResult(RollTable initialTable)
        {
            rollTables = new List<RollTable>() {initialTable};
        }

        public bool AddTable(RollTable table)
        {
            if (rollTables.Contains(table))
            {
                Debug.LogError("Can't Add more than one of the same table to a result)");
                return false;
            }

            rollTables.Add(table);
            return true;
        }

        public void AddEntry(RollEntry newEntry)
        {
            if (newEntry == null)
            {
                Debug.LogError("Tried to Add null to rollResult");
                return;
            }

            if (rolledEntries == null)
                rolledEntries = new List<RollEntry>();

            rolledEntries.Add(newEntry);
        }

        public List<T> GetResultObjectsOfType<T>() where T : Object
        {
            List<T> resultObjects = new List<T>();

            foreach (RollEntry re in rolledEntries)
            {
                if (re.HasType(typeof(T)))
                    resultObjects.Add(re.GetContainedClass<T>());
            }

            return resultObjects;
        }

        public RollEntry FinalEntry()
        {
            if (rolledEntries == null || rolledEntries.Count < 0)
                return null;
            return rolledEntries.Last();
        }
    }

    [Serializable]
    public class RollTable
    {
        [SerializeField] private List<RollEntry> _rollEntries = new List<RollEntry>();

        public bool expandedtable = false;
        private int _lastRoll;
        private int _currentWeight;

        List<RollEntry> RollEntries
        {
            get
            {
                if (_rollEntries.Count <= 0)
                {
                    _rollEntries = new List<RollEntry>();
                    _rollEntries.Add(new RollEntry(null, "Root", -1, 0, 0));
                }

                return _rollEntries;
            }
            set { _rollEntries = value; }
        }


        private RollTableModel tableModel;

        private RollTableModel _TableModel
        {
            get
            {
                if (tableModel != null) return tableModel;
                tableModel = new RollTableModel(RollEntries);
                return tableModel;
            }
        }


        public RollTable()
        {
            _rollEntries = new List<RollEntry> {new RollEntry(null, "Root", -1, 0, 0)};
        }

        
        public RollEntry TestRoll()
        {
            return _TableModel.TableRoll(new RollResult(this), _rollEntries);
        }

        public void CalculateDropChance()
        {
            _TableModel.CalculateDropChance();
        }


        public int IndexOfItem(RollEntry re)
        {
            return _TableModel.IndexOfItem(re);
        }

        public bool Contains(Object o)
        {
            foreach (RollEntry re in _rollEntries)
            {
                if (re.myObject == o)
                    return true;
            }

            return false;
        }

        //We dont count the Root
        public int Count => RollEntries.Count - 1;


        /// <summary>
        /// Rolls for an Entry and returns the object with the rollResult(optional)
        /// </summary>
        /// <param name="rollResult"></param>
        /// <param name="rollContext"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public RollEntry Roll(RollResult rollResult = null, List<RollEntry> rollContext = null, Type filterType = null)
        {
            if (rollResult == null)
                rollResult = new RollResult(this);
            if (rollContext == null)
                rollContext = RollEntries;
            return _TableModel.TableRoll(rollResult, rollContext, filterType);
        }

        /// <summary>
        /// Sugar for returning the final entry's containedObject instead of the entry directly
        /// </summary>
        /// <param name="rollResult"></param>
        /// <param name="rollContext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Roll<T>(RollResult rollResult = null, List<RollEntry> rollContext = null) where T : Object
        {
            if (rollResult == null)    
                rollResult = new RollResult(this);
            if (rollContext == null)
                rollContext = RollEntries;
            return _TableModel.TableRoll(rollResult, rollContext, typeof(T)).GetContainedClass<T>();
        }

        public void AddObjects(Object[] objects)
        {
            _TableModel.AddObjectsToTree(objects);
        }

        public void AddObject(Object obj)
        {
            _TableModel.AddObjectToTree(obj);
        }

        public void RemoveObject(Object obj)
        {
            _TableModel.RemoveObject(obj);
        }

        public void RemoveObjects(Object[] objs)
        {
            _TableModel.RemoveObjects(objs);
        }

        public void RemoveEntry(RollEntry entry)
        {
            if (entry == null)
            {
                Debug.Log("Cant Remove NULL RollEntry");
            }

            List<RollEntry> removeList = new List<RollEntry>() {entry};
            _TableModel.RemoveElements(removeList);
        }

        public void RemoveEntries(List<RollEntry> removeList)
        {
            _TableModel.RemoveElements(removeList);
        }


        protected int TotalWeight => _TableModel.TotalWeight();
    }
}