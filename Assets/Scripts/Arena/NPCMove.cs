using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class NPCMove : TacticsMovement
{
    private bool _alreadyMoved = false;
    private int tileMoved = 0;
    public bool temp = false;
    private int _tempMove = -1;
    
    [SerializeField] protected EnemieBaseInfo UnitInfo;

    [SerializeField] private IaType iaType; //todo: Apres test enlever le SerializedField
    
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

        move = UnitInfo.movement;
        ActiveOne = UnitInfo.activeOne;
        ActiveTwo = UnitInfo.activeTwo;
        Passive = UnitInfo.passive;
        Consumable = UnitInfo.consumable;

        //iaType = UnitInfo.iaType; todo: après le test enlever de commentaire
    }

    protected int RandomHp(int min, int max)
    {
        int ret = Random.Range(min, max + 1);
        return ret;
    }

    void Update()
    {
        if(!Turn) return;
        PlayersTurn = false;

        atkRange = ActiveOne.GetAtkRange();
        
        switch(iaType)
        {
            case IaType.Dumb:
                DumbAI();
                break;
            case IaType.Coward:
                CowardAI();
                break;
            case IaType.Ruthless:
                RuthlessAI();
                break;
            case IaType.Perfectionist:
                PerfectionistAI();
                break;
            case IaType.Friendly:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /*void UpdateV1()
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
            if(!moving)
            {
                //Début du Soulevement du pion lors du mouvement
                if (!passM)
                {
                    transform.GetChild(0).Translate(0, MoveY, 0);
                    passM = true;
                }

                /*FindLowestHpTarget();
                //FindFarthestTarget();
                //FindNearestTarget();
                Debug.Log(target.name);
                CalculatePathFull();
                FindSelectableTile();
                ActualTargetTile.target = true;
                MoveToTile(ActualTargetTile);#1#
                switch(iaType)
                {
                    case IaType.Dumb:
                        DumbAI();
                        break;
                    case IaType.Coward:
                        break;
                    case IaType.Ruthless:
                        break;
                    case IaType.Perfectionist:
                        break;
                    case IaType.Friendly:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
    }*/

    private ArenaTile FindFarthestFromTarget(int cMove)
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

                    if (tile.distance > cMove)
                    {
                        return tile.parent;
                    }

                    float distPToTarget = Vector3.Distance(tile.transform.position, target.transform.position);
                    float distTileToTarget = Vector3.Distance(tile.transform.position, target.transform.position);
                    if (distTileToTarget > distPToTarget)
                    {
                        GameObject TGO = tile.GetGameObjectOnTop();
                        if (TGO == null) process.Enqueue(tile);
                    }
                }
            }
        }

        return null;
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
                            _targetDistance = tile.distance;
                            return;
                        }
                    }
                }
            }
        }
    }
    
    private void FindFarthestTargetInRange() //A revoir
    {
        int distAtk = atkRange + move;

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

                    if (tile.distance > distAtk) return;
                    process.Enqueue(tile);

                    GameObject TGO = tile.GetGameObjectOnTop();
                    if (TGO != null)
                    {
                        if (TGO.CompareTag("Player") && tile.distance > _targetDistance)
                        {
                            target = TGO;
                            _targetDistance = tile.distance;
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
                        if (TGO.CompareTag("Player") && tile.distance > _targetDistance)
                        {
                            target = TGO;
                            _targetDistance = tile.distance;
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
                        if(TGO.CompareTag("Player") && TGO.GetComponent<CombatStat>().isAlive)
                        {
                            int targetHp = 1000;
                            if(target != null)
                                targetHp = target.GetComponent<CombatStat>().CurrHp;
                            int TGOHp = TGO.GetComponent<CombatStat>().CurrHp;
                            if(targetHp == 0 && targetHp == TGOHp)
                            {
                                targetHp = target.GetComponent<CombatStat>().MaxHp;
                                TGOHp = TGO.GetComponent<CombatStat>().MaxHp;
                            }
                            
                            if(targetHp > TGOHp)
                            {
                                target = TGO;
                                _targetDistance = tile.distance;
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
        return FindPathWoAll(targetTile);
    }

    private void DumbAI()
    {
        if(!temp)
        {
            FindNearestTarget();
            RemoveSelectableTile();
            temp = true;
        }

        bool isAttacking = AttackAI(); //lance les attaques
        if (isAttacking) return;
        
        /*if(target != null && _targetDistance <= atkRange && !moving && !_alreadyMoved) //Attack en début de tour si un Player est dans la range
        {
            attacking = true;
            Attack(target, 1);
            return;
        }
        if(_alreadyMoved) //Attack après avoir bougé si un Player est dans la range
        {
            if(target != null && _targetDistance <= atkRange)
            {
                attacking = true;
                Attack(target, 1);
                return;
            }
            else
            {
                EndTurnT();
                return;
            }
        }*/

        if(!attacking && !moving && !_alreadyMoved) //Calcul du mouvement
        {
            transform.GetChild(0).Translate(0, MoveY, 0);
            bool findPath = CalculatePathFull(); //Calcul du trajet normal
            int tDist = _targetDistance;
            bool findPathV2 = CalculatePathWoTrap(); //Calcul du trajet sans les pieges
            int tDistV2 = _targetDistance;

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
                    findPath = CalculatePathWoAll();
                    if(findPath)
                    {
                        MoveToTile(ActualTargetTile);
                    }                 
                    else
                    {
                        transform.GetChild(0).Translate(0, -MoveY, 0);
                        EndTurnT();
                    }
                    /*transform.GetChild(0).Translate(0, -MoveY, 0);
                    EndTurnT();*/
                }
            }
            tileMoved = _path.Count-1;
        }
        if(moving) //Application du mouvement
        {
            Move();
        }
    }
    
    private void CowardAI()
    {
        if(!temp)
        {
            _tempMove = move;
            FindNearestTarget();

            RemoveSelectableTile();
            temp = true;

            if(_targetDistance == atkRange)
            {
                attacking = true;
                Attack(target, 1);
                return;
            }
            else if(_targetDistance < atkRange)
            {
                move = atkRange-_targetDistance;
                
                ActualTargetTile = FindFarthestFromTarget(move);
                
                transform.GetChild(0).Translate(0, MoveY, 0);
                MoveToTile(ActualTargetTile);
            }
            else
            {
                move = _targetDistance-atkRange;
                if(move > _tempMove)
                {
                    move = _tempMove;
                }
                
                transform.GetChild(0).Translate(0, MoveY, 0);
            
                bool findPath = CalculatePathFull(); //Calcul du trajet normal
                if (findPath) //Si seul le 1er trajet et valide
                {
                    CalculatePathFull();
                    MoveToTile(ActualTargetTile);
                }
                else
                {
                    //recherche de chemin en ignorant les pieges
                    findPath = CalculatePathWoTrap();
                    if (findPath)
                    {
                        MoveToTile(ActualTargetTile);
                    }
                    else
                    {
                        //recherche de chemin en ignorant les pieges et les caisses
                        findPath = CalculatePathWoCrate();
                        if (findPath)
                        {
                            MoveToTile(ActualTargetTile);
                        }
                        else
                        {
                            //recherche de chemin en ignorant tous les obstacles
                            findPath = CalculatePathWoAll();
                            if (findPath)
                            {
                                MoveToTile(ActualTargetTile);
                            }
                            else
                            {
                                transform.GetChild(0).Translate(0, -MoveY, 0);
                                EndTurnT();
                            }
                            /*transform.GetChild(0).Translate(0, -MoveY, 0);
                            EndTurnT();*/
                        }
                    }
                }

                tileMoved = _path.Count-1;
            }
        }
        
        if(_alreadyMoved) //Attack après avoir bougé si un Player est dans la range
        {
            if(target != null && _targetDistance <= atkRange)
            {
                attacking = true;
                Attack(target, 1);
                return;
            }
            else
            {
                EndTurnT();
                return;
            }
        }
        
        if(moving) //Application du mouvement
        {
            Move();
        }
    }

    private void RuthlessAI()
    {
        if(!temp)
        {
            FindLowestHpTarget();
            Debug.Log(target.name);
            RemoveSelectableTile();
            temp = true;
        }
        
        bool isAttacking = AttackAI(); //lance les attaques
        if (isAttacking) return;
        
        /*if(target != null && _targetDistance <= atkRange && !moving && !_alreadyMoved) //Attack en début de tour si un Player est dans la range
        {
            attacking = true;
            Attack(target, 1);
            return;
        }
        if(_alreadyMoved) //Attack après avoir bougé si un Player est dans la range
        {
            if(target != null && _targetDistance <= atkRange)
            {
                attacking = true;
                Attack(target, 1);
                return;
            }
            else
            {
                EndTurnT();
                return;
            }
        }*/
        
        if(!attacking && !moving && !_alreadyMoved) //Calcul du mouvement
        {
            transform.GetChild(0).Translate(0, MoveY, 0);
            bool findPath = CalculatePathFull(); //Calcul du trajet normal
            
            if (findPath) //Si seul le 1er trajet et valide
            {
                CalculatePathFull();
                MoveToTile(ActualTargetTile);
            }
            else
            {
                //recherche de chemin en ignorant les pieges
                findPath = CalculatePathWoTrap();
                if (findPath)
                {
                    MoveToTile(ActualTargetTile);
                }
                else
                {
                    //recherche de chemin en ignorant les pieges et les caisses
                    findPath = CalculatePathWoCrate();
                    if (findPath)
                    {
                        MoveToTile(ActualTargetTile);
                    }
                    else
                    {
                        //recherche de chemin en ignorant tous les obstacles
                        findPath = CalculatePathWoAll();
                        if (findPath)
                        {
                            MoveToTile(ActualTargetTile);
                        }
                        else
                        {
                            transform.GetChild(0).Translate(0, -MoveY, 0);
                            EndTurnT();
                        }
                        /*transform.GetChild(0).Translate(0, -MoveY, 0);
                        EndTurnT();*/
                    }
                }
            }

            tileMoved = _path.Count-1;
        }
        if(moving) //Application du mouvement
        {
            Move();
        }
    }

    private void PerfectionistAI()
    {
        if(!temp)
        {
            FindFarthestTargetInRange();
            if(target == null)
            {
                FindFarthestTarget();
            }
            
            RemoveSelectableTile();
            temp = true;
        }
        
        if(!attacking && !moving && !_alreadyMoved) //Calcul du mouvement
        {
            transform.GetChild(0).Translate(0, MoveY, 0);
            bool findPath = CalculatePathFull(); //Calcul du trajet normal
            
            if (findPath) //Si seul le 1er trajet et valide
            {
                CalculatePathFull();
                MoveToTile(ActualTargetTile);
            }
            else
            {
                //recherche de chemin en ignorant les pieges
                findPath = CalculatePathWoTrap();
                if (findPath)
                {
                    MoveToTile(ActualTargetTile);
                }
                else
                {
                    //recherche de chemin en ignorant les pieges et les caisses
                    findPath = CalculatePathWoCrate();
                    if (findPath)
                    {
                        MoveToTile(ActualTargetTile);
                    }
                    else
                    {
                        //recherche de chemin en ignorant tous les obstacles
                        findPath = CalculatePathWoAll();
                        if (findPath)
                        {
                            MoveToTile(ActualTargetTile);
                        }
                        else
                        {
                            transform.GetChild(0).Translate(0, -MoveY, 0);
                            EndTurnT();
                        }
                        /*transform.GetChild(0).Translate(0, -MoveY, 0);
                        EndTurnT();*/
                    }
                }
            }

            tileMoved = _path.Count-1;
        }
        if(moving) //Application du mouvement
        {
            Move();
        }
    }

    private bool AttackAI()
    {
        if(target != null && _targetDistance <= atkRange && !moving && !_alreadyMoved) //Attack en début de tour si un Player est dans la range
        {
            attacking = true;
            Attack(target, 1);
            return true;
        }
        if(_alreadyMoved) //Attack après avoir bougé si un Player est dans la range
        {
            if(target != null && _targetDistance <= atkRange)
            {
                attacking = true;
                Attack(target, 1);
                return true;
            }
            else
            {
                EndTurnT();
                return true;
            }
        }

        return false;
    }

    protected override void EndOfMovement()
    {
        Debug.Log(_targetDistance+" - "+ tileMoved);
        _targetDistance -= tileMoved;
        _alreadyMoved = true;

        if(_tempMove != -1)
        {
            move = _tempMove;
            _tempMove = -1;
        }
        
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
        temp = false;
        attacking = false;
        _alreadyMoved = false;
    }
}
