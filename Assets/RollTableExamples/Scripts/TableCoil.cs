using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RollAnything;
using UnityEngine.Analytics;

namespace RollTableExamples
{
    [RequireComponent(typeof(SphereCollider))]
    public class TableCoil : MonoBehaviour, IReRollable
    {
        [SerializeField] protected float _defaultDamage = 5;
        [SerializeField] protected float _range = 5;
        [SerializeField] protected RollTable _zappables;
        [SerializeField] protected LineRenderer _zapLine;


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

        public RollTable NestedTable => _zappables;

        /// <summary>
        /// Zap a random zappable in the list, damage it unless it has its own tablecoil.
        /// </summary>
        /// <param name="damage"></param>
        public void ZapSomething(float damage = 0)
        {
            if (_zappables.Count < 1)
                return;

            Health healthInstance = _zappables.Roll<Health>(null, NotRecentlyZapped);

            ZapLineEffect(new List<Health>() {healthInstance});

            if (Math.Abs(damage) < .01f)
                damage = _defaultDamage;
            healthInstance.Damage(damage);
        }

        bool NotRecentlyZapped(RollEntry entry)
        {
            return entry.GetContainedClass<Health>().RecentlyDamaged;
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

        void Start()
        {
            InitSphereCollider();
        }

        private void ZapLineEffect(List<Health> rolledHealths)
        {
            int entryLength = rolledHealths.Count;

            LineRenderer zapLineInstance = Instantiate(_zapLine, transform.position, transform.rotation);
            zapLineInstance.positionCount = entryLength + 1; // Ensure we have a base position
            zapLineInstance.SetPosition(0, transform.position);
            //skip first entry because first pos is always transform
            for (int i = 0; i < entryLength; i++)
            {
                Health target = rolledHealths[i];

                zapLineInstance.SetPosition(i + 1, target.transform.position);
            }

            Destroy(zapLineInstance.gameObject, 0.1f);
        }
    }
}