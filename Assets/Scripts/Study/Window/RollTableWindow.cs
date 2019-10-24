using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using RollAnything;

public class RollTableWindow : EditorWindow 
{
    [MenuItem("Diluvion/Window/Roll Tables")]
    static void OpenWindow()
    {
        var window = GetWindow<RollTableWindow>();
//        window.position = GUIContent.GetEditorWindowRect().AlignCenter(800, 600);
        window.titleContent = new GUIContent("Roll Tables");
    }
    
//    protected override Tree BuildMenuTree()
//    {
////        var tree = new OdinMenuTree(true);
////		
////        var customMenuStyle = new OdinMenuStyle
////        {
////            BorderPadding = 0f,
////            AlignTriangleLeft = true,
////            TriangleSize = 16f,
////            TrianglePadding = 0f,
////            Offset = 20f,
////            Height = 23,
////            IconPadding = 0f,
////            BorderAlpha = 0.323f
////        };
//
//        tree.DefaultMenuStyle = customMenuStyle;
//        tree.Config.DrawSearchToolbar = true;
//
//        tree.AddAllAssetsAtPath("Roll Tables", "Prefabs/RollTableObjects", typeof(Table), true);
//        tree.AddAllAssetsAtPath("Roll Tables", "Prefabs/Explorables", typeof(Table), true);
//
//        return tree;
//    }
}
