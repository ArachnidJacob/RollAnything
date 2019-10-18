using System.Collections;
using System.Collections.Generic;
using RollAnything;
using UnityEngine;

namespace RollAnything
{
    [ExecuteInEditMode]
    [CreateAssetMenu(fileName = "new Roll Table", menuName = "RollAnything/Roll Table")]
    public class RollTableAsset : ScriptableObject
    {
        
        private RollTableModel tableModel;

        public RollTableModel TableModel
        {
            get
            {
                if (tableModel != null) return tableModel;
                tableModel = new RollTableModel(_rollEntries);
                return tableModel;
            }
        }

        [SerializeField] private List<RollEntry> _rollEntries = new List<RollEntry>();

        public List<RollEntry> RollEntries
        {
            get
            {
                if (_rollEntries.Count <= 0)
                {
                    _rollEntries = new List<RollEntry>();
                    _rollEntries.Add(new RollEntry("Root", -1, 0));
                }
                return _rollEntries;
            }
            set { _rollEntries = value; }
        }
    }
}