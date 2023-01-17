using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFacingCam : MonoBehaviour
{
    private Transform cam;

    private void Awake()
    {
        cam = FindObjectOfType<Camera>().transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
