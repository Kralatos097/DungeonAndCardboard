using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ActiveEffect
{
    public ActiveType activeType;
    public StatusEffect statusEffect;
    public int value;
}
