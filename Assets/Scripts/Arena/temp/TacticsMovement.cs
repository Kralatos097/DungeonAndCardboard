using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TacticsMovement : MonoBehaviour
{
    protected bool Turn = false;
    public static bool PlayersTurn = false;
    
    private List<ArenaTile> selectableTiles = new List<ArenaTile>();
    private GameObject[] tiles;

    private Stack<ArenaTile> path = new Stack<ArenaTile>();
    private ArenaTile currentTile;

    protected bool moving = false;
    protected bool attacking = false;
    protected int move = 3;
    public float moveSpeed = 2;
    
    protected int atkRange = 0;

    protected float MoveY = .75f;
    protected bool passM = false;

    private Vector3 velocity = new Vector3();
    private Vector3 heading = new Vector3();

    private float halfHeight = 0;

    [HideInInspector] public ArenaTile actualTargetTile;

    protected Active equipmentOne;
    protected int EquiOneCD = 0;
    
    protected Active equipmentTwo;
    protected int EquiTwoCD = 0;

    protected Consumable consummable;
    
    protected Passive passif;

    private Material _unitMat;
    private Color _baseColor;
    private Color _changeColor;

    protected CombatStat _combatStat;

    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        
        _combatStat = gameObject.GetComponent<CombatStat>();
        
        GetUnitInfo();

        halfHeight = GetComponent<Collider>().bounds.extents.y;

        _combatStat.RollInit();
        
        TurnManager.AddUnit(this);
    }

    protected virtual void GetUnitInfo()
    {
        
    }
    
    protected void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    protected ArenaTile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        ArenaTile tile = null;
        
        if (Physics.Raycast(target.transform.position, Vector3.down, out hit, 1))
        {
            tile = hit.collider.GetComponent<ArenaTile>();
        }

        return tile;
    }

    protected void ComputeAdjacencyList()
    {
        foreach (GameObject tile in tiles)
        {
            ArenaTile t = tile.GetComponent<ArenaTile>();
            t.FindNeighbors(null);
        }
    }
    
    protected void ComputeAdjacencyListAtk()
    {
        foreach (GameObject tile in tiles)
        {
            ArenaTile t = tile.GetComponent<ArenaTile>();
            t.FindNeighborsAtk();
        }
    }
    
    protected void ComputeAdjacencyList(ArenaTile target)
    {
        foreach (GameObject tile in tiles)
        {
            ArenaTile t = tile.GetComponent<ArenaTile>();
            t.FindNeighbors(target);
        }
    }

    protected void FindSelectableTile()
    {
        ComputeAdjacencyList();
        GetCurrentTile();

        Queue<ArenaTile> process = new Queue<ArenaTile>();
        
        process.Enqueue(currentTile);
        currentTile.visited = true;

        while (process.Count > 0)
        {
            ArenaTile t = process.Dequeue();
            
            selectableTiles.Add(t);
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
        path.Clear();
        tile.target = true;
        moving = true;

        ArenaTile next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    protected void Move()
    {
        if (path.Count > 0)
        {
            ArenaTile t = path.Peek();
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
                path.Pop();
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
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }
        
        foreach (ArenaTile tile in selectableTiles)
        {
            tile.Reset();
        }
        
        selectableTiles.Clear();
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
        GetCurrentTile();

        List<ArenaTile> openList = new List<ArenaTile>();
        List<ArenaTile> closedList = new List<ArenaTile>();
        
        openList.Add(currentTile);
        currentTile.h = Vector3.Distance(currentTile.transform.position, targetTile.transform.position);
        currentTile.f = currentTile.h;

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
        GetCurrentTile();

        Queue<ArenaTile> process = new Queue<ArenaTile>();
        
        process.Enqueue(currentTile);
        currentTile.visited = true;

        while (process.Count > 0)
        {
            ArenaTile t = process.Dequeue();
            
            selectableTiles.Add(t);

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
        GetCurrentTile();

        Queue<ArenaTile> process = new Queue<ArenaTile>();
        
        process.Enqueue(currentTile);
        currentTile.visited = true;

        while (process.Count > 0)
        {
            ArenaTile t = process.Dequeue();
            
            selectableTiles.Add(t);
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
        
        switch (equip)
        {
            case 1:
                equipmentOne.Effect(gameObject, target, hit);
                target.gameObject.GetComponent<TacticsMovement>().DamageClign();
                EquiOneCD = equipmentOne.GetCd();
                break;
            case 2:
                equipmentTwo.Effect(gameObject, target, hit);
                target.gameObject.GetComponent<TacticsMovement>().DamageClign();
                EquiTwoCD = equipmentTwo.GetCd();
                break;
            case 3:
                consummable.Effect(gameObject, target, hit);
                target.gameObject.GetComponent<TacticsMovement>().DamageClign();
                consummable = null;
                break;
            default:
                break;
        }

        Debug.Log("ATTACKING " + target.gameObject.name + "!\n Now has : " + target.GetComponent<CombatStat>().currHp + " HP!");
        EndOfAttack();
    }
    
    //return 0 for a miss, 1 for a hit, 2 for a critical
    public int GetHitChance()
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
        if (equipmentOne != null) EquiOneCD-=value;
        if (equipmentTwo != null) EquiTwoCD-=value;

        if (EquiOneCD < 0) EquiOneCD = 0;
        if (EquiTwoCD < 0) EquiTwoCD = 0;
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
        return passif;
    }
}