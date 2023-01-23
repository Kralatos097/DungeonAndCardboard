using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCMove : TacticsMovement
{
    private GameObject target;
    private bool _alreadyMoved = false;
    
    [SerializeField] protected EnemieBaseInfo UnitInfo;

    private IaType _iaType;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    protected override void GetUnitInfo()
    {
        CombatStat combatStat = gameObject.GetComponent<CombatStat>();
        
        combatStat.MaxHp = RandomHp(UnitInfo.maxHp.x, UnitInfo.maxHp.y);
        combatStat.CurrHp = combatStat.MaxHp;
        combatStat.ChangeInit(UnitInfo.initiative);

        baseMove = UnitInfo.movement;
        ActiveOne = UnitInfo.activeOne;
        ActiveTwo = UnitInfo.activeTwo;
        Passive = UnitInfo.passive;
        Consumable = UnitInfo.consumable;

        _iaType = UnitInfo.iaType;
    }

    protected int RandomHp(int min, int max)
    {
        int ret = Random.Range(min, max + 1);
        return ret;
    }

    // Update is called once per frame
    void Update()
    {
        if(!Turn) return;
        PlayersTurn = false;

        atkRange = ActiveOne.GetAtkRange();
        bool canAtk = true;

        GameObject temp = AlliesInAttackRange();
        RemoveSelectableTile();
        if (temp != null && !moving)
        {
            Debug.Log("Ennemi Atk !");
            attacking = true;
            Attack(temp, 1);
            return;
        }
        else
        {
            canAtk = false;
        }
        if(!_alreadyMoved && !attacking)
        {
            if (!moving)
            {
                //Début du Soulevement du pion lors du mouvement
                if (!passM)
                {
                    transform.GetChild(0).Translate(0, MoveY, 0);
                    passM = true;
                }

                FindNearestTarget();
                //todo: virer ligne du dessus et mettre les lignes du dessous quand les IA seront plus avancé
                /*switch(_iaType)
                {
                    case IaType.Dumb:
                        FindNearestTarget();
                        break;
                    case IaType.Coward:
                        break;
                    case IaType.Ruthless:
                        break;
                    case IaType.Perfectionist:
                        break;
                    case IaType.Accurate:
                        break;
                    case IaType.Friendly:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }*/
                CalculatePath();
                FindSelectableTile();
                actualTargetTile.target = true;
            }
            else
            {
                Move();
                canAtk = true;
            }
        }

        if (_alreadyMoved && !canAtk)
        {
            EndTurnT();
        }
    }

    private void FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        GameObject nearest = null;
        float distance = Mathf.Infinity;

        foreach (GameObject obj in targets)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);

            if (d < distance)
            {
                distance = d;
                nearest = obj;
            }
        }
        
        target = nearest; 
    }

    private void CalculatePath()
    {
        ArenaTile targetTile = GetTargetTile(target);
        //Debug.Log(targetTile);
        FindPath(targetTile);
    }

    protected override void EndOfMovement()
    {
        _alreadyMoved = true;
        base.EndOfMovement();
    }
    
    protected override void EndOfAttack()
    {
        base.EndOfAttack();
        EndTurnT();
    }

    protected void EndTurnT()
    {
        TurnManager.EndTurnD();
        attacking = false;
        _alreadyMoved = false;
    }
}
