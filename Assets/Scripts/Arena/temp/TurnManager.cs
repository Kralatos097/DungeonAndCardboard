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
    private static bool _combatEnded = false;
    private static bool _isDefeat = false;

    public delegate GameObject TurnManagerDelegate();
    public static TurnManagerDelegate GetCurrentPlayerD;
    
    public delegate void TurnManagerDelegateV();
    private static TurnManagerDelegateV _combatEndPassiveEffectD;
    public static TurnManagerDelegateV EndTurnD;


    private void Awake()
    {
        _unitsList.Clear();
        turnOrder.Clear();
        _playerList.Clear();
    }

    private void Start()
    {
        Invoke(nameof(LateStart), 1);
        _combatEnded = false;
        _combatEndCanvas = CombatEndCanvas;
        GetCurrentPlayerD = GetCurrentPlayer;
        _combatEndPassiveEffectD = OnCombatEndPassiveEffect;
        EndTurnD = EndTurn;
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
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && _combatEnded)
        {
            
            if(!bossFight)
            {
                if(_isDefeat)
                {
                    //todo: loose screen
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(DungeonManager.GetDungeonSceneName()));
                    SceneManager.LoadScene("DefeatScene");
                }
                else
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                    DungeonManager.SceneContainerSwitch(true); 
                    //todo: update life
                    //UiManagerDj.playerInfoUi();
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(DungeonManager.GetDungeonSceneName()));
                }
                
            }
            else
            {
                if(_isDefeat)
                {
                    //todo: loose screen
                    SceneManager.LoadScene("DefeatScene");
                }
                else
                {
                    //todo: Session end
                    SceneManager.LoadScene("VictoryScene");
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

    private static void EndCombat(bool pStatue)
    {
        _combatEnded = true;
        _combatEndPassiveEffectD();
        //Victoire player
        if(pStatue)
        {
            //todo
            Debug.Log("Victiore");
            _combatEndCanvas.GetChild(0).gameObject.SetActive(true);
            SetPlayersInfo();
            //Todo: changement de scene apres un clic
        }
        //Défaite player
        else
        {
            //todo
            Debug.Log("Defeat");
            _isDefeat = true;
            _combatEndCanvas.GetChild(1).gameObject.SetActive(true);
            //Todo: changement de scene apres un clic
        }
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
        if (ArePlayersAlive() && AreEnemysAlive())
        {
            while(!turnOrder.Peek().GetComponent<CombatStat>().isUp)
            {
                if (turnOrder.Peek().CompareTag("Player"))
                {
                    TacticsMovement deadUnit = turnOrder.Dequeue();
                    turnOrder.Enqueue(deadUnit);
                }
                else
                {
                    TacticsMovement deadUnit = turnOrder.Dequeue();
                    Destroy(deadUnit.gameObject);
                }
            }
            Debug.Log("Turn of : " + turnOrder.Peek().name);

            if(turnOrder.Peek().GetComponent<CombatStat>().StatusEffect == StatusEffect.Poison)
            {
                turnOrder.Peek().GetComponent<CombatStat>().Poison();
                if(!turnOrder.Peek().GetComponent<CombatStat>().isUp)
                {
                    TacticsMovement DeadUnit = turnOrder.Dequeue();
                    Destroy(DeadUnit.gameObject);
                    if(!ArePlayersAlive() && !AreEnemysAlive())
                    {
                        EndCombat(ArePlayersAlive());
                    }
                }
            }
            else if(turnOrder.Peek().GetComponent<CombatStat>().StatusEffect == StatusEffect.Freeze)
            {
                turnOrder.Peek().GetComponent<TacticsMovement>().ChangeMove(-turnOrder.Peek().GetComponent<CombatStat>().StatusValue);
            }
            else
            {
                turnOrder.Peek().GetComponent<TacticsMovement>().ChangeMove(0);
            }

            OnTurnStartPassiveEffect(turnOrder.Peek());
            turnOrder.Peek().BeginTurn();
        }
        else
        {
            EndCombat(ArePlayersAlive());
        }
    }

    private void EndTurn()
    {
        TacticsMovement unit = turnOrder.Dequeue();

        if(unit.GetComponent<CombatStat>().StatusEffect == StatusEffect.Burn)
        {
            unit.GetComponent<CombatStat>().Burn();
            if(!unit.GetComponent<CombatStat>().isUp)
            {
                TacticsMovement.PlayersTurn = false;
                StartTurn();
            }
        }
        else if (unit.GetComponent<CombatStat>().StatusEffect == StatusEffect.Freeze)
        {
            unit.GetComponent<CombatStat>().ResetStatus();
        }

        unit.EndTurn();
        OnTurnEndPassiveEffect(turnOrder.Peek());
        unit.EquipCDMinus(1);
        turnOrder.Enqueue(unit);
        TacticsMovement.PlayersTurn = false;
        StartTurn();
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
                    int newBaseInit = unit.gameObject.GetComponent<CombatStat>().initiative;
                    int retBaseInit = unitRet.gameObject.GetComponent<CombatStat>().initiative;

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
    
    public static void AddPlayerToList(PlayerMovement unit)
    {
        _playerList.Add(unit);
    }
    
    //todo: remove of the list

    private static bool ArePlayersAlive()
    {
        foreach (TacticsMovement unit in turnOrder)
        {
            /*if (unit.gameObject.CompareTag("Player") && !unit.gameObject.GetComponent<CombatStat>().isAlive)
            {
                turnOrder.Dequeue();
            }*/
            if(unit.gameObject.CompareTag("Player") && unit.gameObject.GetComponent<CombatStat>().isUp)
            {
                return true;
            }
        }

        return false;
    }
    
    private static bool AreEnemysAlive()
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

    private GameObject GetCurrentPlayer()
    {
        return turnOrder.Peek().gameObject;
    }

    private void OnCombatStartPassiveEffect()
    {
        foreach (var tacticsMovement in turnOrder)
        {
            if (tacticsMovement.GetPassive().GetPassiveTrigger() == PassiveTrigger.OnCombatStart)
            {
                tacticsMovement.GetPassive().Effect(tacticsMovement.gameObject);
            }
        }
    }
    
    private void OnCombatEndPassiveEffect()
    {
        foreach (var tacticsMovement in turnOrder)
        {
            if (tacticsMovement.GetPassive().GetPassiveTrigger() == PassiveTrigger.OnCombatEnd)
            {
                tacticsMovement.GetPassive().Effect(tacticsMovement.gameObject);
            }
        }
    }
    
    private void OnTurnStartPassiveEffect(TacticsMovement user)
    {
        if (user.GetPassive().GetPassiveTrigger() == PassiveTrigger.OnTurnStart)
        {
            user.GetPassive().Effect(user.gameObject);
        }
    }
    
    private void OnTurnEndPassiveEffect(TacticsMovement user)
    {
        if (user.GetPassive().GetPassiveTrigger() == PassiveTrigger.OnTurnEnd)
        {
            user.GetPassive().Effect(user.gameObject);
        }
    }
}
