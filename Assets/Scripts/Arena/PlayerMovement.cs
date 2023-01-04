using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : TacticsMovement
{
    private UIManager _uiManager;
    private bool pass = false;
    private Vector3 _lastPos;
    private Quaternion _lastRot;

    public Perso charaClass;

    [SerializeField] protected PlayerBaseInfo UnitInfo;

    // Start is called before the first frame update
    void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
        Init();
        TurnManager.AddPlayerToList(this);
        _lastPos = transform.position;
        _lastRot = transform.rotation;
    }

    protected override void GetUnitInfo()
    {
        switch (charaClass)
        {
            case Perso.Warrior:
                CombatStat.MaxHp = WarriorInfo.MaxHp;
                CombatStat.CurrHp = WarriorInfo.CurrentHp;
                CombatStat.ChangeInit(WarriorInfo.Init);

                baseMove = WarriorInfo.Movement;
                ActiveOne = WarriorInfo.ActiveOne;
                ActiveTwo = WarriorInfo.ActiveTwo;
                Passive = WarriorInfo.Passive;
                Consumable = WarriorInfo.Consumable;
                break;
            case Perso.Thief:
                CombatStat.MaxHp = ThiefInfo.MaxHp;
                CombatStat.CurrHp = ThiefInfo.CurrentHp;
                CombatStat.ChangeInit(ThiefInfo.Init);

                baseMove = ThiefInfo.Movement;
                ActiveOne = ThiefInfo.ActiveOne;
                ActiveTwo = ThiefInfo.ActiveTwo;
                Passive = ThiefInfo.Passive;
                Consumable = ThiefInfo.Consumable;
                break;
            case Perso.Cleric:
                CombatStat.MaxHp = ClericInfo.MaxHp;
                CombatStat.CurrHp = ClericInfo.CurrentHp;
                CombatStat.ChangeInit(ClericInfo.Init);

                baseMove = ClericInfo.Movement;
                ActiveOne = ClericInfo.ActiveOne;
                ActiveTwo = ClericInfo.ActiveTwo;
                Passive = ClericInfo.Passive;
                Consumable = ClericInfo.Consumable;
                break;
            case Perso.Wizard:
                CombatStat.MaxHp = WizardInfo.MaxHp;
                CombatStat.CurrHp = WizardInfo.CurrentHp;
                CombatStat.ChangeInit(WizardInfo.Init);

                baseMove = WizardInfo.Movement;
                ActiveOne = WizardInfo.ActiveOne;
                ActiveTwo = WizardInfo.ActiveTwo;
                Passive = WizardInfo.Passive;
                Consumable = WizardInfo.Consumable;
                break;
            case Perso.Default:
                CombatStat.MaxHp = UnitInfo.maxHp;
                CombatStat.CurrHp = UnitInfo.maxHp;
                CombatStat.ChangeInit(UnitInfo.initiative);

                baseMove = UnitInfo.movement;
                ActiveOne = UnitInfo.activeOne;
                ActiveTwo = UnitInfo.activeTwo;
                Passive = UnitInfo.passive;
                Consumable = UnitInfo.consumable;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetUnitInfo()
    {
        switch (charaClass)
        {
            case Perso.Warrior:
                WarriorInfo.MaxHp = CombatStat.MaxHp;
                WarriorInfo.CurrentHp = CombatStat.CurrHp;
                WarriorInfo.Init = CombatStat.GetInit();

                WarriorInfo.Consumable = Consumable;
                break;
            case Perso.Thief:
                ThiefInfo.MaxHp = CombatStat.MaxHp;
                ThiefInfo.CurrentHp = CombatStat.CurrHp;
                ThiefInfo.Init = CombatStat.GetInit();

                ThiefInfo.Consumable = Consumable;
                break;
            case Perso.Cleric:
                ClericInfo.MaxHp = CombatStat.MaxHp;
                ClericInfo.CurrentHp = CombatStat.CurrHp;
                ClericInfo.Init = CombatStat.GetInit();

                ClericInfo.Consumable = Consumable;
                break;
            case Perso.Wizard:
                WizardInfo.MaxHp = CombatStat.MaxHp;
                WizardInfo.CurrentHp = CombatStat.CurrHp;
                WizardInfo.Init = CombatStat.GetInit();

                WizardInfo.Consumable = Consumable;
                break;
            case Perso.Default:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Turn) return;
        PlayersTurn = true;

        //display action Selector on turn start
        if (!pass)
        {
            _uiManager.ShowActionSelector();
            pass = true;
        }

        switch (_uiManager.actionSelected)
        {
            case Action.Attack:
                _uiManager.HideActionSelector();
                _uiManager.SetStuff(ActiveOne, ActiveTwo, Consumable);
                _uiManager.SetCd(ActiveOneCd, ActiveTwoCd);
                _uiManager.ShowEquipSelector();
                _uiManager.actionSelected = Action.Equip;
                break;
            case Action.Move:
                _uiManager.HideActionSelector();

                if (!moving)
                {
                    //Début du Soulevement du pion lors du mouvement
                    if (!passM)
                    {
                        transform.GetChild(0).Translate(0, MoveY, 0);
                        passM = true;
                    }

                    FindSelectableTile();
                    CheckMove();
                }
                else
                {
                    Move();
                    _uiManager.alreadyMoved = true;
                }

                break;
            case Action.Stay:
                _lastPos = transform.position;
                _lastRot = transform.rotation;
                TurnManager.EndTurnD();
                pass = false;
                _uiManager.Reset();
                break;
            case Action.Default:
                RemoveSelectableTile();
                if (passM)
                {
                    //Fin du Soulevement du pion lors du mouvement
                    transform.GetChild(0).Translate(0, -MoveY, 0);
                    passM = false;
                }

                break;
            case Action.CancelMove:
                transform.position = _lastPos;
                transform.rotation = _lastRot;

                _uiManager.alreadyMoved = false;
                _uiManager.actionSelected = Action.Default;
                _uiManager.ShowActionSelector();
                break;
            case Action.Equip:
                _uiManager.HideActionSelector();
                switch (_uiManager.stuffSelected)
                {
                    case StuffSelected.EquipOne:
                        atkRange = ActiveOne.GetAtkRange();
                        break;
                    case StuffSelected.EquipTwo:
                        atkRange = ActiveTwo.GetAtkRange();
                        break;
                    case StuffSelected.Consum:
                        atkRange = Consumable.GetAtkRange();
                        break;
                    case StuffSelected.Default:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (_uiManager.stuffSelected != StuffSelected.Default)
                {
                    _uiManager.HideEquipSelector();
                    AffAttackRange();
                    CheckAttack();
                }

                break;
            default:
                break;
        }
    }

    private void CheckMove()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay((Input.mousePosition));

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Tile"))
                {
                    ArenaTile t = hit.collider.GetComponent<ArenaTile>();

                    if (t.selectable)
                    {
                        MoveToTile(t);
                    }
                }
            }
        }
    }

    private void CheckAttack()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay((Input.mousePosition));
            bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && !isOverUI)
            {
                ArenaTile t = null;
                if (hit.collider.CompareTag("Tile"))
                {
                    t = hit.collider.GetComponent<ArenaTile>();
                }
                else if (hit.collider.gameObject.GetComponent<TacticsMovement>() != null)
                {
                    t = hit.collider.GetComponent<TacticsMovement>().GetCurrentTile();
                    Debug.Log(t);
                }

                if (t != null)
                {
                    Debug.Log(t.GetGameObjectOnTop());

                    bool passAtk = false;
                    GameObject TargetGO = t.GetGameObjectOnTop();
                    if (TargetGO != null)
                        passAtk = (TargetGO.CompareTag("Enemy") || TargetGO.CompareTag("Player") ||
                                   TargetGO.CompareTag("Crate"));

                    int equip = 0;
                    switch (_uiManager.stuffSelected)
                    {
                        case StuffSelected.EquipOne:
                            equip = 1;
                            break;
                        case StuffSelected.EquipTwo:
                            equip = 2;
                            break;
                        case StuffSelected.Consum:
                            equip = 3;
                            break;
                        case StuffSelected.Default:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    if (t.selectable && passAtk)
                    {
                        TurnToTarget(TargetGO);

                        Attack(TargetGO, equip);
                    }
                    else
                    {
                        //Todo: If attacking an empty tile -> do atk animation and nothing else
                        GameObject dummy = GameObject.Find("DummyPlayer");
                        
                        Debug.Log("ATTACK");
                        TurnToTarget(t.gameObject);
                        Attack(dummy, equip);
                    }
                }
            }
        }
    }

    protected override Active GetSelectedActive()
    {
        Active active;
        switch (_uiManager.stuffSelected)
        {
            case StuffSelected.EquipOne:
                active = ActiveOne;
                break;
            case StuffSelected.EquipTwo:
                active = ActiveTwo;
                break;
            case StuffSelected.Consum:
                active = Consumable;
                break;
            case StuffSelected.Default:
            default:
                active = null;
                throw new ArgumentOutOfRangeException();
        }

        return active;
    }

    protected override void EndOfMovement()
    {
        base.EndOfMovement();
        _uiManager.actionSelected = Action.Default;
        _uiManager.ShowActionSelector();
    }

    protected override void EndOfAttack()
    {
        base.EndOfAttack();
        _lastPos = transform.position;
        _lastRot = transform.rotation;
        _uiManager.Reset();
        TurnManager.EndTurnD();
    }
}