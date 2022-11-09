using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DungeonManager : MonoBehaviour
{
    private static string _dungeonSceneName;
    
    private static GameObject GameContainer;
    private static DungeonTile CurrentTile;
    
    public string bossScene;
    private static string _bossScene;
    
    [Header("Players Card")]
    [SerializeField] private PlayerBaseInfo warriorCard;
    [SerializeField] private PlayerBaseInfo thiefCard;
    [SerializeField] private PlayerBaseInfo clericCard;
    [SerializeField] private PlayerBaseInfo wizardCard;
    
    [Header("Lists")]
    [SerializeField] private List<string> fightingSceneList;
    private static List<string> _fightingSceneList;
    
    [SerializeField] private List<Stuff> stuffList;
    private static List<Stuff> _stuffList;
    
    [SerializeField] private List<Consumable> consumableList;
    private static List<Consumable> _consumableList;
    
    private void Awake()
    {
        _dungeonSceneName = SceneManager.GetActiveScene().name;
    }
    
    void Start()
    {
        _fightingSceneList = fightingSceneList;
        _stuffList = stuffList;
        _consumableList = consumableList;
        _bossScene = bossScene;
        GameContainer = GameObject.Find("GameContainer");
        
        AssignPlayerInfo();
    }
    
    void Update()
    {
        
    }
    
    private void AssignPlayerInfo()
    {
        WarriorInfo.MaxHp = warriorCard.maxHp;
        WarriorInfo.CurrentHp = warriorCard.maxHp;
        WarriorInfo.Init = warriorCard.initiative;
        WarriorInfo.Movement = warriorCard.movement;
        WarriorInfo.ActiveOne = warriorCard.activeOne;
        WarriorInfo.ActiveTwo = warriorCard.activeTwo;
        WarriorInfo.Passive = warriorCard.passive;
        WarriorInfo.Consumable = warriorCard.consumable;
        
        ThiefInfo.MaxHp = thiefCard.maxHp;
        ThiefInfo.CurrentHp = thiefCard.maxHp;
        ThiefInfo.Init = thiefCard.initiative;
        ThiefInfo.Movement = thiefCard.movement;
        ThiefInfo.ActiveOne = thiefCard.activeOne;
        ThiefInfo.ActiveTwo = thiefCard.activeTwo;
        ThiefInfo.Passive = thiefCard.passive;
        ThiefInfo.Consumable = thiefCard.consumable;
        
        ClericInfo.MaxHp = clericCard.maxHp;
        ClericInfo.CurrentHp = clericCard.maxHp;
        ClericInfo.Init = clericCard.initiative;
        ClericInfo.Movement = clericCard.movement;
        ClericInfo.ActiveOne = clericCard.activeOne;
        ClericInfo.ActiveTwo = clericCard.activeTwo;
        ClericInfo.Passive = clericCard.passive;
        ClericInfo.Consumable = clericCard.consumable;
        
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
