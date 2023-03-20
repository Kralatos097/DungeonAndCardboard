using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DungeonManager : MonoBehaviour
{
    //Action
    public static Action<RoomEffect> LaunchRoomEffectAction;
    public static Action<RoomType> LaunchRoomTypeAction;

    private static string _dungeonSceneName;
    
    [SerializeField] private int restAmbushBaseValue = 20;
    [SerializeField] private RangedInt trapValue;
    
    private GameObject _sceneContainer;
    public static DungeonTile CurrentTile;
    
    [Space]
    [Scene][SerializeField] private string bossScene;

    [Header("Players Card")]
    [SerializeField] private PlayerBaseInfo warriorCard;
    [SerializeField] private PlayerBaseInfo thiefCard;
    [SerializeField] private PlayerBaseInfo clericCard;
    [SerializeField] private PlayerBaseInfo wizardCard;
    
    [Header("Lists")]
    [SerializeField] private List<string> fightingSceneList;
    [SerializeField] private List<string> ambushedSceneList;
    [SerializeField] private List<Stuff> stuffList;
    [SerializeField] private List<Consumable> consumableList;

    private bool artworkShown = false;
    private RoomEffect _roomEffect;
    private LootEffect lootEffect = LootEffect.Default;
    private TreasureEffect treasureEffect = TreasureEffect.Default;

    public static Action<bool> SceneContainerSwitch;
    
    private int restAmbushValue;
    private int RestAmbushValue
    {
        get => restAmbushValue;

        set
        {
            restAmbushValue = value;
            if (restAmbushValue < 0)
                restAmbushValue = 0;
        }
    }

    private void Awake()
    {
        _dungeonSceneName = SceneManager.GetActiveScene().name;
    }
    
    void Start()
    {
        LaunchRoomEffectAction = LaunchRoomEffect;
        LaunchRoomTypeAction = LaunchRoomType;
        
        _sceneContainer = GameObject.Find("SceneContainer");
        SceneContainerSwitch = SceneContainerSwitchFunc;
        
        AssignPlayerInfo();
        DungeonUiManager.PlayerInfoUi();
        FindObjectOfType<AudioManager>().Play("Dungeon");

        restAmbushValue = restAmbushBaseValue;
    }

    private void SceneContainerSwitchFunc(bool obj)
    {
        _sceneContainer.SetActive(obj);
    }

    void Update()
    {
        DungeonUiManager.PlayerInfoUi();
        if (Input.GetMouseButtonDown(0) && artworkShown)
        {
            DungeonUiManager.ResetArtworkUi();
            artworkShown = false;
            switch(_roomEffect)
            {
                case RoomEffect.Boss:
                    LaunchBoss();
                    DungeonUiManager.InChoiceChange();
                    break;
                case RoomEffect.Fight:
                    LaunchFight();
                    DungeonUiManager.InChoiceChange();
                    break;
                case RoomEffect.Rest:
                    LaunchRest();
                    DungeonUiManager.InChoiceChange();
                    break;
                case RoomEffect.Treasure:
                    switch (treasureEffect)
                    {
                        case TreasureEffect.Stuff:
                            StuffSelection();
                            break;
                        case TreasureEffect.Consumable:
                            ConsumableSelection();
                            break;
                        case TreasureEffect.Default:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case RoomEffect.EndFightLoot:
                    switch(treasureEffect)
                    {
                        case TreasureEffect.Stuff:
                            StuffSelection();
                            break;
                        case TreasureEffect.Consumable:
                            ConsumableSelection();
                            break;
                        case TreasureEffect.Default:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case RoomEffect.Loot:
                    switch (lootEffect)
                    {
                        case LootEffect.Trap:
                            LaunchTrap();
                            DungeonUiManager.InChoiceChange();
                            break;
                        case LootEffect.Ambush:
                            LaunchAmbush();
                            DungeonUiManager.InChoiceChange();
                            break;
                        case LootEffect.Stuff:
                            StuffSelection();
                            break;
                        case LootEffect.Consumable:
                            ConsumableSelection();
                            break;
                        case LootEffect.Nothing:
                            DungeonUiManager.InChoiceChange();
                            break;
                        case LootEffect.Default:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case RoomEffect.Default:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private void AssignPlayerInfo()
    {
        if (warriorCard != null)
        {
            WarriorInfo.BaseMaxHp = warriorCard.maxHp;
            WarriorInfo.MaxHp = warriorCard.maxHp;
            WarriorInfo.CurrentHp = warriorCard.maxHp;
            WarriorInfo.Init = warriorCard.initiative;
            WarriorInfo.Movement = warriorCard.movement;
            WarriorInfo.ActiveOne = warriorCard.activeOne;
            WarriorInfo.ActiveTwo = warriorCard.activeTwo;
            WarriorInfo.Passive = warriorCard.passive;
            WarriorInfo.Consumable = warriorCard.consumable;
        }
        if (thiefCard != null)
        {
            ThiefInfo.BaseMaxHp = thiefCard.maxHp;
            ThiefInfo.MaxHp = thiefCard.maxHp;
            ThiefInfo.CurrentHp = thiefCard.maxHp;
            ThiefInfo.Init = thiefCard.initiative;
            ThiefInfo.Movement = thiefCard.movement;
            ThiefInfo.ActiveOne = thiefCard.activeOne;
            ThiefInfo.ActiveTwo = thiefCard.activeTwo;
            ThiefInfo.Passive = thiefCard.passive;
            ThiefInfo.Consumable = thiefCard.consumable;
        }
        if (clericCard != null)
        {
            ClericInfo.BaseMaxHp = clericCard.maxHp;
            ClericInfo.MaxHp = clericCard.maxHp;
            ClericInfo.CurrentHp = clericCard.maxHp;
            ClericInfo.Init = clericCard.initiative;
            ClericInfo.Movement = clericCard.movement;
            ClericInfo.ActiveOne = clericCard.activeOne;
            ClericInfo.ActiveTwo = clericCard.activeTwo;
            ClericInfo.Passive = clericCard.passive;
            ClericInfo.Consumable = clericCard.consumable;
        }

        if (wizardCard != null)
        {
            WizardInfo.BaseMaxHp = wizardCard.maxHp;
            WizardInfo.MaxHp = wizardCard.maxHp;
            WizardInfo.CurrentHp = wizardCard.maxHp;
            WizardInfo.Init = wizardCard.initiative;
            WizardInfo.Movement = wizardCard.movement;
            WizardInfo.ActiveOne = wizardCard.activeOne;
            WizardInfo.ActiveTwo = wizardCard.activeTwo;
            WizardInfo.Passive = wizardCard.passive;
            WizardInfo.Consumable = wizardCard.consumable;
        }
    }

    private void LaunchRoomType(RoomType roomType)
    {
        switch(roomType)
        {
            case RoomType.Normal:
                DungeonUiManager.DisplayLootActionSelectorUI();
                break;
            case RoomType.Boss:
                LaunchRoomEffect(RoomEffect.Boss);
                break;
            case RoomType.Treasure:
                DungeonUiManager.DisplayLootActionSelectorUI();
                break;
            case RoomType.Fighting:
                LaunchRoomEffect(RoomEffect.Fight);
                break;
            case RoomType.FirstRoom:
                DungeonUiManager.DisplayLootActionSelectorUI();
                break;
            case RoomType.Starting:
                DungeonUiManager.ResetArtworkUi();
                DungeonUiManager.ResetLootActionSelectorUI();
                break;
            default:
                throw new ArgumentOutOfRangeException(null);
        }
    }
    
    private void LaunchRoomEffect(RoomEffect roomEffect)
    {
        DungeonUiManager.ResetLootActionSelectorUI();
        CurrentTile.emptied = true;
        _roomEffect = roomEffect;
        switch(roomEffect)
        {
            case RoomEffect.Boss:
                artworkShown = true;
                DungeonUiManager.BossUi();
                break;
            case RoomEffect.Treasure:
                LaunchTreasure();
                break;
            case RoomEffect.Fight:
                artworkShown = true;
                DungeonUiManager.FightUi();
                break;
            case RoomEffect.Rest:
                artworkShown = true;
                DungeonUiManager.RestUi();
                break;
            case RoomEffect.Loot:
                LaunchLoot();
                break;
            case RoomEffect.EndFightLoot:
                LaunchFightLoot();
                break;
            case RoomEffect.Default:
            default:
                throw new ArgumentOutOfRangeException(nameof(roomEffect), roomEffect, null);
        }
    }

    private void LaunchFight()
    {
        LaunchFightFX();
        string scene = SelectFightScene();
        
        Debug.Log(scene);
        fightingSceneList.Remove(scene);
        _sceneContainer.SetActive(false);
        AsyncOperation op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        op.completed += operation =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
        };
    }
    
    private void LaunchAmbush()
    {
        LaunchFightFX();
        string scene = SelectAmbushedScene();
        
        Debug.Log(scene);
        fightingSceneList.Remove(scene);
        Debug.Log(fightingSceneList.Count);
        _sceneContainer.SetActive(false);
        AsyncOperation op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        op.completed += operation =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
        };
    }
    
    private void LaunchBoss()
    {
        LaunchFightFX();
        SceneManager.LoadSceneAsync(bossScene);
    }
    
    private void LaunchLoot()
    {
        artworkShown = true;
        int rand = Random.Range(0, 20);
        if(rand == 0)
        {
            DungeonUiManager.LootAmbushedUi();
            lootEffect = LootEffect.Ambush;
            NegativeLootFX();
        }
        else if (rand is > 0 and <= 3)
        {
            DungeonUiManager.LootTrapUi();
            lootEffect = LootEffect.Trap;
            NegativeLootFX();
        }
        else if(rand is > 3 and <= 7)
        {
            DungeonUiManager.LootStuffUi();
            lootEffect = LootEffect.Stuff;
            PositiveLootFX();
        }
        else if (rand is > 7 and <= 13)
        {
            DungeonUiManager.LootConsumableUi();
            lootEffect = LootEffect.Consumable;
            PositiveLootFX();
        }
        else
        {
            DungeonUiManager.LootNothingUi();
            lootEffect = LootEffect.Nothing;
        }
    }
    
    private void LaunchRest()
    {
        WarriorInfo.CurrentHp += Random.Range(1, 7);
        ThiefInfo.CurrentHp += Random.Range(1, 7);
        ClericInfo.CurrentHp += Random.Range(1, 7);
        WizardInfo.CurrentHp += Random.Range(1, 7);
        
        RestFX();

        int res = Random.Range(0, RestAmbushValue+1);

        if(res == 0)
        {
            LaunchAmbush();
        }
        else
        {
            RestAmbushValue--;
        }
    }
    
    private void LaunchTreasure()
    {
        artworkShown = true;
        Stuff newStuff;
        int rand = Random.Range(0, 5);
        PositiveLootFX();
        if(rand == 0)
        {
            DungeonUiManager.TreasureConsumableUi();
            newStuff = PickConsumable();
            treasureEffect = TreasureEffect.Consumable;
        }
        else
        {
            DungeonUiManager.TreasureStuffUi();
            newStuff = PickStuff();
            treasureEffect = TreasureEffect.Stuff;
        }
    }
    
    private void LaunchFightLoot()
    {
        artworkShown = true;
        Stuff newStuff;
        int rand = Random.Range(0, 5);
        PositiveLootFX();
        if(rand <= 2)
        {
            DungeonUiManager.TreasureConsumableUi();
            newStuff = PickConsumable();
            treasureEffect = TreasureEffect.Consumable;
        }
        else
        {
            DungeonUiManager.TreasureStuffUi();
            newStuff = PickStuff();
            treasureEffect = TreasureEffect.Stuff;
        }
    }
    
    private void LaunchTrap()
    {
        int comp = WarriorInfo.CurrentHp;
        int minus = Random.Range(trapValue.Min, trapValue.Max+1);
        if (comp - minus > 0)
        {
            WarriorInfo.CurrentHp -= minus;
        }
        else
            WarriorInfo.CurrentHp = 1;
        
        comp = ThiefInfo.CurrentHp;
        minus = Random.Range(trapValue.Min, trapValue.Max+1);
        if (comp - minus > 0)
        {
            ThiefInfo.CurrentHp -= minus;
        }
        else
            ThiefInfo.CurrentHp = 1;
        
        comp = ClericInfo.CurrentHp;
        minus = Random.Range(trapValue.Min, trapValue.Max+1);
        if (comp - minus > 0)
        {
            ClericInfo.CurrentHp -= minus;
        }
        else
            ClericInfo.CurrentHp = 1;
        
        comp = WizardInfo.CurrentHp;
        minus = Random.Range(trapValue.Min, trapValue.Max+1);
        if (comp - minus > 0)
        {
            WizardInfo.CurrentHp -= minus;
        }
        else
            WizardInfo.CurrentHp = 1;
    }

    private string SelectFightScene()
    {
        int rand = Random.Range(0, fightingSceneList.Count);
        string sceneName = fightingSceneList[rand];
        
        return sceneName;
    }
    
    private string SelectAmbushedScene()
    {
        int rand = Random.Range(0, ambushedSceneList.Count);
        string sceneName = ambushedSceneList[rand];
        
        return sceneName;
    }

    private Stuff PickStuff()
    {
        int ind = Random.Range(0, stuffList.Count);
        return stuffList[ind];
    }
    
    private Consumable PickConsumable()
    {
        int ind = Random.Range(0, consumableList.Count);
        return consumableList[ind];
    }

    private void StuffSelection()
    {
        Stuff stuff = PickStuff();
        DungeonUiManager.StuffChoiceAction(stuff);
    }
    
    private void ConsumableSelection()
    {
        Stuff stuff = PickConsumable();
        DungeonUiManager.StuffChoiceAction(stuff);
    }

    public static string GetDungeonSceneName()
    {
        return _dungeonSceneName;
    }

    public void LaunchLootButton()
    {
        LaunchRoomEffectAction(CurrentTile.GetComponent<DungeonTile>().roomType == RoomType.Normal
            ? RoomEffect.Loot
            : RoomEffect.Treasure);
    }

    public void LaunchRestButton()
    {
        LaunchRoomEffectAction(RoomEffect.Rest);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadSceneAsync("_MainScenes/MainMenu");
    }

    private void PositiveLootFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("PositiveLoot");
    }
    
    private void RestFX()
    {
        FindObjectOfType<AudioManager>().OneShot("Rest");
    }

    private void NegativeLootFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("NegativeLoot");
    }

    private void StepFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("StepDungeon");
    }

    private void LaunchFightFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("LaunchFight");
    }
    
    /*--------------------------------------------------TEST------------------------------------------*/
    
    [ButtonMethod()]
    [ContextMenu("Trap")]
    public void TestTrapUnits()
    {
        LaunchTrap();
    }
}
