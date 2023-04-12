using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIManager : MonoBehaviour
{
    [HideInInspector] public Action actionSelected = Action.Default;
    private bool _actionSelectorShown = false;
    [HideInInspector] public bool alreadyMoved = false;

    [Header("Value")]
    [SerializeField] private float charaUiIncreaseValue = 1.15f;
    private Vector3 _charaUiBaseValue;
    
    [Header("Drag'n Drop")]
    [SerializeField] private GameObject actionSelectorPanel;
    [SerializeField] private Button moveButton;
    [SerializeField] private GameObject equipSelectorPanel;
    
    [Space]
    [SerializeField] private Sprite moveSprite;
    [SerializeField] private Sprite returnSprite;
    
    [HideInInspector] public StuffSelected stuffSelected = StuffSelected.Default;

    private Active _activeOne = null;
    private int _activeOneCd = 0;
    
    private Active _activeTwo = null;
    private int _activeTwoCd = 0;

    private Consumable _consumable = null;

    [Header("Init Aff")]
    [SerializeField] private Transform InitPanel;
    [SerializeField] private GameObject PlayerInitPanel;
    [SerializeField] private GameObject WarriorInitPanel;
    [SerializeField] private GameObject ThiefInitPanel;
    [SerializeField] private GameObject ClericInitPanel;
    [SerializeField] private GameObject WizardInitPanel;
    [SerializeField] private GameObject EnemyInitPanel;
    [SerializeField] private GameObject EnemyEliteInitPanel;

    [Space]
    [SerializeField] private List<Sprite> DamageSprites;

    public static Action<GameObject> setInitAction;
    public static Action<GameObject> StartTurnInitUIChangeAction;
    public static Action<GameObject> EndTurnInitUIChangeAction;
    
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
        StartTurnInitUIChangeAction = StartTurnInitUIChange;
        EndTurnInitUIChangeAction = EndTurnInitUIChange;
        
        _playerPanelList.Clear();
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
                            //HideEquipSelector();
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
        
        moveButton.GetComponent<Image>().sprite = alreadyMoved ? returnSprite : moveSprite;
        moveButton.GetComponentInChildren<TextMeshProUGUI>().text = alreadyMoved ? "Return" : "Move";
        
        ShowEquipSelector();

        bool isStun = TurnManager.GetCurrentPlayerD().GetComponent<CombatStat>().GetStatusEffect() ==
                          StatusEffect.Stun;//si on est stun alors rend désactives les bouton
        if(isStun) ActivateActions(false); 
    }
    
    private void ActivateActions(bool state)
    {
        for (int i = 0; i < equipSelectorPanel.transform.childCount; i++)
        {
            equipSelectorPanel.transform.GetChild(i).GetComponent<Button>().interactable = state;
        }
    }

    public void HideActionSelector()
    {
        _actionSelectorShown = false;
        actionSelectorPanel.SetActive(false);
        
        //HideEquipSelector();
    }

    private void ShowEquipSelector()
    {
        /*equipSelectorPanel.SetActive(true);*/
        Transform equipButton = equipSelectorPanel.transform;

        if (_activeOne == null)
        {
            equipButton.GetChild(0).gameObject.GetComponent<Button>().interactable = false;
            equipButton.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            equipButton.GetChild(0).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            equipButton.GetChild(0).GetComponent<StuffButtonOver>().enabled = false;
        }
        else if(_activeOneCd > 0)
        {
            equipButton.GetChild(0).gameObject.GetComponent<Button>().interactable = false;
            equipButton.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = ""+_activeOneCd;
            equipButton.GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
            equipButton.GetChild(0).gameObject.GetComponent<Image>().sprite = _activeOne.logo;
            equipButton.GetChild(0).GetComponent<StuffButtonOver>().enabled = true;
            equipButton.GetChild(0).GetComponent<StuffButtonOver>().ChangeStuff(_activeOne);
        }
        else
        {
            equipButton.GetChild(0).gameObject.GetComponent<Button>().interactable = true;
            equipButton.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            equipButton.GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
            equipButton.GetChild(0).gameObject.GetComponent<Image>().sprite = _activeOne.logo;
            equipButton.GetChild(0).GetComponent<StuffButtonOver>().enabled = true;
            equipButton.GetChild(0).GetComponent<StuffButtonOver>().ChangeStuff(_activeOne);
        }
        if (_activeTwo == null)
        {
            equipButton.GetChild(1).gameObject.GetComponent<Button>().interactable = false;
            equipButton.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            equipButton.GetChild(1).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            equipButton.GetChild(1).GetComponent<StuffButtonOver>().enabled = false;
        }
        else if (_activeTwoCd > 0)
        {
            equipButton.GetChild(1).gameObject.GetComponent<Button>().interactable = false;
            equipButton.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = ""+_activeTwoCd;
            equipButton.GetChild(1).gameObject.GetComponent<Image>().color = Color.white;
            equipButton.GetChild(1).gameObject.GetComponent<Image>().sprite = _activeTwo.logo;
            equipButton.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(_activeTwo);
            equipButton.GetChild(1).GetComponent<StuffButtonOver>().enabled = true;
        }
        else
        {
            equipButton.GetChild(1).gameObject.GetComponent<Button>().interactable = true;
            equipButton.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            equipButton.GetChild(1).gameObject.GetComponent<Image>().color = Color.white;
            equipButton.GetChild(1).gameObject.GetComponent<Image>().sprite = _activeTwo.logo;
            equipButton.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(_activeTwo);
            equipButton.GetChild(1).GetComponent<StuffButtonOver>().enabled = true;
        }
        if (_consumable == null)
        {
            equipButton.GetChild(2).gameObject.GetComponent<Button>().interactable = false;
            equipButton.GetChild(2).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            equipButton.GetChild(2).GetComponent<StuffButtonOver>().enabled = false;
        }
        else
        {
            equipButton.GetChild(2).gameObject.GetComponent<Button>().interactable = true;
            equipButton.GetChild(2).gameObject.GetComponent<Image>().color = Color.white;
            equipButton.GetChild(2).gameObject.GetComponent<Image>().sprite = _consumable.logo;
            equipButton.GetChild(2).GetComponent<StuffButtonOver>().enabled = true;
            equipButton.GetChild(2).GetComponent<StuffButtonOver>().ChangeStuff(_consumable);
        }
    }


    public void HideEquipSelector()
    {
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
        actionSelected = Action.Equip;
    }

    public void WeaponSelectionEquipTwo()
    {
        stuffSelected = StuffSelected.EquipTwo;
        actionSelected = Action.Equip;
    }

    public void WeaponSelectionEquipConsum()
    {
        stuffSelected = StuffSelected.Consum;
        actionSelected = Action.Equip;
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

            t.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "" + combatStat.CurrHp;
            t.transform.GetChild(1).Find("FillHpFullImg").GetComponent<Image>().fillAmount =
                (combatStat.CurrHp / (float)combatStat.baseMaxHp);
            t.transform.GetChild(1).Find("FillHpEmptyImg").GetComponent<Image>().fillAmount =
                (combatStat.MaxHp / (float)combatStat.baseMaxHp);
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
            //todo: add NPC variations
            NPCMove npcMove = unit.GetComponent<NPCMove>();

            Sprite npcSprite = npcMove.GetUiSprite();

            if(npcSprite.name.Contains("Elite"))
            {
                t = Instantiate(EnemyEliteInitPanel, InitPanel);
            }
            else
            {
                t = Instantiate(EnemyInitPanel, InitPanel);
            }
            

            t.transform.GetChild(0).GetComponent<Image>().sprite = npcSprite;
        }

        /*if(TurnManager.CombatStarted)
        {
            GameObject currentPlayer = TurnManager.GetCurrentPlayerD();
            int currentPlayerIndex = _playerPanelList[currentPlayer].transform.GetSiblingIndex();
            t.transform.SetSiblingIndex(currentPlayerIndex);
        }*/
        
        _charaUiBaseValue = t.transform.localScale;
        _playerPanelList.Add(unit, t);
        t.transform.Find("ArmorImg").gameObject.SetActive(false);
        t.transform.Find("StatusImg").gameObject.SetActive(false);
        t.GetComponentInChildren<CharaUiOverButton>().SetUnit(unit.GetComponent<TacticsMovement>());
    }

    private void StartTurnInitUIChange(GameObject unit)
    {
        bool containsKey = _playerPanelList.ContainsKey(unit);
        if(containsKey)
        {
            GameObject playerPanel = _playerPanelList[unit];
            playerPanel.transform.localScale = Vector3.one * charaUiIncreaseValue;
        }
    }
    
    private void EndTurnInitUIChange(GameObject unit)
    {
        bool containsKey = _playerPanelList.ContainsKey(unit);
        if(containsKey)
        {
            GameObject playerPanel = _playerPanelList[unit];
            playerPanel.transform.localScale = _charaUiBaseValue;

            int childCount = playerPanel.transform.parent.childCount;
            playerPanel.transform.SetSiblingIndex(childCount);
        }
    }

    private void UpdateHpUi()
    {
        foreach(KeyValuePair<GameObject, GameObject> t in _playerPanelList)
        {
            if (!t.Value.activeSelf) continue; //si l'UI a déjà été retiré alors on passe à la prochaine itération
            
            if(t.Key == null || !t.Key.GetComponent<CombatStat>().isAlive) //si le personnage n'est pas vivant alors on désactive sont UI et on passe à l'iteration suivante
            {
                GameObject temp = t.Value;
                temp.SetActive(false);
                continue;
            }

            CombatStat combatStat = t.Key.GetComponent<CombatStat>();
            if(combatStat.MaxHp == 0) return;
            if(combatStat.CurrHp/(float)combatStat.MaxHp >= .75f)
            {
                t.Value.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
            else if(combatStat.CurrHp/(float)combatStat.MaxHp >= .5f)
            {
                t.Value.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                t.Value.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = DamageSprites[0];
            }
            else if(combatStat.CurrHp/(float)combatStat.MaxHp >= .25f)
            {
                t.Value.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                t.Value.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = DamageSprites[1];
            }
            else
            {
                t.Value.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                t.Value.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = DamageSprites[2];
            }
            
            if(t.Key.GetComponent<PlayerMovement>() != null)
            {
                t.Value.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text =
                    "" + t.Key.GetComponent<CombatStat>().CurrHp;
                t.Value.transform.GetChild(1).Find("FillHpFullImg").GetComponent<Image>().fillAmount =
                    (t.Key.GetComponent<CombatStat>().CurrHp / (float)t.Key.GetComponent<CombatStat>().baseMaxHp);
                t.Value.transform.GetChild(1).Find("FillHpEmptyImg").GetComponent<Image>().fillAmount =
                    (t.Key.GetComponent<CombatStat>().MaxHp / (float)t.Key.GetComponent<CombatStat>().baseMaxHp);

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
                t.Value.transform.Find("StatusImg").GetComponent<StuffButtonOver>().ChangeStatus(status);
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