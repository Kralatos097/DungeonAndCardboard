using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatStat : MonoBehaviour
{
    [HideInInspector] public int initiative;

    [HideInInspector] public int MaxHp;
    private int _currHp;

    public int currHp
    {
        get => _currHp ;
        set
        {
            _currHp = value;
            if(_currHp <= 0)
            {
                _currHp = 0;
                isAlive = false;
                UnitDeath();
            }
            else if(_currHp > MaxHp)
            {
                _currHp = MaxHp;
            }
            /*if(_currHp > 0)
            {
                isAlive = true;
            }*/
        }
    }

    [HideInInspector] public int armor = 0;
    [HideInInspector] public StatusEffect StatusEffect = StatusEffect.Nothing;
    [HideInInspector] public int statusValue = 0;

    [HideInInspector] public int currInit;

    [HideInInspector] public bool isAlive = true;

    public void RollInit()
    {
        currInit = initiative + Random.Range(1,7);
    }

    private void UnitDeath()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        //todo: a virer pour les player
        transform.position = new Vector3(-100, -100, -100);
    }

    [ContextMenu("KillUnit")]
    public void KillUnit()
    {
        currHp -= 100;
    }
}
