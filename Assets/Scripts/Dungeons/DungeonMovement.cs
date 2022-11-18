using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    private Vector3 target;
    private bool canMove = false;

    private void Start()
    {
        target = transform.position;
        DungeonManager.CurrentTile = GetCurrentTile();
        canMove = true;
    }

    private void Update()
    {
        if(canMove/* && !DungeonUiManager.ArtworkShown*/ && !DungeonUiManager.inChoice)
        {
            CheckMove();
        }
        if (Vector3.Distance(transform.position, target) >= .02f)
        {
            DungeonManager.LaunchRoomTypeAction(RoomType.Starting);
            canMove = false;
        }
        else
        { 
            if(!DungeonManager.CurrentTile.emptied)
                DungeonManager.LaunchRoomTypeAction(DungeonManager.CurrentTile.roomType);
            canMove = true;
        }
        if(transform.position != target)
            MoveToTile(target);
    }

    public DungeonTile GetCurrentTile()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, 1);
        DungeonTile djTile =  hit.collider.transform.gameObject.GetComponent<DungeonTile>();
        if(djTile != null)
        {
            djTile.current = true;
            return djTile;
        }
        else
        {
            return null;
        }
    }

    private void CheckMove()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main!.ScreenPointToRay((Input.mousePosition));

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Tile"))
                {
                    DungeonTile t = hit.collider.transform.GetComponent<DungeonTile>();

                    if (t.selectable)
                    {
                        DungeonManager.CurrentTile.current = false;
                        target = new Vector3(t.transform.position.x, transform.position.y, t.transform.position.z);
                        t.current = true;
                        DungeonManager.CurrentTile = t;
                    }
                }
            }
        }
    }

    private void MoveToTile(Vector3 target)
    {
        transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
    }
}
