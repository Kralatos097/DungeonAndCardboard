using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationArene : MonoBehaviour
{
    [SerializeField]
    private Transform CamHolder;

    /*private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            RotateRight();
        }
    }*/

    public void RotateLeft()
    {
        CamHolder.Rotate(Vector3.up, 90);
    }

    public void RotateRight()
    {
        CamHolder.Rotate(Vector3.up, -90);
    }
}
