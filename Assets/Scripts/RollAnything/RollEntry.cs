using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollAnything
{
    [System.Serializable]
    public class RollEntry : TreeElement
    {
        public Object m_ObjectToRoll = null;
        public int m_Weight = 0;
        public int m_GuaranteeBonus = 0;
        public int m_GuaranteeDrop = 0;
        public float localDropChance;
        public float totalDropChance;
        public int m_DropTimes = 0;


        public RollEntry(string name, int depth, int id, int weight = 0, int guaranteeBonus = 0)
        {
            m_Name = name;
            m_Depth = depth;
            m_ID = id;

            m_Weight = weight;
            m_GuaranteeBonus = guaranteeBonus;
        }


        public int TotalWeight
        {
            get { return m_Weight + m_GuaranteeBonus; }
        }

        public void SetLocalDropChance(int localContextWeight)
        {
            localDropChance = (m_Weight * 1.0f / localContextWeight * 1.0f) * 100f;
            //Debug.Log(m_Name + " (w: " + m_Weight + " %: " + localDropChance + ")");
        }

        public void SetTotalDropChance(int fullContectWeight)
        {
            totalDropChance = (m_Weight * 1.0f / fullContectWeight * 1.0f) * 100f;
            //Debug.Log(m_Name + " (w: " + m_Weight + " %: " + localDropChance + ")");
        }


        public bool HasObject(Object o)
        {
            if (m_ObjectToRoll == null)
                return false;
            return m_ObjectToRoll.Equals(o);
        }

        public System.Type EntryType()
        {
            if (m_ObjectToRoll == null) return null;
            return m_ObjectToRoll.GetType();
        }
    }
}