using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollAnything
{
    [System.Serializable]
    public class RollEntry : TreeElement
    {
        public Object MyObject;
        public int Weight;
        public float localDropChance;
        public float totalDropChance;
        public int m_DropTimes;


        public RollEntry(Object targetMyObject, string name = "", int depth = 0, int id = 0, int weight = 1)
        {
            MyObject = targetMyObject;
            m_Name = name;
            m_Depth = depth;
            m_ID = id;
            Weight = weight;
            //   m_GuaranteeBonus = guaranteeBonus;
        }


        public int TotalWeight
        {
            get { return Weight; }
        }

        public void SetLocalDropChance(int localContextWeight)
        {
            localDropChance = (Weight * 1.0f / localContextWeight * 1.0f) * 100f;
            //Debug.Log(m_Name + " (w: " + m_Weight + " %: " + localDropChance + ")");
        }

        public void SetTotalDropChance(int fullContextWeight)
        {
            totalDropChance = (Weight * 1.0f / fullContextWeight * 1.0f) * 100f;
            //Debug.Log(m_Name + " (w: " + m_Weight + " %: " + localDropChance + ")");
        }


        public bool HasObject(Object o)
        {
            if (MyObject == null)
                return false;
            return MyObject.Equals(o);
        }

        public System.Type EntryType()
        {
            if (MyObject == null) return null;
            return MyObject.GetType();
        }
    }
}