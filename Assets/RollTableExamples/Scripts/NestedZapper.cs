using System.Collections;
using System.Collections.Generic;
using RollAnything;
using UnityEngine;

namespace RollTableExamples
{
    /// <summary>
    /// Example Component that finds a Nested Table in the hiearchy, returns the result
    /// </summary>
    public class NestedZapper : MonoBehaviour, IReRollable
    {
        private TableCoil myNestedTable;

        private TableCoil _myNestedTable
        {
            get
            {
                if (myNestedTable != null) return myNestedTable;
                myNestedTable = GetComponentInChildren<TableCoil>();
                return myNestedTable;
            }
        }

        public RollTable NestedTable
        {
            get
            {
                if (_myNestedTable == null) return null;
                return _myNestedTable.NestedTable;
            }
        }
    }
}