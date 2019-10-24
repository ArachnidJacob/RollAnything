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
        public RollTable rollTable;

    }
}