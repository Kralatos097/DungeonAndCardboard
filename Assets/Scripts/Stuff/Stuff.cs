using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Stuff : ScriptableObject
{
    [HideInInspector] public string stuffName;
    [HideInInspector] public string stuffType;
    
    [Header("Value")]
    public Sprite logo;
    public string description;
    
    protected virtual void Start()
    {
        stuffName = name;
    }
}
