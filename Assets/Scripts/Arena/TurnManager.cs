using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private Transform CombatEndCanvas;
    private static Transform _combatEndCanvas;
    
    private static Queue<TacticsMovement> turnOrder = new Queue<TacticsMovement>();
    private static readonly List<TacticsMovement> _unitsList = new List<TacticsMovement>();
    public static List<PlayerMovement> _playerList = new List<PlayerMovement>();

    [HideInInspector] public bool startCombat = false;
    public bool bossFight = false;
    public static bool CombatStarted = false;
    private static bool _combatEnded = false;
    private static bool _isDefeat = false;

    public delegate GameObject TurnManagerDelegate();
    public static TurnManagerDelegate GetCurrentPlayerD;
    
    public delegate void TurnManagerDelegateV();
    private static TurnManagerDelegateV _combatEndPassiveEffectD;
    public static TurnManagerDelegateV EndTurnD;
    
    public delegate int TurnManagerDelegateI();
    public static TurnManagerDelegateI GetNbEnemyD;
    
    [SerializeField][Range(0.1f, 3f)] private float endScreenTime;

    private void Awake()
    {
        _unitsList.Clear();
        turnOrder.Clear();
        _playerList.Clear();
        CombatStarted = false;
        _combatEnded = false;
    }

    private void Start()
    {
        FindObjectOfType<AudioManager>().Play(!bossFight ? "Fight" : "Boss");

        Invoke(nameof(LateStart), 1);
        _combatEndCanvas = CombatEndCanvas;
        GetCurrentPlayerD = GetCurrentPlayer;
        _combatEndPassiveEffectD = OnCombatEndPassiveEffect;
        EndTurnD = EndTurn;
        GetNbEnemyD = GetNbEnemy;
    }
    
    private void LateStart()
    {
        StartCombat();
    }

    void StartCombat()
    {
        ListToQueue();
        SetInitsUi();
        OnCombatStartPassiveEffect();
        StartTurn();
        CombatStarted = true;
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0) && _combatEnded)
        {
            if(!bossFight)
            {
                if(_isDefeat)
                {
                    //todo: loose screen
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(DungeonManager.GetDungeonSceneName()));
                    
                    EndGameInfo.IsVictory = !_isDefeat;
                    EndGameInfo.IsTuto = SceneManager.GetActiveScene().name.Contains("Tuto");
                    
                    SceneManager.LoadScene("EndSession");
                }
                else
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                    DungeonManager.SceneContainerSwitch(true); 
                    //UiManagerDj.playerInfoUi();
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(DungeonManager.GetDungeonSceneName()));
                    DungeonManager.LaunchRoomEffectAction(RoomEffect.EndFightLoot);
                }
                
            }
            else
            {
                if(_isDefeat)
                {
                    //todo: loose screen
                    EndGameInfo.IsVictory = !_isDefeat;
                    EndGameInfo.IsTuto = SceneManager.GetActiveScene().name.Contains("Tuto");
                    
                    SceneManager.LoadScene("EndSession");
                }
                else
                {
                    //todo: Session end
                    EndGameInfo.IsVictory = !_isDefeat;
                    EndGameInfo.IsTuto = SceneManager.GetActiveScene().name.Contains("Tuto");
                    
                    SceneManager.LoadScene("EndSession");
                }
            }
        }
    }

    private void SetInitsUi()
    {
        foreach (TacticsMovement unit in turnOrder)
        {
            UIManager.setInitAction(unit.gameObject);
        }
    }

    private void EndCombat(bool pStatue)
    {
        Invoke(nameof(SwitchCombatEnded), 2/*endScreenTime*/);
        _combatEndPassiveEffectD();
        //Victoire player
        if(pStatue)
        {
            _isDefeat = false;
            _combatEndCanvas.GetChild(0).gameObject.SetActive(true);
            SetPlayersInfo();
            VictoryFx();
        }
        //Défaite player
        else
        {
            _isDefeat = true;
            _combatEndCanvas.GetChild(1).gameObject.SetActive(true);
            SetPlayersInfo();
            DefeatFx();
        }
    }

    private void SwitchCombatEnded()
    {
        _combatEnded = true;
    }
    
    private static void SetPlayersInfo()
    {
        foreach (PlayerMovement movement in _playerList)
        {
            PlayerMovement playerMovement = movement.gameObject.GetComponent<PlayerMovement>();
            if(playerMovement != null)
            {
                playerMovement.SetUnitInfo();
            }
        }
    }
    
    void StartTurn()
    {
        if (ArePlayersAlive() && AreEnemyAlive())
        {
            bool a = true, b = true;

            while(a || b)
            {
                a = DestroyDeadUnits();
                b = PassPlayerTurn();
            }

            if (AreEnemyAllFriendly())
            {
                TransformAllFriendlyEnemy();
            }
            
            if(turnOrder.Peek().GetComponent<CombatStat>().GetStatusEffect() == StatusEffect.Poison)
            {
                turnOrder.Peek().GetComponent<CombatStat>().ActivatePoison();
                bool pass = false;
                if(!turnOrder.Peek().GetComponent<CombatStat>().isAlive)
                {
                    pass = DestroyDeadUnits();
                }
                else if(!turnOrder.Peek().GetComponent<CombatStat>().isUp)
                {
                    pass = PassPlayerTurn();
                }

                if (pass)
                {
                    if(!ArePlayersAlive() || !AreEnemyAlive())
                    {
                        EndCombat(ArePlayersAlive());
                    }
                    if (AreEnemyAllFriendly())
                    {
                        TransformAllFriendlyEnemy();
                    }
                }
            }
            else if(turnOrder.Peek().GetComponent<CombatStat>().GetStatusEffect() == StatusEffect.Freeze)
            {
                turnOrder.Peek().GetComponent<TacticsMovement>().ChangeMove(-turnOrder.Peek().GetComponent<CombatStat>().GetStatusValue());
            }
            else
            {
                turnOrder.Peek().GetComponent<TacticsMovement>().ChangeMove(0);
            }

            OnTurnStartPassiveEffect(turnOrder.Peek());
            if (turnOrder.Peek().CompareTag("Player"))
            {
                AllieStartTurnFx();
            }
            else
            {
                EnemyStartTurnFx();
                turnOrder.Peek().GetComponent<NPCMove>().firstTimePass = false;
            }
            
            turnOrder.Peek().BeginTurn();
            UIManager.StartTurnInitUIChangeAction(turnOrder.Peek().gameObject);
        }
        else
        {
            EndCombat(ArePlayersAlive());
        }
    }
    
    private bool DestroyDeadUnits()
    {
        bool ret = false;
        while(!turnOrder.Peek().gameObject.GetComponent<CombatStat>().isAlive)
        {
            TacticsMovement deadUnit = turnOrder.Dequeue();
            if(deadUnit.CompareTag("Player"))
                deadUnit.GetComponent<PlayerMovement>().SetUnitInfo();
            Destroy(deadUnit.gameObject);
            ret = true;
        }
        return ret;
    }
    
    private bool PassPlayerTurn()
    {
        bool ret = false;
        while(!turnOrder.Peek().GetComponent<CombatStat>().isUp && turnOrder.Peek().CompareTag("Player"))
        {
            TacticsMovement deadUnit = turnOrder.Dequeue();
            turnOrder.Enqueue(deadUnit);
            UIManager.EndTurnInitUIChangeAction(deadUnit.gameObject);
            ret = true;
        }
        return ret;
    }

    private void EndTurn()
    {
        if(turnOrder.Count <= 0)
        {
            StartTurn();
        }
        TacticsMovement unit = turnOrder.Dequeue();

        if(unit.GetComponent<CombatStat>().GetStatusEffect() == StatusEffect.Burn)
        {
            unit.GetComponent<CombatStat>().ActivateBurn();
            if(!unit.GetComponent<CombatStat>().isUp)
            {
                TacticsMovement.PlayersTurn = false;
                unit.EndTurn();
                UIManager.EndTurnInitUIChangeAction(unit.gameObject);
                StartTurn();
                return;
            }
        }
        else if (unit.GetComponent<CombatStat>().GetStatusEffect() == StatusEffect.Freeze && unit.GetComponent<CombatStat>().freezeLastTurn != 0)
        {
            unit.GetComponent<CombatStat>().ResetStatus();
            unit.GetComponent<CombatStat>().EndTurnFreeze(unit.GetComponent<CombatStat>().freezeLastTurn);
            unit.GetComponent<CombatStat>().freezeLastTurn = 0;
        }
        else if (unit.GetComponent<CombatStat>().GetStatusEffect() == StatusEffect.Freeze && unit.GetComponent<CombatStat>().freezeLastTurn == 0)
        {
            unit.GetComponent<CombatStat>().ResetStatus();
        }
        else if (unit.GetComponent<CombatStat>().GetStatusEffect() == StatusEffect.Stun)
        {
            unit.GetComponent<CombatStat>().ResetStatus();
        }

        unit.EndTurn();
        UIManager.EndTurnInitUIChangeAction(unit.gameObject);
        OnTurnEndPassiveEffect(turnOrder.Peek());
        unit.EquipCDMinus(1);
        turnOrder.Enqueue(unit);
        if(!TacticsMovement.PlayersTurn)
        {
            Invoke("StartTurn", .75f);
            TacticsMovement.PlayersTurn = false;
        }
        else
        {
            TacticsMovement.PlayersTurn = false;
            StartTurn();
        }
    }

    private static void ListToQueue()
    {
        while (_unitsList.Count > 0)
        {
            int temp = -100;

            TacticsMovement unitRet = null;
            foreach (TacticsMovement unit in _unitsList)
            {
                int init = unit.gameObject.GetComponent<CombatStat>().currInit;

                //si l'init est surerieur échange
                if (temp < init)
                {
                    temp = init;
                    unitRet = unit;
                }

                //si similaire on départage
                if (temp == init)
                {
                    int newBaseInit = unit.gameObject.GetComponent<CombatStat>().GetInit();
                    int retBaseInit = unitRet.gameObject.GetComponent<CombatStat>().GetInit();

                    //si l'init est surerieur échange
                    if (retBaseInit < newBaseInit)
                    {
                        temp = init;
                        unitRet = unit;
                    }
                    //si égale on départage
                    else if (retBaseInit == newBaseInit)
                    {
                        //on prioritise le player
                        if ((unit.gameObject.CompareTag("Player") && !unitRet.gameObject.CompareTag("Player"))
                            || (!unit.gameObject.CompareTag("Player") && unitRet.gameObject.CompareTag("Player")))
                        {
                            temp = init;
                            unitRet = unit;
                        }
                        //Si les 2 parties sont dans la meme équipe on choisie au hasard
                        else if (unit.gameObject.CompareTag("Player") == unitRet.gameObject.CompareTag("Player"))
                        {
                            int t = Random.Range(0, 2);
                            if (t == 0)
                            {
                                temp = init;
                                unitRet = unit;
                            }
                        }
                    }
                }
            }

            _unitsList.Remove(unitRet);
            turnOrder.Enqueue(unitRet);
        }
    }
    
    public static void AddUnit(TacticsMovement unit)
    {
         _unitsList.Add(unit);
    }
    
    public static void AddUnitToQueue(TacticsMovement unit)
    {
        turnOrder.Enqueue(unit);
        UIManager.setInitAction(unit.gameObject);
    }
    
    public static void AddPlayerToList(PlayerMovement unit)
    {
        _playerList.Add(unit);
    }
    
    //todo: remove of the list
    public static void RemovePlayerToList(PlayerMovement unit)
    {
        _playerList.Remove(unit);
    }

    private static bool ArePlayersAlive()
    {
        foreach (TacticsMovement unit in turnOrder)
        {
            if(unit.gameObject.CompareTag("Player") && unit.gameObject.GetComponent<CombatStat>().isUp)
            {
                return true;
            }
        }

        return false;
    }
    
    private static bool AreEnemyAlive()
    {
        foreach (TacticsMovement unit in turnOrder)
        {
            if (unit.gameObject.CompareTag("Enemy") && unit.gameObject.GetComponent<CombatStat>().isUp)
            {
                return true;
            }
        }
        return false;
    }
    
    private static bool AreEnemyAllFriendly()
    {
        foreach (TacticsMovement unit in turnOrder)
        {
            if (unit.gameObject.CompareTag("Enemy") && unit.gameObject.GetComponent<NPCMove>().GetIaType() != IaType.Friendly)
            {
                return false;
            }
        }
        return true;
    }
    
    private static void TransformAllFriendlyEnemy()
    {
        foreach (TacticsMovement unit in turnOrder)
        {
            NPCMove enemy = unit.gameObject.GetComponent<NPCMove>();
            if (enemy != null)
            {
                enemy.FriendlyTransform();
            }
        }
    }

    private GameObject GetCurrentPlayer()
    {
        return turnOrder.Peek().gameObject;
    }

    private int GetNbEnemy()
    {
        int ret = 0;
        foreach (TacticsMovement unit in turnOrder)
        {
            if(unit.GetComponent<NPCMove>() && unit.GetComponent<CombatStat>().isAlive)
            {
                ret++;
            }
        }
        
        return ret;
    }
    
    //----------------------------- Passive Zone -------------------------------------

    private void OnCombatStartPassiveEffect()
    {
        foreach (var tacticsMovement in turnOrder)
        {
            Passive passive = tacticsMovement.GetPassive();
            if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnCombatStart)
            {
                passive.Effect(tacticsMovement.gameObject);
            }
        }
    }
    
    private void OnCombatEndPassiveEffect()
    {
        foreach (var tacticsMovement in turnOrder)
        {
            Passive passive = tacticsMovement.GetPassive();
            if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnCombatEnd)
            {
                passive.Effect(tacticsMovement.gameObject);
            }
        }
    }
    
    private void OnTurnStartPassiveEffect(TacticsMovement user)
    {
        Passive passive = user.GetPassive();
        if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnTurnStart)
        {
            passive.Effect(user.gameObject);
        }
    }
    
    private void OnTurnEndPassiveEffect(TacticsMovement user)
    {
        Passive passive = user.GetPassive();
        if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnTurnEnd)
        {
            passive.Effect(user.gameObject);
        }
    }

    private void OnDestroy()
    {
        _isDefeat = false;
    }

    //---------------------------------- FX Zone ------------------------------------------

    private void AllieStartTurnFx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("AllieStartTurn");
        //Todo: Add Animation
    }
    
    private void EnemyStartTurnFx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("EnemyStartTurn");
        //Todo: Add Animation
    }

    private void VictoryFx()
    {
        if(bossFight)
        {
            FindObjectOfType<AudioManager>().PlayWtFade("FanfareWin");
        }
        else
        {
            FindObjectOfType<AudioManager>().PlayWtFade("ArenaWin");
        }
    }
    
    private void DefeatFx()
    {
        FindObjectOfType<AudioManager>().PlayWtFade("FanfareLose");
    }
}