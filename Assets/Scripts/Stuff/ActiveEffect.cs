using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ActiveEffect
{
    public ActiveType activeType;
    public int value;
    public ActiveTarget target;
}
