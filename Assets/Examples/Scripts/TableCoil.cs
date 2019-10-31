using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RollAnything;

namespace RollTableExamples
{
    [RequireComponent(typeof(SphereCollider))]
    public class TableCoil : MonoBehaviour
    {
        [SerializeField] private float _defaultDamage = 5;
        [SerializeField] private float _range = 5;
        [SerializeField] private RollTable _zappables;

        [SerializeField] private LineRenderer _zapLine;


        void Start()
        {
            InitSphereCollider();
        }

        private SphereCollider mySphereCollider;

        private SphereCollider MySphereCollider
        {
            get
            {
                if (mySphereCollider != null) return mySphereCollider;

                InitSphereCollider();

                return mySphereCollider;
            }
        }

        void InitSphereCollider()
        {
            mySphereCollider = GetComponentInChildren<SphereCollider>();
            if (mySphereCollider == null)
                mySphereCollider = gameObject.AddComponent<SphereCollider>();
            mySphereCollider.radius = _range;
            mySphereCollider.isTrigger = true;
        }

        public void OnTriggerEnter(Collider otherCollider)
        {
            Health otherHealth = otherCollider.GetComponent<Health>();
            if (!otherHealth)
                return;
            if (_zappables.Contains(otherHealth))
                return;
            _zappables.AddObject(otherHealth);
        }

        public void OnTriggerExit(Collider otherCollider)
        {
            Health otherHealth = otherCollider.GetComponent<Health>();
            if (!otherHealth)
                return;
            if (!_zappables.Contains(otherHealth))
                return;
            _zappables.RemoveObject(otherHealth);
        }

        /// <summary>
        /// Zap a random zappable in my list
        /// </summary>
        /// <param name="damage"></param>
        public void ZapSomething(float damage = 0)
        {
            if (_zappables.Count < 1)
                return;
            Health healthObject = _zappables.Roll<Health>();

            ZapLineEffect(healthObject.transform);
            if (Math.Abs(damage) < .01f)
                damage = _defaultDamage;
            healthObject.Damage(damage);
        }


        void ZapLineEffect(Transform target)
        {
            LineRenderer zapLineInstance = Instantiate<LineRenderer>(_zapLine, transform.position, transform.rotation);

            zapLineInstance.SetPosition(0, transform.position);
            zapLineInstance.SetPosition(1, target.position);

            Destroy(zapLineInstance.gameObject, .1f);
        }
    }
}