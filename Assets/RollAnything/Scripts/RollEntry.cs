using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using RollAnything.TreeBaseClasses;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


namespace RollAnything
{
    [System.Serializable]
    public class RollEntry : TreeElement
    {
        public Object myObject;
        public int weight;
        public float localDropChance;
        public float totalDropChance;
        public int mDropTimes;


        public RollEntry(Object targetMyObject, string name = "", int depth = 0, int id = 0, int weight = 1)
        {
            myObject = targetMyObject;
            m_Name = name;
            m_Depth = depth;
            m_ID = id;
            this.weight = weight;
            //   m_GuaranteeBonus = guaranteeBonus;
        }


        public int TotalWeight
        {
            get { return weight; }
        }

        public void SetLocalDropChance(int localContextWeight)
        {
            localDropChance = (weight * 1.0f / localContextWeight * 1.0f) * 100f;
            //Debug.Log(m_Name + " (w: " + m_Weight + " %: " + localDropChance + ")");
        }

        public void SetTotalDropChance(int fullContextWeight)
        {
            totalDropChance = (weight * 1.0f / fullContextWeight * 1.0f) * 100f;
            //Debug.Log(m_Name + " (w: " + m_Weight + " %: " + localDropChance + ")");
        }


        public bool HasObject(Object o)
        {
            return myObject != null && myObject.Equals(o);
        }

        public bool HasObject(Object[] oa)
        {
            if (myObject == null)
                return false;
            if (oa != null) return oa.Contains(myObject);
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
//            Debug.Log("Checking if " + name + " has " + checkType + " in " + myObject);
            Type myObjectType = myObject.GetType();
            if (checkType == myObjectType)
                return true;

            if (myObjectType == typeof(GameObject))
            {
                GameObject myGameObject = (GameObject) myObject;

                foreach (var c in myGameObject.GetComponents(checkType))
                {
                    if (c.GetType() == checkType)
                    {
                        return true;
                    }
                }
            }

            if (myObjectType == typeof(Component))
            {
                Component myComponent = myObject as Component;
                GameObject myGameObject = myComponent?.gameObject;

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

//        public T GetContainedInterface<T>() where T : class
//        {
//            Type storedType = MyObject.GetType();
//
//            if (typeof(T) == typeof(Component))
//            {
//                if (storedType == typeof(GameObject))
//                {
//                    GameObject rolledGameObject = (GameObject) MyObject;
//                    var rolledGameObjectComponent = rolledGameObject.GetComponent<T>();
//                    if (rolledGameObjectComponent != null)
//                    {
//                        return rolledGameObjectComponent;
//                    }
//                    Debug.LogError(
//                        rolledGameObject.name + " does not have the interface: " + typeof(T).ToString() + " attached.",
//                        rolledGameObject);
//                    return null;
//                }
//                Debug.LogError(
//                    MyObject.name + " is not a gameObject, and cannot have  " + typeof(T).ToString() + " attached.",
//                    MyObject);
//                return null;
//            }
//            return MyObject as T;
//        }

        /// <summary>
        /// Retrieves the Unity Object instance in the input Entry, 
        /// </summary>
        /// <param name="rollEntry"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetContainedClass<T>() where T : class
        {
            Type storedType = myObject.GetType();
            Type checkingType = typeof(T);

            if (storedType == checkingType)
                return myObject as T;
           

            //TODO ADD HANDLER FOR INTERFACES
            if (checkingType == typeof(Component)||checkingType.IsInterface)
            {
                if (storedType == typeof(GameObject))
                {
                    GameObject rolledGameObject = (GameObject) myObject;
                    var rolledGameObjectComponent = rolledGameObject.GetComponent<T>();
                    if (rolledGameObjectComponent != null)
                    {
                        return rolledGameObjectComponent;
                    }
                } 
                if(storedType == typeof(Component))
                {
                    Component rolledGameObject = (Component) myObject;
                    var rolledGameObjectComponent = rolledGameObject.GetComponent<T>();
                    if (rolledGameObjectComponent != null)
                    {
                        return rolledGameObjectComponent;
                    }
                }
            }
            return myObject as T;
        }

        public System.Type EntryType()
        {
            return myObject == null ? null : myObject.GetType();
        }
    }
}