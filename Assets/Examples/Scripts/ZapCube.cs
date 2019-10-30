using System.Collections;
using System.Collections.Generic;
using RollAnything;
using UnityEditor.U2D;
using UnityEngine;

namespace RollTableExamples
{
    public class ZapCube : MonoBehaviour, IRollWeighted, IZappable
    {
        [SerializeField] private int timesZapped = 0;


        private TextMesh textMesh;

        private TextMesh TextMesh
        {
            get
            {
                if (textMesh != null) return textMesh;
                textMesh = GetComponentInChildren<TextMesh>();
                return textMesh;
            }
        }

        public void Zap()
        {
            timesZapped++;
            TextMesh.text = timesZapped.ToString();
            TextMesh.fontSize++;
        }

        private const int maxWeightDistanceBonus = 100;
        private TableCoil _coil;

        private TableCoil Coil
        {
            get
            {
                if (_coil != null) return _coil;

                _coil = GetClosestTableCoil();
                return _coil;
            }
        }

        private float ClosestCoilDistance()
        {
            return Vector3.Distance(Coil.transform.position, transform.position);
        }

        public TableCoil GetClosestTableCoil()
        {
            float lowestCoilSqDistance = Mathf.Infinity;
            TableCoil contenderCoil = null;
            foreach (TableCoil tc in FindObjectsOfType<TableCoil>())
            {
                Vector3 coilVector = tc.transform.position - transform.position;
                float coilSqDistance = coilVector.sqrMagnitude;
                if (coilSqDistance > lowestCoilSqDistance)
                    continue;
                lowestCoilSqDistance = coilSqDistance;
                contenderCoil = tc;
            }
            return contenderCoil;
        }

        public int GetRollWeight()
        {
            int rollInverseDistanceValue = Mathf.FloorToInt((1 / ClosestCoilDistance() * maxWeightDistanceBonus));
            return Mathf.Clamp(rollInverseDistanceValue, 1, maxWeightDistanceBonus);
        }
    }
}