using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CamButton : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] private Direction direction;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        int dir = 0;
        if(direction == Direction.Left)dir = -1;
        else dir = 1;
        RotationArene.TurnCamAction(dir);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        RotationArene.TurnCamAction(0);
    }

    public enum Direction
    {
        Left,
        Right,
    }
}
