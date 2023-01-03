using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TacticsMovement : MonoBehaviour
{
    protected bool Turn = false;
    public static bool PlayersTurn = false;
    
    private List<ArenaTile> _selectableTiles = new List<ArenaTile>();
    private GameObject[] _tiles;

    private Stack<ArenaTile> _path = new Stack<ArenaTile>();
    private ArenaTile _currentTile;

    protected bool moving = false;
    protected bool attacking = false;
    protected int baseMove = 3;
    private int move = 3;
    public float moveSpeed = 2;
    
    [HideInInspector] public int atkRange = 0;

    protected float MoveY = .75f;
    protected bool passM = false;

    private Vector3 velocity = new Vector3();
    private Vector3 heading = new Vector3();

    private float halfHeight = 0;

    [HideInInspector] public ArenaTile actualTargetTile;

    protected Active ActiveOne;
    protected int ActiveOneCd = 0;
    
    protected Active ActiveTwo;
    protected int ActiveTwoCd = 0;

    protected Consumable Consumable;
    
    private Passive _passive;
    protected Passive Passive
    {
        get => _passive;

        set
        {
            //todo: faire en sorte que les effets leseffets s'annulent lorsque l'on retire le passif
            /*if (_passive != null && _passive.GetPassiveTrigger() == PassiveTrigger.OnObtained)
            {
                _passive.Effect(gameObject);
            }*/
            
            _passive = value;

            if (_passive != null && _passive.GetPassiveTrigger() == PassiveTrigger.OnObtained)
            {
                _passive.Effect(gameObject);
            }
        }
    }

    private Material _unitMat;
    private Color _baseColor;
    private Color _changeColor;

    protected CombatStat CombatStat;

    protected void Init()
    {
        _tiles = GameObject.FindGameObjectsWithTag("Tile");
        
        CombatStat = gameObject.GetComponent<CombatStat>();
        
        GetUnitInfo();
        SetCurrentTile();

        halfHeight = GetComponent<Collider>().bounds.extents.y;

        CombatStat.RollInit();
        
        TurnManager.AddUnit(this);
    }

    protected virtual void GetUnitInfo() {}

    public ArenaTile GetCurrentTile()
    {
        return GetTargetTile(gameObject);
    }

    private void SetCurrentTile()
    {
        _currentTile = GetTargetTile(gameObject);
        _currentTile.current = true;
    }

    protected ArenaTile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        ArenaTile tile = null;
        
        if (Physics.Raycast(target.transform.position, Vector3.down, out hit, 2))
        {
            tile = hit.collider.GetComponent<ArenaTile>();
        }

        return tile;
    }

    protected void ComputeAdjacencyList()
    {
        foreach (GameObject tile in _tiles)
        {
            ArenaTile t = tile.GetComponent<ArenaTile>();
            t.FindNeighbors(null);
        }
    }
    
    protected void ComputeAdjacencyListAtk()
    {
        foreach (GameObject tile in _tiles)
        {
            ArenaTile t = tile.GetComponent<ArenaTile>();
            t.FindNeighborsAtk();
        }
    }
    
    protected void ComputeAdjacencyList(ArenaTile target)
    {
        foreach (GameObject tile in _tiles)
        {
            ArenaTile t = tile.GetComponent<ArenaTile>();
            t.FindNeighbors(target);
        }
    }

    protected void FindSelectableTile()
    {
        ComputeAdjacencyList();
        SetCurrentTile();

        Queue<ArenaTile> process = new Queue<ArenaTile>();
        
        process.Enqueue(_currentTile);
        _currentTile.visited = true;

        while (process.Count > 0)
        {
            ArenaTile t = process.Dequeue();
            
            _selectableTiles.Add(t);
            t.selectable = true;

            if (t.distance < move)
            {
                foreach (ArenaTile tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    protected void MoveToTile(ArenaTile tile)
    {
        _path.Clear();
        tile.target = true;
        moving = true;

        ArenaTile next = tile;
        while (next != null)
        {
            _path.Push(next);
            next = next.parent;
        }
    }

    protected void Move()
    {
        if (_path.Count > 0)
        {
            ArenaTile t = _path.Peek();
            Vector3 target = t.transform.position;

            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                CalculateHeading(target);
                SetHorizontalVelocity();

                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                //tile center reached
                transform.position = target;
                _path.Pop();
            }
        }
        else
        {
            RemoveSelectableTile();
            moving = false;
            
            EndOfMovement();
        }
    }

    private void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    private void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    protected void RemoveSelectableTile()
    {
        if (_currentTile != null)
        {
            _currentTile.current = false;
            _currentTile = null;
        }
        
        foreach (ArenaTile tile in _selectableTiles)
        {
            tile.Reset();
        }
        
        _selectableTiles.Clear();
    }

    public void BeginTurn()
    {
        StartTurnClign();
    }

    public void EndTurn()
    {
        Turn = false;
    }

    protected ArenaTile FindEndTile(ArenaTile t)
    {
        Stack<ArenaTile> tempPath = new Stack<ArenaTile>();

        ArenaTile next = t.parent;
        while(next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        if (tempPath.Count <= move)
        {
            return t.parent;
        }

        ArenaTile endTile = null;

        for (int i = 0; i <= move; i++)
        {
            endTile = tempPath.Pop();
        }

        return endTile;
    }

    protected void FindPath(ArenaTile targetTile)
    {
        ComputeAdjacencyList(targetTile);
        SetCurrentTile();

        List<ArenaTile> openList = new List<ArenaTile>();
        List<ArenaTile> closedList = new List<ArenaTile>();
        
        openList.Add(_currentTile);
        _currentTile.h = Vector3.Distance(_currentTile.transform.position, targetTile.transform.position);
        _currentTile.f = _currentTile.h;

        while (openList.Count > 0)
        {
            ArenaTile t = FindLowestF(openList);
            
            closedList.Add(t);

            if (t == targetTile)
            {
                actualTargetTile = FindEndTile(t);
                MoveToTile(actualTargetTile);
                return;
            }

            foreach (ArenaTile tile in t.adjacencyList)
            {
                if (closedList.Contains(tile))
                {
                    //Do nothing, already processed
                }
                else if (openList.Contains(tile))
                {
                    float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

                    if (tempG < tile.g)
                    {
                        tile.parent = t;
                        tile.g = tempG;
                        tile.f = tile.g + tile.h;
                    }
                }
                else
                {
                    tile.parent = t;

                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, targetTile.transform.position);

                    tile.f = tile.g + tile.h;
                    
                    openList.Add(tile);
                }
            }
        }
        
        //todo - what do you do if there is no path to the target tile?
        Debug.Log("Path not Found");
    }

    protected ArenaTile FindLowestF(List<ArenaTile> list)
    {
        ArenaTile lowest = list[0];

        foreach (ArenaTile t in list)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }

        list.Remove(lowest);
        
        return lowest;
    }

    protected virtual void EndOfMovement()
    {
        //Fin du Soulevement du pion lors du mouvement
        transform.GetChild(0).Translate(0,-MoveY,0);
        passM = false;
    }
    
    protected GameObject AlliesInAttackRange()
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

            if (t.distance < atkRange)
            {
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
                                return TGO;
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    protected void AffAttackRange()
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
            t.selectable = true;

            if (t.distance < atkRange)
            {
                foreach (ArenaTile tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    protected void Attack(GameObject target, int equip)
    {
        RemoveSelectableTile();
        int hit = GetHitChance();

        if(Passive)
        {
            if (Passive.GetPassiveTrigger() == PassiveTrigger.OnAttack)
            {
                hit = Passive.Effect(gameObject, hit);
            }
        }

        CrateCombatState targetCS = target.GetComponent<CrateCombatState>();
        if(targetCS)
        {
            switch (equip)
            {
                case 1:
                    ActiveOne.Effect(gameObject, target, hit);
                    ActiveOneCd = ActiveOne.GetCd();
                    break;
                case 2:
                    ActiveTwo.Effect(gameObject, target, hit);
                    ActiveTwoCd = ActiveTwo.GetCd();
                    break;
                case 3:
                    Consumable.Effect(gameObject, target, hit);
                    Consumable = null;
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (equip)
            {
                case 1:
                    ActiveOne.Effect(gameObject, target, hit);
                    ActiveOneCd = ActiveOne.GetCd();
                    break;
                case 2:
                    ActiveTwo.Effect(gameObject, target, hit);
                    ActiveTwoCd = ActiveTwo.GetCd();
                    break;
                case 3:
                    Consumable.Effect(gameObject, target, hit);
                    Consumable = null;
                    break;
                default:
                    break;
            }
        }

        Debug.Log("ATTACKING " + target.gameObject.name + "!\n Now has : " + target.GetComponent<CombatStat>().CurrHp + " HP!");
        EndOfAttack();
    }
    
    //return 0 for a miss, 1 for a hit, 2 for a critical
    private int GetHitChance()
    {
        int nb = Random.Range(1,7);
        return nb switch
        {
            1 => 0,
            6 => 2,
            _ => 1
        };
    }

    protected virtual void EndOfAttack()
    {
        RemoveSelectableTile();
    }

    public void EquipCDMinus(int value)
    {
        if (ActiveOne != null) ActiveOneCd-=value;
        if (ActiveTwo != null) ActiveTwoCd-=value;

        if (ActiveOneCd < 0) ActiveOneCd = 0;
        if (ActiveTwoCd < 0) ActiveTwoCd = 0;
    }

    public void StartTurnClign()
    {
        _unitMat = transform.GetChild(0).GetComponent<Renderer>().material;
        
        _baseColor = _unitMat.color;
        _changeColor = new Color(1f, .5f, .5f);
        
        float timing = 0.3f;
        ColorClign(timing);
        Invoke("TrueBeginTurn",timing*3);
    }
    
    public void DamageClign()
    {
        _unitMat = transform.GetChild(0).GetComponent<Renderer>().material;
        
        _baseColor = _unitMat.color;
        _changeColor = new Color(1,1,1,.5f);

        float timing = 0.2f;
        ColorClign(timing);
    }

    protected void ColorClign(float t)
    {
        ChangeColorChange();
        Invoke("ChangeColorBase", t);
        Invoke("ChangeColorChange", t*2);
        Invoke("ChangeColorBase", t*3);
    }

    protected void TrueBeginTurn()
    {
        Turn = true;
    }

    protected void ChangeColorBase()
    {
        _unitMat.color = _baseColor;
    }
    
    protected void ChangeColorChange()
    {
        _unitMat.color = _changeColor;
    }

    public Passive GetPassive()
    {
        return Passive;
    }

    public void ChangeMove(int value)
    {
        move = baseMove + value;
    }
}