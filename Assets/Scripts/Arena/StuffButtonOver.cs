using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class StuffButtonOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Stuff _stuff;
    public RectTransform descriptionPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        InsertInfo();
        
        var menuCorners = new Vector3[4];
        descriptionPanel.GetWorldCorners(menuCorners);
        
        if (transform.position.y >= Screen.height/2)
        {
            if (transform.position.x >= Screen.width / 2)
            {
                var anchoredPosition = descriptionPanel.anchoredPosition;
                var position = transform.position;
                Vector2 newPos = new Vector2(position.x - (menuCorners[2].x - anchoredPosition.x), 
                                      position.y - (menuCorners[2].y - anchoredPosition.y));
                descriptionPanel.anchoredPosition = newPos;
            }
            else
            {
                var anchoredPosition = descriptionPanel.anchoredPosition;
                var position = transform.position;
                Vector2 newPos = new Vector2(position.x - (menuCorners[1].x - anchoredPosition.x), 
                    position.y - (menuCorners[1].y - anchoredPosition.y));
                descriptionPanel.anchoredPosition = newPos;
            }
        }
        else
        {
            if (transform.position.x >= Screen.width / 2)
            {
                var anchoredPosition = descriptionPanel.anchoredPosition;
                var position = transform.position;
                Vector2 newPos = new Vector2(position.x - (menuCorners[3].x - anchoredPosition.x), 
                    position.y - (menuCorners[3].y - anchoredPosition.y));
                descriptionPanel.anchoredPosition = newPos;
            }
            else
            {
                descriptionPanel.anchoredPosition = transform.position;
            }
        }
        
        descriptionPanel.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPanel.gameObject.SetActive(false);
    }

    public void ChangeStuff(Stuff newStuff)
    {
        _stuff = newStuff;
    }
    
    private void InsertInfo()
    {
        descriptionPanel.GetChild(0).GetComponent<TextMeshProUGUI>().text = _stuff.name;
        descriptionPanel.GetChild(1).GetComponent<TextMeshProUGUI>().text = _stuff.description;

        if(_stuff.stuffType == "Passive")
        {
            descriptionPanel.GetChild(2).GetChild(0).gameObject.SetActive(false);
            descriptionPanel.GetChild(3).GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            descriptionPanel.GetChild(2).GetChild(0).gameObject.SetActive(true);
            descriptionPanel.GetChild(3).GetChild(0).gameObject.SetActive(true);
            Active active = (Active)_stuff;
            descriptionPanel.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = active.GetAtkRange().ToString();
            descriptionPanel.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = active.GetCd().ToString();
        }
    }
}
