using System;
using UnityEngine;
using UnityEngine.Events;

namespace RollTableExamples
{
    public enum DamageResult
    {
        Hit,
        Ignored,
        Dead
    }

    public interface IDamageable
    {
        DamageResult Damage(float amount);
    }

    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 10;
        float health = 10;

        [SerializeField] private float damageCD = 0.1f;
        private float damageCDCount = 0;
        public bool RecentlyDamaged { get; private set; }

        public UnityEvent onDamaged;
        public UnityEvent onDead;

        void Start()
        {
            Reset();
        }

        private void Update()
        {
            if (!RecentlyDamaged)
                return;
            if (damageCDCount < damageCD)
            {
                damageCDCount += Time.deltaTime;
                return;
            }

            damageCDCount = 0;
            RecentlyDamaged = false;
        }

        void Reset()
        {
            health = maxHealth;
            damageCDCount = 0;
            RecentlyDamaged = false;
        }

        public DamageResult Damage(float damageAmount = 1)
        {
            if (RecentlyDamaged)
                return DamageResult.Ignored;
            health -= damageAmount;
            onDamaged.Invoke();
            if (health <= 0)
            {
                Kill();
                return DamageResult.Dead;
            }

            return DamageResult.Hit;
        }


        public void Kill()
        {
            onDead.Invoke();
            Destroy(gameObject);
        }
    }
}