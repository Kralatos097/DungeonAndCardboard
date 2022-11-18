using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DungeonUiManager : MonoBehaviour
{
    public static bool inChoice = false;
    private Stuff _newStuff;

    [SerializeField] private Sprite emptyIcon;
    
    [Header("ArtworksPanel")]
    [SerializeField] private GameObject bossPanel;
    [SerializeField] private GameObject lootTrapPanel;
    [SerializeField] private GameObject lootAmbushPanel;
    [SerializeField] private GameObject lootStuffPanel;
    [SerializeField] private GameObject lootConsumablePanel;
    [SerializeField] private GameObject lootNothingPanel;
    [SerializeField] private GameObject treasureConsumablePanel;
    [SerializeField] private GameObject treasureStuffPanel;
    [SerializeField] private GameObject fightPanel;
    [SerializeField] private GameObject restPanel;

    [Header("StuffSelectionPanel")]
    [SerializeField] private GameObject lootSelectCanvas;
    
    [SerializeField] private GameObject stuffCharaSelectPanel;
    [SerializeField] private GameObject stuffReplaceSelectPanel;
    [SerializeField] private GameObject stuffIconPanel;
    [SerializeField] private Transform playerInfoPanel;
    
    public delegate void RoomArtworkUi();
    public static RoomArtworkUi BossUi;
    public static RoomArtworkUi FightUi;
    public static RoomArtworkUi TreasureConsumableUi;
    public static RoomArtworkUi TreasureStuffUi;
    public static RoomArtworkUi RestUi;
    public static RoomArtworkUi LootStuffUi;
    public static RoomArtworkUi LootConsumableUi;
    public static RoomArtworkUi LootAmbushedUi;
    public static RoomArtworkUi LootNothingUi;
    public static RoomArtworkUi LootTrapUi;
    public static RoomArtworkUi ResetArtworkUi;
    
    public static RoomArtworkUi PlayerInfoUi;

    public static RoomArtworkUi DisplayLootActionSelectorUI;
    public static RoomArtworkUi ResetLootActionSelectorUI;
    
    public delegate void StuffChoiceUi(Stuff newStuff);
    public static StuffChoiceUi StuffChoiceAction;
    private Perso _charaSelected;
    private Stuff _changedStuff;

    private void Awake()
    {
        PlayerInfoUi = SetUiPlayerInfo;
    }

    // Start is called before the first frame update
    void Start()
    {
        LootTrapUi += DisplayTrapArtwork;
        LootTrapUi += InChoiceChange;
        
        LootAmbushedUi += DisplayAmbushArtwork;
        LootAmbushedUi += InChoiceChange;
        
        LootConsumableUi += DisplayLootConsumableArtwork;
        LootConsumableUi += InChoiceChange;
        
        LootStuffUi += DisplayLootStuffArtwork;
        LootStuffUi += InChoiceChange;

        LootNothingUi += DisplayLootNothingArtwork;
        LootNothingUi += InChoiceChange;

        BossUi += DisplayBossArtwork;
        BossUi += InChoiceChange;

        FightUi += DisplayFightArtwork;
        FightUi += InChoiceChange;

        RestUi += DisplayRestArtwork;
        RestUi += InChoiceChange;

        TreasureConsumableUi += DisplayTreasureConsuArtwork;
        TreasureConsumableUi += InChoiceChange;

        TreasureStuffUi += DisplayTreasureStuffArtwork;
        TreasureStuffUi += InChoiceChange;

        ResetArtworkUi += ResetArtwork;

        DisplayLootActionSelectorUI += DisplayLootActionSelector;

        ResetLootActionSelectorUI += ResetLootActionSelector;
        
        StuffChoiceAction = StuffChoice;
    }
    
    private void DisplayTrapArtwork()
    {
        lootTrapPanel.SetActive(true);
    }

    private void DisplayAmbushArtwork()
    {
        lootAmbushPanel.SetActive(true);
    }
    
    private void DisplayLootStuffArtwork()
    {
        lootStuffPanel.SetActive(true);
    }
    
    private void DisplayLootConsumableArtwork()
    {
        lootConsumablePanel.SetActive(true);
    }

    private void DisplayLootNothingArtwork()
    {
        lootNothingPanel.SetActive(true);
    }

    private void DisplayRestArtwork()
    {
        restPanel.SetActive(true);
    }

    private void DisplayBossArtwork()
    {
        bossPanel.SetActive(true);
    }

    private void DisplayFightArtwork()
    {
        fightPanel.SetActive(true);
    }

    private void DisplayTreasureStuffArtwork()
    {
        treasureStuffPanel.SetActive(true);
    }

    private void DisplayTreasureConsuArtwork()
    {
        treasureConsumablePanel.SetActive(true);
    }

    public static void InChoiceChange()
    {
        inChoice = !inChoice;
    }

    private void ResetArtwork()
    {
        bossPanel.SetActive(false);
        fightPanel.SetActive(false);
        lootTrapPanel.SetActive(false);
        lootAmbushPanel.SetActive(false);
        lootStuffPanel.SetActive(false);
        lootConsumablePanel.SetActive(false);
        lootNothingPanel.SetActive(false);
        restPanel.SetActive(false);
        treasureStuffPanel.SetActive(false);
        treasureConsumablePanel.SetActive(false);
    }

    private void DisplayLootActionSelector()
    {
        lootSelectCanvas.SetActive(true);
    }
    
    private void ResetLootActionSelector()
    {
        lootSelectCanvas.SetActive(false);
    }
    
    private void StuffChoice(Stuff stuff)
    {
        _newStuff = stuff;
        stuffCharaSelectPanel.SetActive(true);
        stuffCharaSelectPanel.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = stuff.logo;
    }
    
    public void CharaSelect(int nb)
    {
        stuffIconPanel.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = _newStuff.logo;
        ChoiceButtonInteract();
        switch(nb)
        {
            case 0: //Warrior
                _charaSelected = Perso.Warrior;
                EquipChoiceIconChange(WarriorInfo.ActiveOne, stuffIconPanel.transform.GetChild(1).gameObject);
                EquipChoiceIconChange(WarriorInfo.ActiveTwo, stuffIconPanel.transform.GetChild(2).gameObject);
                EquipChoiceIconChange(WarriorInfo.Passive, stuffIconPanel.transform.GetChild(3).gameObject);
                EquipChoiceIconChange(WarriorInfo.Consumable, stuffIconPanel.transform.GetChild(4).gameObject);
                break;
            
            case 1: //Thief
                _charaSelected = Perso.Thief;
                EquipChoiceIconChange(ThiefInfo.ActiveOne, stuffIconPanel.transform.GetChild(1).gameObject);
                EquipChoiceIconChange(ThiefInfo.ActiveTwo, stuffIconPanel.transform.GetChild(2).gameObject);
                EquipChoiceIconChange(ThiefInfo.Passive, stuffIconPanel.transform.GetChild(3).gameObject);
                EquipChoiceIconChange(ThiefInfo.Consumable, stuffIconPanel.transform.GetChild(4).gameObject);
                break;
            
            case 2: //Cleric
                _charaSelected = Perso.Cleric;
                EquipChoiceIconChange(ClericInfo.ActiveOne, stuffIconPanel.transform.GetChild(1).gameObject);
                EquipChoiceIconChange(ClericInfo.ActiveTwo, stuffIconPanel.transform.GetChild(2).gameObject);
                EquipChoiceIconChange(ClericInfo.Passive, stuffIconPanel.transform.GetChild(3).gameObject);
                EquipChoiceIconChange(ClericInfo.Consumable, stuffIconPanel.transform.GetChild(4).gameObject);
                break;
            
            case 3: //Wizard
                _charaSelected = Perso.Wizard;
                EquipChoiceIconChange(WizardInfo.ActiveOne, stuffIconPanel.transform.GetChild(1).gameObject);
                EquipChoiceIconChange(WizardInfo.ActiveTwo, stuffIconPanel.transform.GetChild(2).gameObject);
                EquipChoiceIconChange(WizardInfo.Passive, stuffIconPanel.transform.GetChild(3).gameObject);
                EquipChoiceIconChange(WizardInfo.Consumable, stuffIconPanel.transform.GetChild(4).gameObject);
                break;
            default:
                break;
        }
        stuffCharaSelectPanel.SetActive(false);
        stuffReplaceSelectPanel.SetActive(true);
    }

    private void EquipChoiceIconChange(Stuff stuff, GameObject Go)
    {
        if (stuff != null)
        {
            Go.GetComponent<Image>().sprite = stuff.logo;
        }
        else
        {
            Go.GetComponent<Image>().sprite = emptyIcon;
        }
    }

    private void ChoiceButtonInteract()
    {
        if(_newStuff.GetType() == typeof(Consumable))
        {
            stuffIconPanel.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = false;
            stuffIconPanel.transform.GetChild(2).gameObject.GetComponent<Button>().interactable = false;
            stuffIconPanel.transform.GetChild(3).gameObject.GetComponent<Button>().interactable = false;
            stuffIconPanel.transform.GetChild(4).gameObject.GetComponent<Button>().interactable = true;
        }
        else if(_newStuff.GetType() == typeof(Passive))
        {
            stuffIconPanel.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = false;
            stuffIconPanel.transform.GetChild(2).gameObject.GetComponent<Button>().interactable = false;
            stuffIconPanel.transform.GetChild(3).gameObject.GetComponent<Button>().interactable = true;
            stuffIconPanel.transform.GetChild(4).gameObject.GetComponent<Button>().interactable = false;
        }
        else if(_newStuff.GetType() == typeof(Active))
        {
            stuffIconPanel.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = true;
            stuffIconPanel.transform.GetChild(2).gameObject.GetComponent<Button>().interactable = true;
            stuffIconPanel.transform.GetChild(3).gameObject.GetComponent<Button>().interactable = false;
            stuffIconPanel.transform.GetChild(4).gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public void ChangeEquipOneButton()
    {
        Stuff stuff = null;
        _changedStuff = _newStuff;
        switch (_charaSelected)
        {
            case Perso.Warrior:
                stuff = WarriorInfo.ActiveOne;
                WarriorInfo.ActiveOne = (Active)_newStuff;
                break;
            case Perso.Thief:
                stuff = ThiefInfo.ActiveOne;
                ThiefInfo.ActiveOne = (Active)_newStuff;
                break;
            case Perso.Cleric:
                stuff = ClericInfo.ActiveOne;
                ClericInfo.ActiveOne = (Active)_newStuff;
                break;
            case Perso.Wizard:
                stuff = WizardInfo.ActiveOne;
                WizardInfo.ActiveOne = (Active)_newStuff;
                break;
            case Perso.Default:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        GameObject buttonClicked = EventSystem.current.currentSelectedGameObject;
        
        buttonClicked.GetComponent<Button>().image.sprite = _newStuff != null ? _newStuff.logo : emptyIcon;
        stuffIconPanel.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = stuff != null ? stuff.logo : emptyIcon;

        _newStuff = stuff;
    }
    
    public void ChangeEquipTwoButton()
    {
        Stuff stuff = null;
        _changedStuff = _newStuff;
        switch (_charaSelected)
        {
            case Perso.Warrior:
                stuff = WarriorInfo.ActiveTwo;
                WarriorInfo.ActiveTwo = (Active)_newStuff;
                break;
            case Perso.Thief:
                stuff = ThiefInfo.ActiveTwo;
                ThiefInfo.ActiveTwo = (Active)_newStuff;
                break;
            case Perso.Cleric:
                stuff = ClericInfo.ActiveTwo;
                ClericInfo.ActiveTwo = (Active)_newStuff;
                break;
            case Perso.Wizard:
                stuff = WizardInfo.ActiveTwo;
                WizardInfo.ActiveTwo = (Active)_newStuff;
                break;
            case Perso.Default:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        GameObject buttonClicked = EventSystem.current.currentSelectedGameObject;
        
        buttonClicked.GetComponent<Button>().image.sprite = _newStuff != null ? _newStuff.logo : emptyIcon;
        stuffIconPanel.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = stuff != null ? stuff.logo : emptyIcon;

        _newStuff = stuff;
    }
    
    public void ChangePassifButton()
    {
        Stuff stuff = null;
        _changedStuff = _newStuff;
        switch (_charaSelected)
        {
            case Perso.Warrior:
                stuff = WarriorInfo.Passive;
                WarriorInfo.Passive = (Passive)_newStuff;
                break;
            case Perso.Thief:
                stuff = ThiefInfo.Passive;
                ThiefInfo.Passive = (Passive)_newStuff;
                break;
            case Perso.Cleric:
                stuff = ClericInfo.Passive;
                ClericInfo.Passive = (Passive)_newStuff;
                break;
            case Perso.Wizard:
                stuff = WizardInfo.Passive;
                WizardInfo.Passive = (Passive)_newStuff;
                break;
            case Perso.Default:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        GameObject buttonClicked = EventSystem.current.currentSelectedGameObject;
        
        buttonClicked.GetComponent<Button>().image.sprite = _newStuff != null ? _newStuff.logo : emptyIcon;
        stuffIconPanel.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = stuff != null ? stuff.logo : emptyIcon;

        _newStuff = stuff;
    }
    
    public void ChangeConsumableButton()
    {
        Stuff stuff = null;
        _changedStuff = _newStuff;
        switch (_charaSelected)
        {
            case Perso.Warrior:
                stuff = WarriorInfo.Consumable;
                WarriorInfo.Consumable = (Consumable)_newStuff;
                break;
            case Perso.Thief:
                stuff = ThiefInfo.Consumable;
                ThiefInfo.Consumable = (Consumable)_newStuff;
                break;
            case Perso.Cleric:
                stuff = ClericInfo.Consumable;
                ClericInfo.Consumable = (Consumable)_newStuff;
                break;
            case Perso.Wizard:
                stuff = WizardInfo.Consumable;
                WizardInfo.Consumable = (Consumable)_newStuff;
                break;
            case Perso.Default:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        GameObject buttonClicked = EventSystem.current.currentSelectedGameObject;

        buttonClicked.GetComponent<Button>().image.sprite = _newStuff != null ? _newStuff.logo : emptyIcon;
        stuffIconPanel.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = stuff != null ? stuff.logo : emptyIcon;

        _newStuff = stuff;
    }

    public void EndStuffChange()
    {
        stuffReplaceSelectPanel.SetActive(false);
        
        inChoice = false;
    }
    
    private void SetUiPlayerInfo()
    {
        //Warrior
        Transform playerPanel = playerInfoPanel.GetChild(0);
        if (WarriorInfo.MaxHp > 0)
        {
            playerPanel.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount =
                WarriorInfo.CurrentHp / (float)WarriorInfo.MaxHp;
            playerPanel.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = WarriorInfo.CurrentHp.ToString();

            if (WarriorInfo.Passive == null)
            {
                playerPanel.transform.Find("PassifImg").gameObject.SetActive(false);
            }
            else
                playerPanel.transform.Find("PassifImg").GetComponent<Image>().sprite = WarriorInfo.Passive.logo;
        }
        else
            playerPanel.gameObject.SetActive(false);

        //Thief
        playerPanel = playerInfoPanel.GetChild(1);
        if (ThiefInfo.MaxHp > 0)
        {
            playerPanel.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount =
                ThiefInfo.CurrentHp / (float)ThiefInfo.MaxHp;
            playerPanel.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = ThiefInfo.CurrentHp.ToString();

            if (ThiefInfo.Passive == null)
            {
                playerPanel.transform.Find("PassifImg").gameObject.SetActive(false);
            }
            else
                playerPanel.transform.Find("PassifImg").GetComponent<Image>().sprite = ThiefInfo.Passive.logo;
        }
        else
            playerPanel.gameObject.SetActive(false);

        //Cleric
        playerPanel = playerInfoPanel.GetChild(2);
        if (ClericInfo.MaxHp > 0)
        {
            playerPanel.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount =
                ClericInfo.CurrentHp / (float)ClericInfo.MaxHp;
            playerPanel.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = ClericInfo.CurrentHp.ToString();

            if (ClericInfo.Passive == null)
            {
                playerPanel.transform.Find("PassifImg").gameObject.SetActive(false);
            }
            else
                playerPanel.transform.Find("PassifImg").GetComponent<Image>().sprite = ClericInfo.Passive.logo;
        }
        else
            playerPanel.gameObject.SetActive(false);

        //Wizard
        playerPanel = playerInfoPanel.GetChild(3);
        if (WizardInfo.MaxHp > 0)
        {
            playerPanel.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount =
                WizardInfo.CurrentHp / (float)WizardInfo.MaxHp;
            playerPanel.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = WizardInfo.CurrentHp.ToString();

            if (WizardInfo.Passive == null)
            {
                playerPanel.transform.Find("PassifImg").gameObject.SetActive(false);
            }
            else
                playerPanel.transform.Find("PassifImg").GetComponent<Image>().sprite = WizardInfo.Passive.logo;
        }
        else
            playerPanel.gameObject.SetActive(false);
    }
}
