using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using RollAnything;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;


//Unneccesary because a rollentry has no meaning outside of a context of other rollentries(?!)
[CustomPropertyDrawer(typeof(RollEntry))]
public class RollEntryPropertyDrawer : PropertyDrawer
{

}
