using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharaUiOverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TacticsMovement unit;
    
    public void SetUnit(TacticsMovement unitP)
    {
        this.unit = unitP;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        unit.ShowUnit(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        unit.ShowUnit(false);
    }
}
