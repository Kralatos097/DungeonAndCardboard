using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ActiveCard", order = 2)]
public class Active : Stuff
{
    [SerializeField] private int range;
    [SerializeField] private int cd;
    [SerializeField] private ActiveTarget clickTarget;

    [Header("Effects")]
    [SerializeField] private List<ActiveEffect> activeEffectList;

    public override void OnEnable()
    {
        base.OnEnable();
        stuffType = "Active";
    }
    
    public void Effect(GameObject user, GameObject target, int hitParam)
    {
        switch (range)
        {
            case 0:
                SelfAtkFx();
                break;
            case 1:
                CacAtkFx();
                break;
            case > 1:
                RangeAtkFx();
                break;
            default:
                Debug.LogWarning("Out of range");
                break;
        }

        foreach(ActiveEffect activeEffect in activeEffectList)
        {
            GameObject go = activeEffect.onSelf ? user : target;
            int hit;
            if (activeEffect.noMiss && hitParam == 0) hit = 1;
            else if (activeEffect.critOnly && hitParam != 2) hit = 0;
            else hit = hitParam;

            switch(hit)
            {
                case 0 :
                    MissFx();
                    break;
                case 1 :
                    HitFx();
                    break;
                case 2 :
                    CritFx();
                    break;
                default:
                    break;
            }

            switch(activeEffect.activeType)
            {
                case ActiveType.Damage:
                    Damage(go,activeEffect, hit);
                    break;
                case ActiveType.Heal:
                    Heal(go,activeEffect, hit);
                    break;
                case ActiveType.Burn:
                    Burn(go,activeEffect, hit);
                    break;
                case ActiveType.Stun:
                    Stun(go,activeEffect, hit);
                    break;
                case ActiveType.Freeze:
                    Freeze(go,activeEffect, hit);
                    break;
                case ActiveType.Poison:
                    Poison(go,activeEffect, hit);
                    break;
                case ActiveType.Armor:
                    Armor(go,activeEffect, hit);
                    break;
                case ActiveType.Cure:
                    Cure(go, hit);
                    break;
                case ActiveType.Splash:
                    //todo
                    break;
                case ActiveType.SacredMace:
                    SacredMace(user, go, activeEffect, hit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void CacAtkFx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("CaCAction");
    }
    
    private void RangeAtkFx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("RangeAction");
    }
    
    private void SelfAtkFx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("SelfAction");
    }
    
    private void MissFx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("Miss");
    }
    
    private void HitFx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("Hit");
    }
    
    private void CritFx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("Critical");
    }

    private void Damage(GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnDamageGiven)
                {
                    passive.Effect(target);
                }
                break;
            case 2:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnDamageGiven)
                {
                    passive.Effect(target, true);
                }
                break;
        }
        target.GetComponent<CombatStat>().TakeDamage(hit * activeEffect.value);
    }
    
    private void Heal(GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnHealGiven)
                {
                    passive.Effect(target);
                }
                break;
            case 2:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnHealGiven)
                {
                    passive.Effect(target, true);
                }
                break;
        }
        target.GetComponent<CombatStat>().TakeHeal(hit*activeEffect.value);
    }

    private void Burn(GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Burn, activeEffect.value);
                break;
            case 2:
                 passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target, true);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Burn, activeEffect.value*2);
                break;
        }
    }
    
    private void Freeze(GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Freeze, activeEffect.value);
                break;
            case 2:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target, true);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Freeze, activeEffect.value*2);
                break;
        }
    }
    
    private void Poison(GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Poison, activeEffect.value);
                break;
            case 2:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target, true);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Poison, activeEffect.value*2);
                break;
        }
    }
    
    private void Stun(GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Stun, activeEffect.value);
                break;
            case 2:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target, true);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Stun, activeEffect.value*2);
                break;
        }
    }
    
    private void Armor(GameObject target, ActiveEffect activeEffect, int hit)
    {
        switch(hit)
        {
            case 0:
                break;
            case 1:
                target.GetComponent<CombatStat>().ChangeArmor(activeEffect.value);
                break;
            case 2:
                target.GetComponent<CombatStat>().ChangeArmor(activeEffect.value*2);
                break;
        }
    }

    private void Cure(GameObject target, int hit)
    {
        switch(hit)
        {
            case 0:
                break;
            case 1 or 2:
                target.GetComponent<CombatStat>().ResetStatus();
                break;
        }
    }

    private void SacredMace(GameObject user, GameObject target, ActiveEffect activeEffect, int hit)
    {
        if(!user.CompareTag(target.tag))
            Damage(target,activeEffect,hit);
        else
        {
            ActiveEffect tempAE = new ActiveEffect(activeEffect.activeType, activeEffect.value / 2, activeEffect.onSelf,
                activeEffect.noMiss, activeEffect.critOnly);
            Heal(target, tempAE, hit);
        }
    }

    public int GetCd()
    {
        return cd;
    }

    public int GetAtkRange()
    {
        return range;
    }

    public ActiveTarget GetActiveTarget()
    {
        return clickTarget;
    }
}
