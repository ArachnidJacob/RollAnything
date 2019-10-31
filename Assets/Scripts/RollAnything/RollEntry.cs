using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;


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
            return MyObject != null && MyObject.Equals(o);
        }

        public bool HasObject(Object[] oa)
        {
            if (MyObject == null)
                return false;
            if (oa != null) return oa.Contains(MyObject);
            Debug.LogError("No array to check for object");
            return false;
        }
        
        /// <summary>
        /// Check if this entry either is or has a type
        /// </summary>
        /// <param name="checkType"></param>
        /// <returns></returns>
        public bool HasType(Type checkType)
        {
            Type objectType = MyObject.GetType();
            if (checkType == objectType)
                return true;
            if (objectType == typeof(GameObject))
            {
                GameObject myGameObject = (GameObject)MyObject;

                foreach (var c in myGameObject.GetComponents(checkType))
                {
                    if (c.GetType() == checkType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public System.Type EntryType()
        {
            return MyObject == null ? null : MyObject.GetType();
        }
    }
}