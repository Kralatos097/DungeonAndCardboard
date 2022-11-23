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
    
    public abstract void Effect(GameObject user);
    public abstract void Effect(GameObject user, int touchStatus);
    public abstract void Effect(GameObject user, GameObject target, int hit);
}
