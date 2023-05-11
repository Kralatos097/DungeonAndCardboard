using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class NPCMove : TacticsMovement
{
    private bool _alreadyMoved = false;
    private int _tileMoved = 0;
    private int _tempMove = -1;
    
    [HideInInspector] public bool firstTimePass = false;
    
    [SerializeField] protected EnemieBaseInfo UnitInfo;
    
    private IaType iaType;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        Init();
    }

    protected override void GetUnitInfo()
    {
        CombatStat combatStat = gameObject.GetComponent<CombatStat>();
        
        combatStat.MaxHp = RandomHp(UnitInfo.maxHp.x, UnitInfo.maxHp.y);
        combatStat.CurrHp = combatStat.MaxHp;
        combatStat.ChangeInit(UnitInfo.initiative);

        baseMove = UnitInfo.movement;
        move = UnitInfo.movement;
        ActiveOne = UnitInfo.activeOne;
        ActiveTwo = UnitInfo.activeTwo;
        Passive = UnitInfo.passive;
        Consumable = UnitInfo.consumable;

        iaType = UnitInfo.iaType;
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
                FriendlyAI();
                break;
            case IaType.BossS1:
                BossS1AI();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private ArenaTile FindFarthestFromTarget()
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

                    if (tile.distance > move)
                    {
                        return tile.parent;
                    }

                    float distPToTarget = Vector3.Distance(t.transform.position, target.transform.position);
                    float distTileToTarget = Vector3.Distance(tile.transform.position, target.transform.position);
                    if (distTileToTarget >= distPToTarget)
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
    
    private void FindNearestTargetInRangeAlive()
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
                    
                    if (tile.distance > atkRange + move) return;

                    GameObject TGO = tile.GetGameObjectOnTop();
                    if (TGO != null)
                    {
                        if (TGO.CompareTag("Player") && TGO.GetComponent<CombatStat>().isUp)
                        {
                            target = TGO;
                            _targetDistance = tile.distance;
                            return;
                        }
                    }
                    else process.Enqueue(tile);
                }
            }
        }
    }
    
    private void FindFarthestTargetInRange()
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

                    if (tile.distance > atkRange + move) return;

                    GameObject TGO = tile.GetGameObjectOnTop();
                    if (TGO != null)
                    {
                        if (TGO.CompareTag("Player") && tile.distance >= _targetDistance)
                        {
                            target = TGO;
                            _targetDistance = tile.distance;
                        }
                    }
                    else process.Enqueue(tile);
                }
            }
        }
    }
    
    private void FindFarthestTargetInRangeAlive()
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

                    GameObject TGO = tile.GetGameObjectOnTop();
                    if (TGO != null)
                    {
                        if (TGO.CompareTag("Player") && tile.distance >= _targetDistance & TGO.GetComponent<CombatStat>().isUp)
                        {
                            target = TGO;
                            _targetDistance = tile.distance;
                        }
                    }
                    else process.Enqueue(tile);
                }
            }
        }
    }
    
    private void FindLowestLifeEnemyInRange()
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
                    if (TGO != null && TGO.GetComponent<TacticsMovement>() != null)
                    {
                        bool comp = true;
                        if (target != null && target.GetComponent<TacticsMovement>() != null)
                        {
                            CombatStat targetCs = target.GetComponent<CombatStat>();
                            CombatStat TgoCs = TGO.GetComponent<CombatStat>();
                            if ((targetCs.CurrHp / targetCs.MaxHp)<=(TgoCs.CurrHp / TgoCs.MaxHp))
                            {
                                comp = false;
                            }
                        }
                        
                        if (TGO.CompareTag("Enemy") && comp)
                        {
                            target = TGO;
                            _targetDistance = tile.distance;
                        }
                    }
                }
            }
        }
    }
    
    private void FindLowestLifeEnemy()
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
                        bool comp = true;
                        if (target != null)
                        {
                            CombatStat targetCs = target.GetComponent<CombatStat>();
                            CombatStat TgoCs = TGO.GetComponent<CombatStat>();
                            if ((targetCs.CurrHp / targetCs.MaxHp)<=(TgoCs.CurrHp / TgoCs.MaxHp))
                            {
                                comp = false;
                            }
                        }
                        
                        if (TGO.CompareTag("Enemy") && comp)
                        {
                            target = TGO;
                            _targetDistance = tile.distance;
                        }
                    }
                }
            }
        }
    }
    
    private void FindFarthestTarget()
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
                        if (TGO.CompareTag("Player") && tile.distance >= _targetDistance)
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
        if (target == null) return false;
        ArenaTile targetTile = GetTargetTile(target);
        return FindPathFull(targetTile);
    }
    
    private bool CalculatePathWoTrap()
    {
        if (target == null) return false;
        ArenaTile targetTile = GetTargetTile(target);
        return FindPathWoTrap(targetTile);
    }
    
    private bool CalculatePathWoCrate()
    {
        Debug.Log(target);
        if (target == null) return false;
        ArenaTile targetTile = GetTargetTile(target);
        return FindPathWoCrate(targetTile);
    }
    
    private bool CalculatePathWoAll()
    {
        
        if (target == null) return false;
        ArenaTile targetTile = GetTargetTile(target);
        return FindPathWoAll(targetTile);
    }

    private void DumbAI()
    {
        if(!firstTimePass)
        {
            FindNearestTargetInRangeAlive();
            
            bool findPath = CalculatePathFull();
            bool findPathV2 = CalculatePathWoTrap();

            if (target == null || (!findPath && !findPathV2)) FindNearestTarget();

            RemoveSelectableTile();
            firstTimePass = true;
        }

        bool isAttacking = AttackAI(); //lance les attaques
        if(isAttacking) return;

        if(!attacking && !moving && !_alreadyMoved) //Calcul du mouvement
        {
            animator.SetBool("IsGrounded", false);
            ArenaTile targetTile = target.GetComponent<TacticsMovement>().GetCurrentTile();
            bool findPath = CalculatePathFull(); //Calcul du trajet normal
            int tDist = GetTravelDist(targetTile);
            bool findPathV2 = CalculatePathWoTrap(); //Calcul du trajet sans les pieges
            int tDistV2 = GetTravelDist(targetTile);
                
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
                        animator.SetBool("IsGrounded", true);
                        EndTurnT();
                    }
                }
            }
            _tileMoved = _path.Count-1;
        }
        if(moving) //Application du mouvement
        {
            Move();
        }
    }

    private int GetTravelDist(ArenaTile arenaTile)
    {
        int d = 0;
        if (arenaTile.parent == null) return 100;
        
        while(arenaTile != _currentTile)
        {
            d++;
            arenaTile = arenaTile.parent;
        }

        return d;
    }

    protected int GetDistToTarget(ArenaTile arenaTile)
    {
        int ret = 0;

        
        
        return ret;
    }
    
    protected int GetDistToTargetRec(ArenaTile target, ArenaTile arenaTile, int nb)
    {


        if (target == arenaTile)
        {
            return nb++;
        }
        else
        {
            int ret = GetDistToTargetRec(target, arenaTile, nb);

            return ret;
        }
    }

    private void CowardAI()
    {
        if(!firstTimePass)
        {
            _tempMove = move;
            FindNearestTargetInRangeAlive();
            if (target == null)
            {
                FindNearestTarget();
            }

            RemoveSelectableTile();
            firstTimePass = true;
            
            if(_targetDistance == atkRange)
            {
                attacking = true;
                Attack(target, 1);
                return;
            }
            else if(_targetDistance < atkRange)
            {
                move = atkRange-_targetDistance;
                if(move > _tempMove)
                {
                    move = _tempMove;
                }
                
                ActualTargetTile = FindFarthestFromTarget();
                
                if (ActualTargetTile == null)
                {
                    attacking = true;
                    Attack(target, 1);
                    return;
                }
                else
                {
                    animator.SetBool("IsGrounded", false);
                    MoveToTile(ActualTargetTile);
                }
            }
            else
            {
                move = _targetDistance-atkRange;
                if(move > _tempMove)
                {
                    move = _tempMove;
                }
                Debug.Log(move);
                
                animator.SetBool("IsGrounded", false);
            
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
                                animator.SetBool("IsGrounded", true);
                                EndTurnT();
                            }
                        }
                    }
                }

                _tileMoved = _path.Count-1;
            }
        }
        
        if(_alreadyMoved) //Attack après avoir bougé si un Player est dans la range
        {
            Debug.Log(target +"\n"+ _targetDistance +" - "+ atkRange);
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
        if(!firstTimePass)
        {
            FindLowestHpTarget();
            Debug.Log(target.name);
            RemoveSelectableTile();
            firstTimePass = true;
        }
        
        bool isAttacking = AttackAI(); //lance les attaques
        if (isAttacking) return;
        
        if(!attacking && !moving && !_alreadyMoved) //Calcul du mouvement
        {
            animator.SetBool("IsGrounded", false);
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
                            animator.SetBool("IsGrounded", true);
                            EndTurnT();
                        }
                        /*animator.SetBool("IsGrounded", true);
                        EndTurnT();*/
                    }
                }
            }

            _tileMoved = _path.Count-1;
        }
        if(moving) //Application du mouvement
        {
            Move();
        }
    }

    private void PerfectionistAI()
    {
        if(!firstTimePass)
        {
            FindFarthestTargetInRangeAlive();
            
            bool findPath = CalculatePathFull();
            
            if (target == null || !findPath)
            {
                _targetDistance = 0;
                FindFarthestTargetInRange();
                findPath = CalculatePathFull();
            }
            if(target == null || !findPath)
            {
                _targetDistance = 0;
                FindFarthestTarget();
            }
            
            RemoveSelectableTile();
            firstTimePass = true;
        }
        
        bool isAttacking = AttackAI(); //lance les attaques
        if (isAttacking) return;
        
        if(!attacking && !moving && !_alreadyMoved) //Calcul du mouvement
        {
            animator.SetBool("IsGrounded", false);
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
                            animator.SetBool("IsGrounded", true);
                            EndTurnT();
                        }
                        /*animator.SetBool("IsGrounded", true);
                        EndTurnT();*/
                    }
                }
            }

            _tileMoved = _path.Count-1;
        }
        if(moving) //Application du mouvement
        {
            Move();
        }
    }
    
    private void FriendlyAI()
    {
        if(!firstTimePass)
        {
            FindLowestLifeEnemyInRange();
            if(target == null)
            {
                FindLowestLifeEnemy();
            }
            
            RemoveSelectableTile();
            firstTimePass = true;
        }
        
        bool isAttacking = AttackAI(); //lance les attaques
        if (isAttacking) return;
        
        if(!attacking && !moving && !_alreadyMoved) //Calcul du mouvement
        {
            animator.SetBool("IsGrounded", false);
            bool findPath = CalculatePathFull(); //Calcul du trajet normal
            
            if (findPath) //Si le trajet et valide
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
                            animator.SetBool("IsGrounded", true);
                            EndTurnT();
                        }
                    }
                }
            }

            _tileMoved = _path.Count-1;
        }
        if(moving) //Application du mouvement
        {
            Move();
        }
    }

    private void BossS1AI()
    {
        int nbEnemy = TurnManager.GetNbEnemyD();

        if(nbEnemy == 1)
        {
            GetComponent<SpawnEnemy>().LaunchSpawn();
            EndTurnT();
            return; 
        }
        else
        {
            DumbAI();
        }
    }

    private bool AttackAI()
    {
        if (CombatStat.GetStatusEffect() == StatusEffect.Stun && _alreadyMoved)
        {
            EndTurnT();
            return true;
        }
        else if (CombatStat.GetStatusEffect() == StatusEffect.Stun)
        {
            return false;
        }
        
        if(target != null && _targetDistance <= atkRange && !moving && !_alreadyMoved) //Attack en début de tour si un Player est dans la range
        {
            attacking = true;
            Attack(target, 1);
            return true;
        }
        if(_alreadyMoved) //Attack après avoir bougé si un Player est dans la range
        {
            if (target != null)
            {
                if (atkRange > 1)
                {
                    if (_targetDistance <= atkRange)
                    {
                        attacking = true;
                        Attack(target, 1);
                        return true;
                    }
                }
                else
                {
                    _currentTile = GetCurrentTile();
                    if (GetTravelDist(target.GetComponent<TacticsMovement>().GetCurrentTile()) <= atkRange)
                    {
                        attacking = true;
                        Attack(target, 1);
                        return true;
                    }
                }
            }

            EndTurnT();
            return true;
        }

        return false;
    }

    protected override void EndOfMovement()
    {
        /*_targetDistance -= _tileMoved;*/
        FindNearestTarget();
        _targetDistance = target.GetComponent<TacticsMovement>().GetCurrentTile().distance;
        
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
        firstTimePass = false;
        attacking = false;
        _alreadyMoved = false;
        _targetDistance = 0;
    }

    public IaType GetIaType()
    {
        return iaType;
    }

    public Sprite GetUiSprite()
    {
        return UnitInfo.charaUiIcon;
    }

    public void FriendlyTransform()
    {
        FriendlyTransformFx();
        Debug.Log("TRAAAAANSFORMATION!");

        (ActiveOne, ActiveTwo) = (ActiveTwo, ActiveOne);
        iaType = IaType.Dumb;
    }

    private void FriendlyTransformFx()
    {
        //todo
    }
}
