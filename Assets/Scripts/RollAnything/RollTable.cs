using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Tilemaps;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RollAnything
{
    [Serializable]
    public class RollTable<T> where T:Object
    {
        [SerializeField] private List<RollEntry<T>> _rollEntries;

        public bool expandedtable = false;
        private int _lastRoll;
        private int _currentWeight;

        List<RollEntry<T>> RollEntries
        {
            get
            {
                if (_rollEntries.Count <= 0)
                {
                    _rollEntries = new List<RollEntry<T>>();
                    _rollEntries.Add(new RollEntry<T>(null, "Root", -1, 0, 0));
                }
                return _rollEntries;
            }
            set { _rollEntries = value; }
        }


        private RollTableModel<T> _tableModel;

        private RollTableModel<T> TableModel
        {
            get
            {
                if (_tableModel != null) return _tableModel;
                _tableModel = new RollTableModel<T>(RollEntries);
                return _tableModel;
            }
        }


        public RollTable()
        {
            _rollEntries = new List<RollEntry<T>>();
            _rollEntries.Add(new RollEntry<T>(null, "Root", -1, 0, 0));
        }


        public RollEntry<T> TestRoll()
        {
            return TableModel.Roll(_rollEntries);
        }

        public void CalculateDropChance()
        {
            TableModel.CalculateDropChance();
        }


        public int IndexOfItem(RollEntry<T> re)
        {
            return TableModel.IndexOfItem(re);
        }

        public bool Contains(Object o)
        {
            foreach (RollEntry<T> re in _rollEntries)
            {
                if (re.MyObject == o)
                    return true;
            }
            return false;
        }

        public RollEntry<T> Roll(List<RollEntry<T>> rollContext = null)
        {
            return TableModel.Roll();
        }


        public void AddObjects(Object[] objects)
        {
            TableModel.AddObjectsToTree(objects);
        }


        public void AddObject(Object obj)
        {
            TableModel.AddObjectToTree(obj);
        }

        public void RemoveObject(Object obj)
        {
            TableModel.RemoveObject(obj);
        }
        
        public void RemoveObjects(Object[] objs)
        {
            TableModel.RemoveObjects(objs);
        }


        protected int TotalWeight => TableModel.TotalWeight;
    }
}


//        /// <summary>
//        /// Returns TotalWeight
//        /// </summary>
//        public int TotalWeight()
//        {
//            int currentWeight = 0;
//            for (int i = 0; i < rollableEntries.Count; i++)
//            {
//                if (rollableEntries[i] == null) continue;
//                currentWeight += rollableEntries[i].TotalWeight;
//            }
//
//            return _totalWeight = currentWeight;
//        }
//
//        public void AddRoll(Object entryObject)
//        {
//            if (entryObject == null) return;
//            if (ContainsObject(entryObject)) return;
//            //Debug.Log("adding" + entryObject.name);
//            RollEntry te = new RollEntry(entryObject);
//            rollableEntries.Add(te);
//        }
//
//
//        public void RemoveRoll(int index)
//        {
//            if (rollableEntries.Count > index)
//                rollableEntries.RemoveAt(index);
//        }
//
//
//        public void RemoveRoll(Object o)
//        {
//            if (o == null) return;
//
//            StoreForRemoval(o);
//            RemoveStored();
//        }
//
//
//        public void StoreForRemoval(Object o)
//        {
//            foreach (RollEntry re in rollableEntries)
//            {
//                if (re.HasObject(o))
//                    _removeList.Add(re);
//            }
//        }
//
//        public void RemoveStored()
//        {
//            foreach (RollEntry re in _removeList)
//                rollableEntries.Remove(re);
//        }
//
//
//        public bool ContainsType(Type t)
//        {
//            Type st = null;
//            for (int i = 0; i < containedTypes.Count; i++)
//            {
//                st = containedTypes[i];
//                if (st == null) continue;
//
//                if (st == t)
//                {
//                    //Debug.Log("IT IS!!!!", this);
//                    return true;
//                }
//            }
//
//            //Debug.Log("ITS NOT!!!!", this);
//            return false;
//        }
//
//        /// <summary>
//        /// Checks if this table has the input entryObject
//        /// </summary>
//        /// <param name="o">the roll we are looking for</param>
//        /// <returns>true if it has the entryObject</returns>
//        public bool ContainsObject(Object o)
//        {
//            if (o == null) return false;
//            foreach (RollEntry re in rollableEntries)
//            {
//                if (re.HasObject(o))
//                    return true;
//            }
//
//            return false;
//        }
//
//
//        /// <summary>
//        /// Resets all the droptimes
//        /// </summary>
//        public void ResetRollCounter()
//        {
//            foreach (RollEntry re in rollableEntries)
//            {
//                re.dropTimes = 0;
//            }
//        }
//    }