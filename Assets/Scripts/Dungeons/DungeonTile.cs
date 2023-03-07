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
    public SpriteRenderer RoomRenderer;

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
    public Sprite nonWalkedRoomSprite;
    
    [Header("Discovered Room Sprite")]
    public Sprite oneWaySprite;
    public Sprite corridorSprite;
    public Sprite angleSprite;
    public Sprite threeWaySprite;
    public Sprite fourWaySprite;
    
    /*[Header("TileColors")]
    public Color currentColor = new Color(0, 1, 0, 0.3f);
    //public Color selectableColor = new Color(.25f, .25f, .25f, 0.3f);
    //public Color walkedIntoColor = new Color(1, 1, 1, 0.3f);*/

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
        
        /*if(current)
            ChangeColor(currentColor);
        else */if(selectable && !walkedInto)
        {
            //ChangeColor(selectableColor);
            ChangeRoomSprite(nonWalkedRoomSprite);
        }
        else if(walkedInto)
        {
            //ChangeColor(walkedIntoColor);
            SetDiscoveredRoomSprite();
        }

        switch (roomType)
        {
            case RoomType.FirstRoom:
            case RoomType.Normal:
            case RoomType.Fighting:
                break;
            case RoomType.Boss:
                ChangeOverlaySprite(bossRoomSprite);
                break;
            case RoomType.Treasure:
                ChangeOverlaySprite(treasureRoomSprite);
                break;
            case RoomType.Starting:
                ChangeOverlaySprite(startingRoomSprite);
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
    
    private void ChangeOverlaySprite(Sprite sprite)
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

    private void SetDiscoveredRoomSprite()
    {
        switch(doorUp, doorDown, doorRight, doorLeft)
        {
            //1 portes
            case (true, false, false, false)://vers le haut
                ChangeRoomSprite(oneWaySprite);
                break;
            case ( false, true, false, false)://vers le bas
                ChangeRoomSprite(oneWaySprite, false, true);
                break;
            case (false, false, true, false)://vers la droite
                ChangeRoomSprite(oneWaySprite, Quaternion.Euler(90,90,0));
                break;
            case (false, false, false, true)://vers la gauche
                ChangeRoomSprite(oneWaySprite, Quaternion.Euler(90,-90,0));
                break;
            //2 portes
            case (true, true, false, false)://Couloir haut-bas
                ChangeRoomSprite(corridorSprite);
                break;
            case (true, false, true, false)://angle haut-droite
                ChangeRoomSprite(angleSprite, true, false);
                break;
            case (true, false, false, true)://angle haut-gauche
                ChangeRoomSprite(angleSprite);
                break;
            case (false, true, true, false)://angle bas-droite
                ChangeRoomSprite(angleSprite, true, true);
                break;
            case (false, true, false, true)://angle bas-gauche
                ChangeRoomSprite(angleSprite, false, true);
                break;
            case (false, false, true, true)://Couloir droite-gauche
                ChangeRoomSprite(corridorSprite, Quaternion.Euler(90,90,0));
                break;
            //3 portes
            case (true, true, true, false)://T vers haut-bas-droite
                ChangeRoomSprite(threeWaySprite, Quaternion.Euler(90,90,0));
                break;
            case (true, true, false, true)://T vers haut-bas-gauche
                ChangeRoomSprite(threeWaySprite, Quaternion.Euler(90,-90,0));
                break;
            case (true, false, true, true)://T vers haut-droite-gauche
                ChangeRoomSprite(threeWaySprite);
                break;
            case (false, true, true, true)://T vers bas-droite-gauche
                ChangeRoomSprite(threeWaySprite, false, true);
                break;
            //4 portes
            case (true, true, true, true)://Carrefour complet
                ChangeRoomSprite(fourWaySprite);
                break;
        }
    }
    
    private void ChangeRoomSprite(Sprite sprite)
    {
        RoomRenderer.flipX = false;
        RoomRenderer.flipY = false;
        RoomRenderer.transform.rotation = Quaternion.Euler(90,0,0);
        RoomRenderer.sprite = sprite;
    }

    private void ChangeRoomSprite(Sprite sprite, bool flipX, bool flipY)
    {
        RoomRenderer.flipX = flipX;
        RoomRenderer.flipY = flipY;
        RoomRenderer.transform.rotation = Quaternion.Euler(90,0,0);
        RoomRenderer.sprite = sprite;
    }
    
    private void ChangeRoomSprite(Sprite sprite, Quaternion angle)
    {
        RoomRenderer.flipX = false;
        RoomRenderer.flipY = false;
        RoomRenderer.transform.rotation = angle;
        RoomRenderer.sprite = sprite;
    }
    
    private void ChangeRoomSprite(Sprite sprite, bool flipX, bool flipY, Quaternion angle)
    {
        RoomRenderer.flipX = flipX;
        RoomRenderer.flipY = flipY;
        RoomRenderer.transform.rotation = angle;
        RoomRenderer.sprite = sprite;
    }
}
