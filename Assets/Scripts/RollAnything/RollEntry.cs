using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RollAnything
{
    [System.Serializable]
    public class RollEntry<T> : TreeElement where  T : Object
    {
        public T MyObject;
        public int Weight;
        public float localDropChance;
        public float totalDropChance;
        public int m_DropTimes;


        public RollEntry(T targetMyObject, string name = "", int depth = 0, int id = 0, int weight = 1)
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


        public bool HasObject(T o)
        {
            return MyObject != null && MyObject.Equals(o);
        }

        public bool HasObject(T[] oa)
        {
            if (MyObject == null)
                return false;
            if (oa != null) return oa.Contains(MyObject);
            Debug.LogError("No array to check for object");
            return false;
        }


        public System.Type EntryType()
        {
            return MyObject == null ? null : MyObject.GetType();
        }
    }
}