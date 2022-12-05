using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Stuff : ScriptableObject
{
    [HideInInspector] public string stuffName;
    [Header("Value")]
    public Sprite logo;
    public string description;
    
    protected void Start()
    {
        string Name = name;
    }
}
