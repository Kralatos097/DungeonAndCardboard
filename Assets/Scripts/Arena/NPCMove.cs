using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCMove : TacticsMovement
{
    private GameObject target;
    private int targetDistance = 0;
    private GameObject CrateOnPath = null;
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
        if(temp != null && !moving)
        {
            //Debug.Log("Ennemi Atk !");
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

                FindLowestHpTarget();
                //FindFarthestTarget();
                //FindNearestTarget();
                Debug.Log(target.name);
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
                CalculatePathFull();
                FindSelectableTile();
                ActualTargetTile.target = true;
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

    private void FindNearestTargetDist()
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
    
    private void FindNearestTarget()
    {
        ComputeAdjacencyListAtk();
        SetCurrentTile();

        Queue<ArenaTile> process = new Queue<ArenaTile>();
        
        process.Enqueue(_currentTile);
        _currentTile.visited = true;

        while (process.Count > 0)
        {
            ArenaTile t = process.Dequeue();
            
            _selectableTiles.Add(t);

            foreach (ArenaTile tile in t.adjacencyList)
            {
                if (!tile.visited)
                {
                    tile.parent = t;
                    tile.visited = true;
                    tile.distance = 1 + t.distance;
                    process.Enqueue(tile);

                    GameObject TGO = tile.GetGameObjectOnTop();
                    if (TGO != null)
                    {
                        if (TGO.CompareTag("Player"))
                        {
                            target = TGO;
                            targetDistance = tile.distance;
                            return;
                        }
                    }
                }
            }
        }
    }
    
    private void FindFarthestTarget() //A revoir
    {
        ComputeAdjacencyListAtk();
        SetCurrentTile();

        Queue<ArenaTile> process = new Queue<ArenaTile>();
        
        process.Enqueue(_currentTile);
        _currentTile.visited = true;

        while (process.Count > 0)
        {
            ArenaTile t = process.Dequeue();
            
            _selectableTiles.Add(t);

            foreach (ArenaTile tile in t.adjacencyList)
            {
                if (!tile.visited)
                {
                    tile.parent = t;
                    tile.visited = true;
                    tile.distance = 1 + t.distance;
                    process.Enqueue(tile);

                    GameObject TGO = tile.GetGameObjectOnTop();
                    if (TGO != null)
                    {
                        if (TGO.CompareTag("Player") && tile.distance > targetDistance)
                        {
                            target = TGO;
                            targetDistance = tile.distance;
                        }
                    }
                }
            }
        }
    }
    
    private void FindLowestHpTarget()
    {
        ComputeAdjacencyListAtk();
        SetCurrentTile();

        Queue<ArenaTile> process = new Queue<ArenaTile>();
        
        process.Enqueue(_currentTile);
        _currentTile.visited = true;

        while (process.Count > 0)
        {
            ArenaTile t = process.Dequeue();
            
            _selectableTiles.Add(t);

            foreach (ArenaTile tile in t.adjacencyList)
            {
                if (!tile.visited)
                {
                    tile.parent = t;
                    tile.visited = true;
                    tile.distance = 1 + t.distance;
                    process.Enqueue(tile);

                    GameObject TGO = tile.GetGameObjectOnTop();
                    if (TGO != null)
                    {
                        if (TGO.CompareTag("Player"))
                        {
                            int targetHp = 1000;
                            if(target != null)
                                targetHp = target.GetComponent<CombatStat>().CurrHp;
                            int TGOHp = TGO.GetComponent<CombatStat>().CurrHp;
                            if(targetHp > TGOHp)
                            {
                                target = TGO;
                                targetDistance = tile.distance;
                            }
                        }
                    }
                }
            }
        }
    }

    private bool CalculatePathFull()
    {
        ArenaTile targetTile = GetTargetTile(target);
        return FindPathFull(targetTile);
    }
    
    private bool CalculatePathWoTrap()
    {
        ArenaTile targetTile = GetTargetTile(target);
        return FindPathWoTrap(targetTile);
    }
    
    private bool CalculatePathWoCrate()
    {
        ArenaTile targetTile = GetTargetTile(target);
        return FindPathWoCrate(targetTile);
    }
    
    private bool CalculatePathWoAll()
    {
        ArenaTile targetTile = GetTargetTile(target);
        return FindPathWoCrate(targetTile);
    }

    private void DumbAI()
    {
        if(AlliesInAttackRange())
        {
            //attack target
        }
        else
        {
            FindNearestTarget();

            bool findPath = CalculatePathFull(); //Calcul du trajet normal
            int tDist = targetDistance;
            bool findPathV2 = CalculatePathWoTrap(); //Calcul du trajet sans les pieges
            int tDistV2 = targetDistance;

            if (findPath && findPathV2) //Si les 2 on des trajets valides
            {
                if (tDist <= tDistV2) //On compare la longueur des trajets et on prend le plus court
                {
                    CalculatePathFull();
                    MoveToTile(ActualTargetTile);
                }
                else
                {
                    CalculatePathWoTrap();
                    MoveToTile(ActualTargetTile);
                }
            }
            else if (findPath) //Si seul le 1er trajet et valide
            {
                CalculatePathFull();
                MoveToTile(ActualTargetTile);
            }
            else if (findPathV2) //Si seul le 2eme trajet et valide
            {
                CalculatePathWoTrap();
                MoveToTile(ActualTargetTile);
            }
            else //Si les trajets ne sont pas valide
            {
                findPath = CalculatePathWoCrate();
                if (findPath)
                {
                    MoveToTile(ActualTargetTile);
                }
                else
                {
                    //recherche de chemin en ignorant tous les obstacles
                    /*if(findPath)
                     {
                        bouge le plus loins possible
                     }                 
                     else
                     {
                        Fin du tour
                     }
                     */
                }
            }
            if(AlliesInAttackRange())
            {
                //attack target
            }
            else
            {
                //Fin du tour
            }
        }
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
