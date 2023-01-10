using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [HideInInspector] public Action actionSelected = Action.Default;
    private bool _actionSelectorShown = false;
    [HideInInspector] public bool alreadyMoved = false;

    [FormerlySerializedAs("ActionSelectorPanel")]
    [Header("Drag'n Drop")]
    [SerializeField] private GameObject actionSelectorPanel;
    [SerializeField] private Button moveButton;
    [SerializeField] private GameObject equipSelectorPanel;
    
    [HideInInspector] public StuffSelected stuffSelected = StuffSelected.Default;

    private Active _activeOne = null;
    private int _activeOneCd = 0;
    
    private Active _activeTwo = null;
    private int _activeTwoCd = 0;

    private Consumable _consumable = null;

    [Header("Init Aff")]
    public Transform InitPanel;
    public GameObject PlayerInitPanel;
    public GameObject WarriorInitPanel;
    public GameObject ThiefInitPanel;
    public GameObject ClericInitPanel;
    public GameObject WizardInitPanel;
    public GameObject EnemyInitPanel;

    public static Action<GameObject> setInitAction;
    
    private Dictionary<GameObject,GameObject> _playerPanelList = new Dictionary<GameObject, GameObject>();

    public static bool Moving = false; 

    [Header("Status Icons")]
    public Sprite BurnIcon;
    public Sprite StunIcon;
    public Sprite PoisonIcon;
    public Sprite FreezeIcon;

    private void Awake()
    {
        setInitAction = AddUnitInitUi;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && !_actionSelectorShown && !EventSystem.current.IsPointerOverGameObject() && TacticsMovement.PlayersTurn)
        {
            ShowActionSelector();
        }
        
        UpdateHpUi();
        
            
        if (Input.GetMouseButtonUp(1))//On release right click
        {
            switch (_actionSelectorShown)
            {
                case true:
                    HideActionSelector();
                    FindObjectOfType<AudioManager>().RandomPitch("CloseActionMenu");
                    break;
                case false:
                    switch (actionSelected)
                    {
                        case Action.Default:
                            if(alreadyMoved)
                            {
                                actionSelected = Action.CancelMove;
                            }
                            return;
                        case Action.Move:
                            if(alreadyMoved)
                                break;
                            actionSelected = Action.Default;
                            ShowActionSelector();
                            break;
                        case Action.Attack:
                            actionSelected = Action.Default;
                            ShowActionSelector();
                            break;
                        case Action.Stay:
                            actionSelected = Action.Default;
                            ShowActionSelector();
                            break;
                        case Action.Equip:
                            actionSelected = Action.Default;
                            HideEquipSelector();
                            stuffSelected = StuffSelected.Default;
                            ShowActionSelector();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
            }
        }
    }
    
    public void ShowActionSelector()
    {
        FindObjectOfType<AudioManager>().RandomPitch("OpenActionMenu");
        _actionSelectorShown = true;
        actionSelectorPanel.SetActive(true);
        
        moveButton.GetComponentInChildren<TextMeshProUGUI>().text = alreadyMoved ? "Return" : "Move";

        if(TurnManager.GetCurrentPlayerD().GetComponent<CombatStat>().GetStatusEffect() == StatusEffect.Stun)
            actionSelectorPanel.transform.Find("AttackButton").GetComponent<Button>().interactable = false;
        else
            actionSelectorPanel.transform.Find("AttackButton").GetComponent<Button>().interactable = true;
    }

    public void HideActionSelector()
    {
        _actionSelectorShown = false;
        actionSelectorPanel.SetActive(false);
    }
    
    public void ShowEquipSelector()
    {
        /*_equipSelectorShown = true;*/
        equipSelectorPanel.SetActive(true);

        if (_activeOne == null)
        {
            equipSelectorPanel.transform.GetChild(0).gameObject.GetComponent<Button>().interactable = false;
            equipSelectorPanel.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            equipSelectorPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            equipSelectorPanel.transform.GetChild(0).GetComponent<StuffButtonOver>().enabled = false;
        }
        else if(_activeOneCd > 0)
        {
            equipSelectorPanel.transform.GetChild(0).gameObject.GetComponent<Button>().interactable = false;
            equipSelectorPanel.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = ""+_activeOneCd;
            equipSelectorPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
            equipSelectorPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = _activeOne.logo;
            equipSelectorPanel.transform.GetChild(0).GetComponent<StuffButtonOver>().enabled = true;
            equipSelectorPanel.transform.GetChild(0).GetComponent<StuffButtonOver>().ChangeStuff(_activeOne);
        }
        else
        {
            equipSelectorPanel.transform.GetChild(0).gameObject.GetComponent<Button>().interactable = true;
            equipSelectorPanel.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            equipSelectorPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
            equipSelectorPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = _activeOne.logo;
            equipSelectorPanel.transform.GetChild(0).GetComponent<StuffButtonOver>().enabled = true;
            equipSelectorPanel.transform.GetChild(0).GetComponent<StuffButtonOver>().ChangeStuff(_activeOne);
        }
        if (_activeTwo == null)
        {
            equipSelectorPanel.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = false;
            equipSelectorPanel.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            equipSelectorPanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            equipSelectorPanel.transform.GetChild(1).GetComponent<StuffButtonOver>().enabled = false;
        }
        else if (_activeTwoCd > 0)
        {
            equipSelectorPanel.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = false;
            equipSelectorPanel.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = ""+_activeTwoCd;
            equipSelectorPanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
            equipSelectorPanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().sprite = _activeTwo.logo;
            equipSelectorPanel.transform.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(_activeTwo);
            equipSelectorPanel.transform.GetChild(1).GetComponent<StuffButtonOver>().enabled = true;
        }
        else
        {
            equipSelectorPanel.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = true;
            equipSelectorPanel.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            equipSelectorPanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
            equipSelectorPanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().sprite = _activeTwo.logo;
            equipSelectorPanel.transform.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(_activeTwo);
            equipSelectorPanel.transform.GetChild(1).GetComponent<StuffButtonOver>().enabled = true;
        }
        if (_consumable == null)
        {
            equipSelectorPanel.transform.GetChild(2).gameObject.GetComponent<Button>().interactable = false;
            equipSelectorPanel.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            equipSelectorPanel.transform.GetChild(2).GetComponent<StuffButtonOver>().enabled = false;
        }
        else
        {
            equipSelectorPanel.transform.GetChild(2).gameObject.GetComponent<Button>().interactable = true;
            equipSelectorPanel.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
            equipSelectorPanel.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Image>().sprite = _consumable.logo;
            equipSelectorPanel.transform.GetChild(2).GetComponent<StuffButtonOver>().enabled = true;
            equipSelectorPanel.transform.GetChild(2).GetComponent<StuffButtonOver>().ChangeStuff(_consumable);
        }
    }


    public void HideEquipSelector()
    {
        /*_equipSelectorShown = false;*/
        equipSelectorPanel.SetActive(false);
    }

    public void MoveSelected()
    {
        if (alreadyMoved)
        {
            actionSelected = Action.CancelMove;
            HideActionSelector();
        }
        else
        {
            actionSelected = Action.Move;
            HideActionSelector();
        }
    }
    
    public void AttackSelected()
    {
        actionSelected = Action.Attack;
        HideActionSelector();
    }
    
    public void StaySelected()
    {
        actionSelected = Action.Stay;
        HideActionSelector();
    }

    public void Reset()
    {
        _actionSelectorShown = false;
        actionSelected = Action.Default;
        stuffSelected = StuffSelected.Default;
        alreadyMoved = false;
    }

    public void WeaponSelectionEquipOne()
    {
        stuffSelected = StuffSelected.EquipOne;
    }

    public void WeaponSelectionEquipTwo()
    {
        stuffSelected = StuffSelected.EquipTwo;
    }

    public void WeaponSelectionEquipConsum()
    {
        stuffSelected = StuffSelected.Consum;
    }

    public void SetStuff(Active equipOne, Active equipTwo, Consumable consum)
    {
        _activeOne = equipOne;
        _activeTwo = equipTwo;
        _consumable = consum;
    }

    public void SetCd(int CdOne, int CdTwo)
    {
        _activeOneCd = CdOne;
        _activeTwoCd = CdTwo;
    }

    private void AddUnitInitUi(GameObject unit)
    {
        PlayerMovement playerMovement = unit.GetComponent<PlayerMovement>();
        CombatStat combatStat = unit.GetComponent<CombatStat>();
        GameObject t;
        if (playerMovement != null)
        {
            switch (playerMovement.charaClass)
            {
                case Perso.Default:
                    t = Instantiate(PlayerInitPanel, InitPanel);
                    break;
                case Perso.Warrior:
                    t = Instantiate(WarriorInitPanel, InitPanel);
                    break;
                case Perso.Thief:
                    t = Instantiate(ThiefInitPanel, InitPanel);
                    break;
                case Perso.Cleric:
                    t = Instantiate(ClericInitPanel, InitPanel);
                    break;
                case Perso.Wizard:
                    t = Instantiate(WizardInitPanel, InitPanel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            t.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + combatStat.CurrHp;
            t.transform.GetChild(1).Find("FillHpImg").GetComponent<Image>().fillAmount =
                (combatStat.CurrHp / (float)combatStat.MaxHp);
            if (playerMovement.GetPassive() == null)
            {
                t.transform.Find("PassiveImg").gameObject.SetActive(false);
            }
            else
            {
                t.transform.Find("PassiveImg").GetComponent<StuffButtonOver>().ChangeStuff(playerMovement.GetPassive());
                t.transform.Find("PassiveImg").GetComponent<Image>().sprite = playerMovement.GetPassive().logo;
            }
        }
        else
        {
            //NPCMove npcMove = unit.GetComponent<NPCMove>();
            t = Instantiate(EnemyInitPanel, InitPanel);
        }
        _playerPanelList.Add(unit, t);
        t.transform.Find("ArmorImg").gameObject.SetActive(false);
        t.transform.Find("StatusImg").gameObject.SetActive(false);
    }

    private void UpdateHpUi()
    {
        foreach(KeyValuePair<GameObject, GameObject> t in _playerPanelList)
        {
            if(t.Key.GetComponent<PlayerMovement>() != null)
            {
                t.Value.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    "" + t.Key.GetComponent<CombatStat>().CurrHp;
                t.Value.transform.GetChild(1).Find("FillHpImg").GetComponent<Image>().fillAmount =
                    (t.Key.GetComponent<CombatStat>().CurrHp / (float)t.Key.GetComponent<CombatStat>().MaxHp);
            
                if (t.Key.GetComponent<PlayerMovement>().GetPassive() == null)
                {
                    t.Value.transform.Find("PassiveImg").gameObject.SetActive(false);
                }
                else
                {
                    t.Value.transform.Find("PassiveImg").GetComponent<StuffButtonOver>().ChangeStuff(t.Key.GetComponent<PlayerMovement>().GetPassive());
                    t.Value.transform.Find("PassiveImg").GetComponent<Image>().sprite =
                        t.Key.GetComponent<TacticsMovement>().GetPassive().logo;
                }
            }

            int armor = t.Key.GetComponent<CombatStat>().GetArmor();
            if(armor > 0)
            {
                t.Value.transform.Find("ArmorImg").gameObject.SetActive(true);
                t.Value.transform.Find("ArmorImg").GetComponentInChildren<TextMeshProUGUI>().text =
                    armor.ToString();
            }
            else
            {
                t.Value.transform.Find("ArmorImg").gameObject.SetActive(false);
            }

            StatusEffect status = t.Key.GetComponent<CombatStat>().GetStatusEffect();
            if(status != StatusEffect.Nothing)
            {
                t.Value.transform.Find("StatusImg").gameObject.SetActive(true);
                t.Value.transform.Find("StatusImg").GetComponentInChildren<TextMeshProUGUI>().text =
                    t.Key.GetComponent<CombatStat>().StatusValue.ToString();
                
                switch(status)
                {
                    case StatusEffect.Poison:
                        t.Value.transform.Find("StatusImg").GetComponent<Image>().sprite = PoisonIcon;
                        break;
                    case StatusEffect.Stun:
                        t.Value.transform.Find("StatusImg").GetComponent<Image>().sprite = StunIcon;
                        t.Value.transform.Find("StatusImg").GetComponentInChildren<TextMeshProUGUI>().text = "";
                        break;
                    case StatusEffect.Burn:
                        t.Value.transform.Find("StatusImg").GetComponent<Image>().sprite = BurnIcon;
                        break;
                    case StatusEffect.Freeze:
                        t.Value.transform.Find("StatusImg").GetComponent<Image>().sprite = FreezeIcon;
                        break;
                    case StatusEffect.Nothing:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                t.Value.transform.Find("StatusImg").gameObject.SetActive(false);
            }
        }
    }

    public void UiButtonSfx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("UiClic");
    }
}