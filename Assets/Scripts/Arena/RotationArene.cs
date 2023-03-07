using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RotationArene : MonoBehaviour
{
    private Transform _camHolder;
    private int _dir = 0; //1 == right & -1 == left & 0 == stop
    private float rotCounter;
    
    [SerializeField] private float rotSpeed;
    [SerializeField] private float rotTime;
    
    [SerializeField] private CamButton.Direction sens;

    public static Action<int> TurnCamAction;

    private void Start()
    {
        _camHolder = this.transform;
        TurnCamAction = TurnCam;
    }

    private void Update()
    {
        if (sens == CamButton.Direction.Right)
        {
            switch (Input.mouseScrollDelta.y)
            {
                case > 0:
                    _dir = 1;
                    break;
                case < 0:
                    _dir = -1;
                    break;
                default:
                    _dir = 0;
                    break;
            }
        }
        else
        {
            switch (Input.mouseScrollDelta.y)
            {
                case > 0:
                    _dir = -1;
                    break;
                case < 0:
                    _dir = 1;
                    break;
                default:
                    _dir = 0;
                    break;
            }
        }

        if(_dir != 0)
        {
            //_camHolder.Rotate(Vector3.up, _dir * rotSpeed);
            rotCounter = rotTime;
        }

        
        if(rotCounter >= 0)
        {
            rotCounter -= Time.deltaTime;
            _camHolder.rotation = Quaternion.Lerp(_camHolder.rotation,
                Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + rotSpeed * _dir,
                    transform.eulerAngles.z), Time.deltaTime * 10);
        }
    }

    private void TurnCam(int dir)
    {
        _dir = dir;
    }

    public void RotateLeft()
    {
        _camHolder.Rotate(Vector3.up, 90);
    }

    public void RotateRight()
    {
        _camHolder.Rotate(Vector3.up, -90);
    }
}
