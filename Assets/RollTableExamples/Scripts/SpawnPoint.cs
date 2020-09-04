using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RollAnything;

namespace RollTableExamples
{
    public class SpawnPoint : MonoBehaviour, IRollWeighted
    {
        [SerializeField] private int spawnChanceWeight = 10;

        public int GetRollWeight()
        {
            return spawnChanceWeight;
        }
    }
}
