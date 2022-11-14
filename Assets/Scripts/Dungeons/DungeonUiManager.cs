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
    [SerializeField] private GameObject reposPanel;

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
    
    public delegate void StuffChoiceUi(Stuff newStuff);
    public static StuffChoiceUi StuffChoice;
    
    // Start is called before the first frame update
    void Start()
    {
        LootTrapUi += DisplayTrapArtwork;
        LootTrapUi += InChoiceChange;
    }

    private void DisplayTrapArtwork()
    {
        //todo
    }

    private void InChoiceChange()
    {
        inChoice = !inChoice;
    }

    private void ResetArtwork()
    {
        
    }
}
