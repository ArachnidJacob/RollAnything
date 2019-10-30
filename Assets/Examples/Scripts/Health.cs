using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;


namespace RollTableExamples
{
    public enum DamageResult
    {
        Hit,
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

        public UnityEvent onDamaged;
        public UnityEvent onDead;

        public void Start()
        {
            Reset();
        }

        void Reset()
        {
            health = maxHealth;
        }

        public DamageResult Damage(float damageAmount = 1)
        {
            health -= damageAmount;
            onDamaged.Invoke();
            if (health <= 0)
            {
                onDead.Invoke();
                return DamageResult.Dead;
            }

            return DamageResult.Hit;
        }
    }
}