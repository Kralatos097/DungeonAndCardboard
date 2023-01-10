using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CombatStat : MonoBehaviour
{
    private int _initiative;

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

    protected int _currHp;
    public virtual int CurrHp
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
                        TakeDamageFX();
                        _currHp += armor;
                        armor = 0;
                    }
                }
                else
                {
                    TakeDamageFX();
                    _currHp = value;
                }

                if (_currHp <= 0)
                {
                    if(_revive)
                    {
                        Debug.Log("Revived!");
                        _currHp = _reviveValue;
                        _revive = false;
                        _reviveValue = 0;
                    }
                    else
                    {
                        _currHp = 0;
                        isUp = false;
                        UnitDeath();
                    }
                }
                else if (_currHp > MaxHp)
                {
                    _currHp = MaxHp;
                }

                if (_currHp > 0 && !isUp)
                {
                    ReviveUnit();
                    isUp = true;
                }
            }
        }
    }

    private int armor = 0;
    private bool holyShield = false;
    private bool _revive = false;
    private int _reviveValue = 0;
    protected StatusEffect StatusEffect = StatusEffect.Nothing;
    protected int statusValue = 0;
    public virtual int StatusValue
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
        currInit = _initiative + Random.Range(1,7);
    }

    protected virtual void UnitDeath()
    {
        Passive passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
        if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnDeath)
        {
            passive.Effect(gameObject);
            if (isUp)
            {
                return;
            }
        }
        
        if(gameObject.CompareTag("Player"))
        {
            transform.GetChild(0).GetComponent<Renderer>().material.color = Color.grey;
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.position = new Vector3(-100, -100, -100);
            isAlive = false;
        }
    }

    private void ReviveUnit()
    {
        transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
    }

    public void TakeDamage(int value)
    {
        Passive passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
        if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnDamageTaken)
        {
            passive.Effect(gameObject);
        }
        
        CurrHp-=value;
    }

    public void TakeDamageFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch(gameObject.CompareTag("Player") ? "AllieDamaged" : "EnemyDamaged");
    }
    
    public void TakeHeal(int value)
    {
        Passive passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
        if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnHealTaken)
        {
            passive.Effect(gameObject);
        }
        
        FindObjectOfType<AudioManager>().RandomPitch(gameObject.CompareTag("Player") ? "AllieHealed" : "EnemyHealed");
        CurrHp+=value;
    }

    public void ChangeStatus(StatusEffect effect, int value)
    {
        Passive passive;
        switch(effect)
        {
            case StatusEffect.Poison:
                //todo: Poison Effect
                StatusEffect = effect;
                StatusValue = value;
                passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
                if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatueTaken)
                {
                    passive.Effect(gameObject);
                }
                break;
            case StatusEffect.Stun:
                //todo: Stun Effect
                StatusEffect = effect;
                StatusValue = value;
                passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
                if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatueTaken)
                {
                    passive.Effect(gameObject);
                }
                break;
            case StatusEffect.Burn:
                //todo: Burn Effect
                StatusEffect = effect;
                StatusValue = value;
                passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
                if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatueTaken)
                {
                    passive.Effect(gameObject);
                }
                break;
            case StatusEffect.Freeze:
                //todo: Freeze Effect
                StatusEffect = effect;
                StatusValue = value;
                passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
                if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatueTaken)
                {
                    passive.Effect(gameObject);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
        }
    }
    
    public int GetArmor()
    {
        return armor;
    }

    public void ChangeArmor(int value)
    {
        gameObject.GetComponent<CombatStat>().armor = value;
    }

    public void ActivatePoison()
    {
        TakeDamage(1);
        StatusValue--;
    }
    
    public void ActivateBurn()
    {
        TakeDamage(StatusValue);
        StatusValue = 0;
    }
    
    public void ResetStatus()
    {
        Passive passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
        if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatueClean)
        {
            passive.Effect(gameObject);
        }
        StatusEffect = StatusEffect.Nothing;
        statusValue = 0;
    }

    public int GetInit()
    {
        return _initiative;
    }

    public void ChangeInit(int value)
    {
        _initiative = value;
    }

    public StatusEffect GetStatusEffect()
    {
        return StatusEffect;
    }
    
    public int GetStatusValue()
    {
        return StatusValue;
    }

    public void ActivateHolyShield()
    {
        holyShield = true;
    }

    public void ActivateRevive(int value)
    {
        _revive = true;
        _reviveValue = value;
    }

    private void DamageFX()
    {
        
    }

    private void HealFX()
    {
        
    }

    private void DamageHolyShieldFX()
    {
        
    }

    private void GetBurnFX()
    {
        
    }

    private void GetFreezeFX()
    {
        
    }

    private void GetStunFX()
    {
        
    }

    private void GetPoisonFX()
    {
        
    }

    private void ActivatePoisonFX()
    {
        
    }
    
    //todo: Add more FX
    
    //-------------- TEST FUNCTION -------------

    [ContextMenu("Kill Unit")]
    public void KillUnit()
    {
        TakeDamage(100);
    }
    
    [ContextMenu("Heal Unit")]
    public void HealUnit()
    {
        TakeHeal(100);
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
