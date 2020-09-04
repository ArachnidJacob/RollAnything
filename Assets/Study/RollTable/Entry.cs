using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RollAnything
{
    [CreateAssetMenu(fileName = "new rollObject", menuName = "Diluvion/RollTables/TableRoll Object")]
    public class Entry : ScriptableObject
    {
        [Tooltip(
            "The m_Weight of the object, determines how often it can be rolled (Is relative to all weighted objects in the table it is in")]
        public int weight = 10;


        [Tooltip("The tags which roller looks to compare its own tags against")]
        public List<Tag> tags = new List<Tag>();


        #region tags

        /// <summary>
        /// Returns true if the roller has at least all the tags this entry has
        /// </summary>
        /// <returns></returns>
        public virtual bool AllTagsTrue(IRoller roller)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                if (tags[i] == null) return false;
                if (!tags[i].IsValid(roller))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if this entry has at least all the tags the roller has
        /// </summary>
        public virtual bool MatchRollerTags(IRoller roller)
        {
            for (int i = 0; i < roller.RollingTags.Count; i++)
            {
                if (roller.RollingTags[i] == null) continue;

                if (!tags.Contains(roller.RollingTags[i])) return false;
            }

            //Debug.Log("Matched tags!",this);
            return true;
        }


        public virtual void AddTagList(List<Tag> tagsToAdd)
        {
            foreach (Tag t in tagsToAdd)
            {
                if (!tags.Contains(t))
                    tags.Add(t);
            }
        }

        #endregion

        /// <summary>
        /// Checks to see if this object is affordable
        /// </summary>
        public virtual bool CanAfford(int budget)
        {

            if (budget < Value())
                return false;

            return true;
        }


        /// <summary>
        /// Arbitrary value of the entry, can change depending on what kind of entry it is.
        /// </summary>
        ///<para>Explorables use their population amount as value, ships use danger as value</para>
        /// <returns></returns>
        public virtual int Value()
        {
            return 0;
        }


        public void ExplainObject()
        {
            Debug.Log(ToString(), this);
        }

        protected string fullString;

        public override string ToString()
        {
            fullString = "<color=yellow><b>" + this.name + "</b></color> can be rolled if ";


            fullString += " the table has more than <color=cyan>" + Value() + "</color> Value \n";

            foreach (Tag tag in tags)
            {
                fullString += "and, <color = yellow><b>" + tag.ToString() + "</b></color> \n";
            }

            return fullString;
        }
    }
}