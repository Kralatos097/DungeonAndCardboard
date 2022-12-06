using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CombatStat : MonoBehaviour
{
    [HideInInspector] public int initiative;

    private int _maxHp;
    public int MaxHp
    {
        get => _maxHp;

        set
        {
            _maxHp = value;

            isAlive = _maxHp > 0;
        }
    }
    
    private int _currHp;
    public int CurrHp
    {
        get => _currHp ;
        set
        {
            if (holyShield)
            {
                holyShield = false;
            }
            else
            {
                if (armor > 0)
                {
                    armor -= value;
                    if (armor < 0)
                    {
                        _currHp += armor;
                        armor = 0;
                    }
                }
                else
                {
                    _currHp = value;
                }

                if (_currHp <= 0)
                {
                    _currHp = 0;
                    isUp = false;
                    UnitDeath();
                }
                else if (_currHp > MaxHp)
                {
                    _currHp = MaxHp;
                }

                if (_currHp > 0)
                {
                    isUp = true;
                }
            }
        }
    }

    [HideInInspector] public int armor = 0;
    [HideInInspector] public bool holyShield = false; 
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

    [HideInInspector] public bool isUp = true;
    [HideInInspector] public bool isAlive = true;

    public void RollInit()
    {
        currInit = initiative + Random.Range(1,7);
    }

    private void UnitDeath()
    {
        if(gameObject.CompareTag("Player"))
        {
            transform.GetChild(0).GetComponent<Renderer>().material.color = Color.grey;
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.position = new Vector3(-100, -100, -100);
        }
    }

    private void ReviveUnit()
    {
        transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
    }

    [ContextMenu("Kill Unit")]
    public void KillUnit()
    {
        CurrHp -= 100;
    }

    public void TakeDamage(int value)
    {
        CurrHp-=value;
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
    
    [ContextMenu("Armor Unit")]
    public void ArmorTest()
    {;
        armor += 1;
    }
    
    [ContextMenu("Burn Unit")]
    public void BurnTest()
    {
        StatusEffect = StatusEffect.Burn;
        statusValue = 2;
    }
    
    [ContextMenu("Poison Unit")]
    public void PoisonTest()
    {
        StatusEffect = StatusEffect.Poison;
        statusValue = 2;
    }
    
    [ContextMenu("Stun Unit")]
    public void StunTest()
    {
        StatusEffect = StatusEffect.Stun;
        statusValue = 1;
    }
    
    [ContextMenu("Freeze Unit")]
    public void FreezeTest()
    {
        StatusEffect = StatusEffect.Freeze;
        statusValue = 2;
    }
}
