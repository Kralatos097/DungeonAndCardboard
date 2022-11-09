using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DungeonTile : MonoBehaviour
{
    public SpriteRenderer Overlay;
    public SpriteRenderer OverlayS;
    public SpriteRenderer OverlayE;

    public RoomType roomType = RoomType.Normal;
    
    [Header("Room State")]
    public bool selectable;
    public bool emptied = false;
    public bool walkedInto;
    private bool _current = false;
    public bool current
    {
        get => _current;
        set
        {
            _current = value;

            if(!_current) return;
            if(walkedInto) return;
        
            DiscoverTiles();
            walkedInto = true;
        }
    }

    [Header("Room Acces")]
    public bool doorUp = false;
    public bool doorDown = false;
    public bool doorRight = false;
    public bool doorLeft = false;

    [Header("Room Sprite")]
    public Sprite bossRoomSprite;
    public Sprite treasureRoomSprite;
    public Sprite startingRoomSprite;
    
    [Header("TileColors")]
    public Color currentColor = new Color(0, 1, 0, 0.3f);
    public Color selectableColor = new Color(.25f, .25f, .25f, 0.3f);
    public Color walkedIntoColor = new Color(1, 1, 1, 0.3f);

    private void Start()
    {
        if (roomType == RoomType.Starting)
        {
            current = true;
            selectable = true;
        }
    }

    private void Update()
    {
        ShowTile();
        
        if(current)
            ChangeColor(currentColor);
        else if(selectable && !walkedInto)
            ChangeColor(selectableColor);
        else if(walkedInto)
            ChangeColor(walkedIntoColor);

        switch (roomType)
        {
            case RoomType.Normal:
                break;
            case RoomType.Boss:
                ChangeSprite(bossRoomSprite);
                break;
            case RoomType.Treasure:
                ChangeSprite(treasureRoomSprite);
                break;
            case RoomType.Fighting:
                break;
            case RoomType.Starting:
                ChangeSprite(startingRoomSprite);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        OverlayE.gameObject.SetActive(emptied);

    }

    private void ChangeColor(Color color)
    {
        Overlay.color = color;
    }
    
    private void ChangeSprite(Sprite sprite)
    {
        OverlayS.color = Color.white;
        OverlayS.sprite = sprite;
    }
    
    private void CleanSprite()
    {
        OverlayS.color = new Color(1,1,1,0);
        OverlayS.sprite = null;
    }

    private void DiscoverTiles()
    {
        if(doorUp)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.forward, out hit, 1);
            DungeonTile djTile =  hit.collider.GetComponent<DungeonTile>();
            if(djTile != null)
            {
                djTile.selectable = true;
            }
        }
        if(doorDown)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.back, out hit, 1);
            DungeonTile djTile =  hit.collider.GetComponent<DungeonTile>();
            if(djTile != null)
            {
                djTile.selectable = true;
            }
        }
        if(doorRight)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.right, out hit, 1);
            DungeonTile djTile =  hit.collider.GetComponent<DungeonTile>();
            if(djTile != null)
            {
                djTile.selectable = true;
            }
        }
        if(doorLeft)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.left, out hit, 1);
            DungeonTile djTile =  hit.collider.GetComponent<DungeonTile>();
            if(djTile != null)
            {
                djTile.selectable = true;
            }
        }
    }

    private void ShowTile()
    {
        foreach (Transform GO in transform)
        {
            GO.gameObject.SetActive(selectable);
        }
    }
}
