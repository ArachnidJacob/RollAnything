using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Mover : MonoBehaviour
{

   public bool Moving = false;
   public Vector3 DirectionToMove;



   void Update()
   {
      if (!Moving) return;
      transform.Translate(DirectionToMove*Time.deltaTime);
   }

}
