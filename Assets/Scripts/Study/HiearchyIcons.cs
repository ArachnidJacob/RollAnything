using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using SpiderWeb;


[InitializeOnLoad]
class HierachyIcons
{
    static Texture2D selectedConfigPanel;
    static Texture2D playerTexturePanel;
    //static Texture2D traderTexturePanel;
    static Texture2D armedTexturePanel;
    //static Texture2D openWaterTexture;
    //static Texture2D ceilingTexture;
    //static Texture2D groundedTexture;
    static Texture2D speechBubble;
	static Texture2D checkpoint;
	static Texture2D tutorial;
    static List<int> markedObjects;

    static HierachyIcons()
    {
        // Init
     
        playerTexturePanel = AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/diluvion player icon.png", typeof(Texture2D)) as Texture2D;     
        //traderTexturePanel = AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/diluvion trader icon.png", typeof(Texture2D)) as Texture2D;
        armedTexturePanel = AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/diluvion pirate icon.png", typeof(Texture2D)) as Texture2D;
        //openWaterTexture = AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/Open Explorable Icon.png", typeof(Texture2D)) as Texture2D;
        //ceilingTexture = AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/Ceiling Explorable Icon.png", typeof(Texture2D)) as Texture2D;
        //groundedTexture = AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/Grounded Explorable Icon.png", typeof(Texture2D)) as Texture2D;
        speechBubble = AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/speechBubble.png", typeof(Texture2D)) as Texture2D;
		checkpoint = AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/checkpoint.png", typeof(Texture2D)) as Texture2D;
		tutorial = AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/tutorial.png", typeof(Texture2D)) as Texture2D;
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
    }

  
    static void HierarchyItemCB(int instanceID, Rect selectionRect)
    {
        // place the icon to the right of the list:
        Rect r = new Rect(selectionRect);
        r.x = r.width - 20;
        r.width = 20;
      

        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (go == null) return;       
       
       

    }
}