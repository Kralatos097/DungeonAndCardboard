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
            if (armor > 0)
            {
                armor -= value;
                if(armor < 0)
                {
                    _currHp = armor * -1;
                    armor = 0;
                }
            }
            else
            {
                _currHp = value;
            }
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
            if(_currHp > 0)
            {
                isAlive = true;
            }
        }
    }

    [HideInInspector] public int armor = 0;
    [HideInInspector] public StatusEffect StatusEffect = StatusEffect.Nothing;
    private int statusValue = 0;
    public int StatusValue
    {
        get => statusValue;
        set
        {
            statusValue = value;
            if(statusValue <= 0)
            {
                StatusEffect = StatusEffect.Nothing;
            }
        }
    }

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

    [ContextMenu("Kill Unit")]
    public void KillUnit()
    {
        currHp -= 100;
    }

    public void TakeDamage(int value)
    {
        currHp-=value;
    }

    public void Poison()
    {
        TakeDamage(1);
        StatusValue--;
    }
    
    public void Burn()
    {
        TakeDamage(StatusValue);
        StatusValue = 0;
    }
    
    public void ResetStatus()
    {
        StatusEffect = StatusEffect.Nothing;
        statusValue = 0;
    }
    
    [ContextMenu("Burn Unit")]
    public void BurnTest()
    {
        StatusEffect = StatusEffect.Burn;
        statusValue = 2;
    }
}
