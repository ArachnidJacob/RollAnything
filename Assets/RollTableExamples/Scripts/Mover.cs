using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Mover : MonoBehaviour
{
    public bool Moving = false;
    [SerializeField] private bool localDirection = false;

    [SerializeField] private Vector3 directionToMove;

    public Vector3 DirectionToMove
    {
        get
        {
            if (localDirection)
                return transform.rotation * directionToMove;
            return directionToMove;
        }
    }


    void Update()
    {
        if (!Moving) return;

        transform.Translate(DirectionToMove * Time.deltaTime);
    }
}