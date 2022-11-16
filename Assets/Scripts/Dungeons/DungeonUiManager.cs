using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DungeonUiManager : MonoBehaviour
{
    private bool inChoice = false;
    
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

    public static RoomArtworkUi DisplayLootActionSelectorUI;
    public static RoomArtworkUi ResetLootActionSelectorUI;
    
    public delegate void StuffChoiceUi(Stuff newStuff);
    public static StuffChoiceUi StuffChoice;
    
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

    private void InChoiceChange()
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
}
