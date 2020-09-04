using System.Collections;
using System.Collections.Generic;
using RollAnything;
using UnityEngine;

namespace RollAnything
{
    [ExecuteInEditMode]
    [CreateAssetMenu(fileName = "new TableRoll Table", menuName = "RollAnything/TableRoll Table")]
    public class RollTableAsset : ScriptableObject
    {
        public RollTable rollTable;

    }
}