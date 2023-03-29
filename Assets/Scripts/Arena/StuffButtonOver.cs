using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class StuffButtonOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Stuff stuff;
    private StatusEffect statusEffect = StatusEffect.Nothing;
    
    private RectTransform descriptionPanel;

    private void Start()
    {
        descriptionPanel = GameObject.Find("DescriptionCanvas").transform.GetChild(0).GetComponent<RectTransform>();
    }

    private void Update()
    {
        if((Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0)) && descriptionPanel.gameObject.activeSelf)
        {
            descriptionPanel.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(stuff == null && statusEffect == StatusEffect.Nothing) return;
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
        statusEffect = StatusEffect.Nothing;
        stuff = newStuff;
    }

    public void ChangeStatus(StatusEffect status)
    {
        stuff = null;
        statusEffect = status;
    }
    
    private void InsertInfo()
    {
        if(stuff != null)
        {
            descriptionPanel.GetChild(0).GetComponent<TextMeshProUGUI>().text = stuff.name;
            descriptionPanel.GetChild(1).GetComponent<TextMeshProUGUI>().text = stuff.description;

            if (stuff.stuffType == "Passive")
            {
                descriptionPanel.GetChild(2).gameObject.SetActive(false);
                descriptionPanel.GetChild(3).gameObject.SetActive(false);
            }
            else if (stuff.stuffType == "Consumable")
            {
                descriptionPanel.GetChild(2).gameObject.SetActive(true);
                descriptionPanel.GetChild(3).gameObject.SetActive(false);
                Consumable consumable = (Consumable)stuff;
                descriptionPanel.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    consumable.GetAtkRange().ToString();
            }
            else
            {
                descriptionPanel.GetChild(2).gameObject.SetActive(true);
                descriptionPanel.GetChild(3).gameObject.SetActive(true);
                Active active = (Active)stuff;
                descriptionPanel.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    active.GetAtkRange().ToString();
                descriptionPanel.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    active.GetCd().ToString();
            }
        }
        else
        {
            descriptionPanel.GetChild(2).gameObject.SetActive(false);
            descriptionPanel.GetChild(3).gameObject.SetActive(false);
            
            switch(statusEffect)
            {
                case StatusEffect.Nothing:
                    descriptionPanel.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                    descriptionPanel.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                    break;
                case StatusEffect.Poison:
                    descriptionPanel.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Poison";
                    descriptionPanel.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Deal 1 Damage at the start of unit's next turn for X turn.";
                    break;
                case StatusEffect.Stun:
                    descriptionPanel.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Stun";
                    descriptionPanel.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Prevent from doing Action during unit's next turn.";
                    break;
                case StatusEffect.Burn:
                    descriptionPanel.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Burn";
                    descriptionPanel.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Deal X Damage at the end of unit's turn.";
                    break;
                case StatusEffect.Freeze:
                    descriptionPanel.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Freeze";
                    descriptionPanel.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Reduce by X the Movement of the unit during it's next turn.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
