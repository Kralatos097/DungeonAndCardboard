using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerCard", order = 1)]
public class PlayerBaseInfo : ScriptableObject
{
    [FormerlySerializedAs("maxHpV2")] [MinMaxSlider(0, 20)]
    public Vector2Int maxHp;
    public int initiative;
    public int movement;
    public Active activeOne;
    public Active activeTwo;
    public Passive passive;
    public Consumable consumable;
}
