using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Stuff : ScriptableObject
{
    [HideInInspector] public string stuffType;
    protected bool pass = false;
    
    [Header("Value")]
    public Sprite logo;
    public string description;

    public virtual void OnEnable()
    {
        if (pass) return;
        else pass = false;
    }
}
