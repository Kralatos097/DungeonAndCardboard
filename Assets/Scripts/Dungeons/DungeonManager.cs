using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DungeonManager : MonoBehaviour
{
    //Action
    public static Action<RoomEffect> LaunchRoomEffectAction;
    public static Action<RoomType> LaunchRoomTypeAction;

    private static string _dungeonSceneName;
    
    private GameObject gameContainer;
    public static DungeonTile currentTile;
    
    private  string bossScene;
    
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
    
    private void Awake()
    {
        _dungeonSceneName = SceneManager.GetActiveScene().name;
    }
    
    void Start()
    {
        LaunchRoomEffectAction = LaunchRoomEffect;
        LaunchRoomTypeAction = LaunchRoomType;
        
        gameContainer = GameObject.Find("GameContainer");
        
        AssignPlayerInfo();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && artworkShown)
        {
            DungeonUiManager.ResetArtworkUi();
            artworkShown = false;
            switch(_roomEffect)
            {
                case RoomEffect.Boss:
                    LaunchBoss();
                    break;
                case RoomEffect.Fight:
                    LaunchFight();
                    break;
                case RoomEffect.Rest:
                    LaunchRest();
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
                case RoomEffect.Loot:
                    switch (lootEffect)
                    {
                        case LootEffect.Trap:
                            LaunchTrap();
                            break;
                        case LootEffect.Ambush:
                            LaunchAmbush();
                            break;
                        case LootEffect.Stuff:
                            StuffSelection();
                            break;
                        case LootEffect.Consumable:
                            ConsumableSelection();
                            break;
                        case LootEffect.Nothing:
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
            //todo: autoriser movement
        }
    }
    
    private void AssignPlayerInfo()
    {
        if (warriorCard != null)
        {
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
                //todo: loot ui
                DungeonUiManager.DisplayLootActionSelectorUI();
                break;
            case RoomType.Boss:
                LaunchRoomEffect(RoomEffect.Boss);
                break;
            case RoomType.Treasure:
                //todo: loot ui
                break;
            case RoomType.Fighting:
                LaunchRoomEffect(RoomEffect.Fight);
                break;
            case RoomType.FirstRoom:
                LaunchRoomEffect(RoomEffect.Treasure);
                break;
            case RoomType.Starting:
                //DungeonUiManager.ResetArtworkUi();
                break;
            default:
                throw new ArgumentOutOfRangeException(null);
        }
    }
    
    private void LaunchRoomEffect(RoomEffect roomEffect)
    {
        currentTile.emptied = true;
        _roomEffect = roomEffect;
        switch(roomEffect)
        {
            case RoomEffect.Boss:
                DungeonUiManager.BossUi();
                break;
            case RoomEffect.Treasure:
                LaunchTreasure();
                break;
            case RoomEffect.Fight:
                DungeonUiManager.FightUi();
                break;
            case RoomEffect.Rest:
                DungeonUiManager.RestUi();
                break;
            case RoomEffect.Loot:
                LaunchLoot();
                break;
            case RoomEffect.Default:
            default:
                throw new ArgumentOutOfRangeException(nameof(roomEffect), roomEffect, null);
        }
    }

    private void LaunchFight()
    {
        string scene = SelectFightScene();
        
        Debug.Log(scene);
        fightingSceneList.Remove(scene);
        Debug.Log(fightingSceneList.Count);
        gameContainer.SetActive(false);
        AsyncOperation op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        op.completed += operation =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
        };
    }
    
    private void LaunchAmbush()
    {
        string scene = SelectAmbushedScene();
        
        Debug.Log(scene);
        fightingSceneList.Remove(scene);
        Debug.Log(fightingSceneList.Count);
        gameContainer.SetActive(false);
        AsyncOperation op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        op.completed += operation =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
        };
    }
    
    private void LaunchBoss()
    {
        SceneManager.LoadSceneAsync(bossScene);
    }
    
    private void LaunchLoot()
    {
        artworkShown = true;
        int rand = Random.Range(0, 20);
        Debug.Log(rand);
        if(rand == 0)
        {
            DungeonUiManager.LootAmbushedUi();
            lootEffect = LootEffect.Ambush;
        }
        else if (rand is > 0 and <= 3)
        {
            
            DungeonUiManager.LootTrapUi();
            lootEffect = LootEffect.Trap;
        }
        else if(rand is > 3 and <= 7)
        {
            DungeonUiManager.LootStuffUi();
            lootEffect = LootEffect.Stuff;
        }
        else if (rand is > 7 and <= 13)
        {
            DungeonUiManager.LootConsumableUi();
            lootEffect = LootEffect.Consumable;
        }
        else
        {
            DungeonUiManager.LootNothingUi();
            lootEffect = LootEffect.Nothing;
        }
    }
    
    private void LaunchRest()
    {
        artworkShown = true;
        
        WarriorInfo.CurrentHp += Random.Range(1, 7);
        ThiefInfo.CurrentHp += Random.Range(1, 7);
        ClericInfo.CurrentHp += Random.Range(1, 7);
        WizardInfo.CurrentHp += Random.Range(1, 7);
        
        Debug.Log(WarriorInfo.CurrentHp);
        Debug.Log(ThiefInfo.CurrentHp);
        Debug.Log(ClericInfo.CurrentHp);
        Debug.Log(WizardInfo.CurrentHp);
    }
    
    private void LaunchTreasure()
    {
        artworkShown = true;
        Stuff newStuff;
        int rand = Random.Range(0, 5);
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
        DungeonUiManager.StuffChoice(newStuff);
    }
    
    private void LaunchTrap()
    {
        int comp = WarriorInfo.CurrentHp;
        int minus = Random.Range(1, 4);
        if (comp - minus > 0)
        {
            WarriorInfo.CurrentHp -= minus;
        }
        else
            WarriorInfo.CurrentHp = 1;
        
        comp = ThiefInfo.CurrentHp;
        minus = Random.Range(1, 4);
        if (comp - minus > 0)
        {
            ThiefInfo.CurrentHp -= minus;
        }
        else
            ThiefInfo.CurrentHp = 1;
        
        comp = ClericInfo.CurrentHp;
        minus = Random.Range(1, 4);
        if (comp - minus > 0)
        {
            ClericInfo.CurrentHp -= minus;
        }
        else
            ClericInfo.CurrentHp = 1;
        
        comp = WizardInfo.CurrentHp;
        minus = Random.Range(1, 4);
        if (comp - minus > 0)
        {
            WizardInfo.CurrentHp -= minus;
        }
        else
            WizardInfo.CurrentHp = 1;
        
        Debug.Log(WarriorInfo.CurrentHp);
        Debug.Log(ThiefInfo.CurrentHp);
        Debug.Log(ClericInfo.CurrentHp);
        Debug.Log(WizardInfo.CurrentHp);
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
        string sceneName = fightingSceneList[rand];
        
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
        DungeonUiManager.StuffChoice(stuff);
    }
    
    private void ConsumableSelection()
    {
        Stuff stuff = PickConsumable();
        DungeonUiManager.StuffChoice(stuff);
    }

    public static string GetDungeonSceneName()
    {
        return _dungeonSceneName;
    }
}
