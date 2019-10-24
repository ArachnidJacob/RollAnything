using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RollAnything;

namespace RollTableExamples
{
    public class TableCoil : MonoBehaviour
    {
        [SerializeField] private RollTable zappables;

        [SerializeField] private LineRenderer zapLine;


        void Start()
        {
            AddZappablesToTable();

            StartCoroutine("ZapNumerator");
        }

        IEnumerator ZapNumerator()
        {
            while (true)
            {
                ZapSomething();
                yield return new WaitForSeconds(.2f);
            }
        }

        private List<ZapCube> zappableList = new List<ZapCube>();

        void ZapSomething()
        {
            RollEntry entry = zappables.Roll();
            ZapCube zappedCube = (ZapCube) entry.MyObject;
            ZapLineEffect(zappedCube.transform);
            zappedCube.Zap();
        }


        void ZapLineEffect(Transform target)
        {
            LineRenderer zapLineInstance = Instantiate<LineRenderer>(zapLine, transform.position, transform.rotation);

            zapLineInstance.SetPosition(0, transform.position);
            zapLineInstance.SetPosition(1, target.position);

            Destroy(zapLineInstance.gameObject, .1f);
        }

        void AddZappablesToTable()
        {
            zappables.AddObjects(FindObjectsOfType<ZapCube>());
        }
    }
}