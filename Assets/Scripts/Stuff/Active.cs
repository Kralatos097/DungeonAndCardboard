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
        foreach(ActiveEffect activeEffect in activeEffectList)
        {
            var gameObject = activeEffect.onSelf ? user : target;
            int hit;
            if (activeEffect.noMiss && hitParam == 0) hit = 1;
            else if (activeEffect.critOnly && hitParam != 2) hit = 0;
            else hit = hitParam;

            switch(activeEffect.activeType)
            {
                case ActiveType.Damage:
                    Damage(gameObject,activeEffect, hit);
                    break;
                case ActiveType.Heal:
                    Heal(gameObject,activeEffect, hit);
                    break;
                case ActiveType.Burn:
                    Burn(gameObject,activeEffect, hit);
                    break;
                case ActiveType.Stun:
                    Stun(gameObject,activeEffect, hit);
                    break;
                case ActiveType.Freeze:
                    Freeze(gameObject,activeEffect, hit);
                    break;
                case ActiveType.Poison:
                    Poison(gameObject,activeEffect, hit);
                    break;
                case ActiveType.Armor:
                    Armor(gameObject,activeEffect, hit);
                    break;
                case ActiveType.Cure:
                    Cure(gameObject, hit);
                    break;
                case ActiveType.Splash:
                    //todo
                    break;
                case ActiveType.TwoHit:
                    //todo
                    break;
                case ActiveType.ThreeHit:
                    //todo
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void Damage(GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                AtkMiss();
                break;
            case 1:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnDamageGiven)
                {
                    passive.Effect(target);
                }
                AtkHit();
                break;
            case 2:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnDamageGiven)
                {
                    passive.Effect(target, true);
                }
                AtkCritical();
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
                AtkMiss();
                break;
            case 1:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnHealGiven)
                {
                    passive.Effect(target);
                }
                AtkHit();
                break;
            case 2:
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnHealGiven)
                {
                    passive.Effect(target, true);
                }
                AtkCritical();
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
                AtkMiss();
                break;
            case 1:
                AtkHit();
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Burn, activeEffect.value);
                break;
            case 2:
                AtkCritical();
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
                AtkMiss();
                break;
            case 1:
                AtkHit();
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Freeze, activeEffect.value);
                break;
            case 2:
                AtkCritical();
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
                AtkMiss();
                break;
            case 1:
                AtkHit();
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Poison, activeEffect.value);
                break;
            case 2:
                AtkCritical();
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
                AtkMiss();
                break;
            case 1:
                AtkHit();
                passive = target.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Stun, activeEffect.value);
                break;
            case 2:
                AtkCritical();
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
                AtkMiss();
                break;
            case 1:
                AtkHit();
                target.GetComponent<CombatStat>().ChangeArmor(activeEffect.value);
                break;
            case 2:
                AtkCritical();
                target.GetComponent<CombatStat>().ChangeArmor(activeEffect.value*2);
                break;
        }
    }

    private void Cure(GameObject target, int hit)
    {
        switch(hit)
        {
            case 0:
                AtkMiss();
                break;
            case 1 or 2:
                AtkHit();
                target.GetComponent<CombatStat>().ResetStatus();
                break;
        }
    }

    private void AtkMiss()
    {
        
    }

    private void AtkHit()
    {
        
    }

    private void AtkCritical()
    {
        
    }

    public int GetCd()
    {
        return cd;
    }

    public int GetAtkRange()
    {
        return range;
    }
}
