using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaTile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer overlay;

    public bool walkable = true;
    public bool current = false;
    public bool target = false;
    public bool selectable = false;

    [HideInInspector] public List<ArenaTile> adjacencyList = new List<ArenaTile>();
    
    //Needed BFS (breadth first search)
    [HideInInspector] public bool visited = false;
    [HideInInspector] public ArenaTile parent = null;
    [HideInInspector] public int distance = 0;
    
    //Needed A*
    [HideInInspector] public float f = 0;
    [HideInInspector] public float g = 0;
    [HideInInspector] public float h = 0;

    private void Update()
    {
        if(!TacticsMovement.PlayersTurn)
        {
            overlay.color = new Color(1, 1, 1, 0);
            return;
        }

        if(current)
        {
            overlay.color = new Color(1,0,1,.5f);
        }
        else if(target)
        {
            overlay.color = new Color(0,1,0,.5f);
        }
        else if(selectable)
        {
            overlay.color = new Color(1,0,0,.5f);
        }
        else
        {
            overlay.color = new Color(1,1,1,0);
        }
    }

    public void Reset()
    {
        walkable = true;
        current = false;
        target = false;
        selectable = false;

        adjacencyList.Clear();
    
        visited = false;
        parent = null;
        distance = 0;

        f = 0;
        g = 0;
        h = 0;
    }
    
    public void FindNeighbors(ArenaTile target)
    {
        Reset();
        
        CheckTile(Vector3.forward, target);
        CheckTile(Vector3.back, target);
        CheckTile(Vector3.right, target);
        CheckTile(Vector3.left, target);
    }
    
    public void FindNeighborsAtk()
    {
        Reset();
        
        CheckTileAtk(Vector3.forward);
        CheckTileAtk(Vector3.back);
        CheckTileAtk(Vector3.right);
        CheckTileAtk(Vector3.left);
    }

    public void CheckTile(Vector3 dir, ArenaTile target)
    {
        Vector3 halfExtents = new Vector3(.25f,.25f,.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + dir, halfExtents);

        foreach (Collider item in colliders)
        {
            ArenaTile arenaTile = item.GetComponent<ArenaTile>();
            if (arenaTile != null && arenaTile.walkable == true)
            {
                RaycastHit hit;

                if (!Physics.Raycast(arenaTile.transform.position, Vector3.up, out hit, 1) || (arenaTile == target))
                {
                    adjacencyList.Add(arenaTile);
                }
            }
        }
    }
    
    public void CheckTileAtk(Vector3 dir)
    {
        Vector3 halfExtents = new Vector3(.25f,.25f,.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + dir, halfExtents);

        foreach (Collider item in colliders)
        {
            ArenaTile arenaTile = item.GetComponent<ArenaTile>();
            if (arenaTile != null)
            {
                //RaycastHit hit;
                adjacencyList.Add(arenaTile);
            }
        }
    }

    public GameObject GetGameObjectOnTop()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.up, out hit, 1);
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 1))
        {
            return hit.transform.gameObject;
        }
        return null;
    }
}
