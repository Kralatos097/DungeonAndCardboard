using System;
using System.Collections;
using System.Collections.Generic;
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

    public void Effect(GameObject user, GameObject target, int hitParam)
    {
        GameObject targetf;
        int hit;
        
        foreach(ActiveEffect activeEffect in activeEffectList)
        {
            targetf = activeEffect.onSelf ? user : target;
            if (activeEffect.noMiss && hitParam == 0) hit = 1;
            else if (activeEffect.critOnly && hitParam != 2) hit = 0;
            else hit = hitParam;

                switch(activeEffect.activeType)
            {
                case ActiveType.Damage:
                    Damage(targetf,activeEffect, hit);
                    break;
                case ActiveType.Heal:
                    Heal(targetf,activeEffect, hit);
                    break;
                case ActiveType.Burn:
                    Burn(targetf,activeEffect, hit);
                    break;
                case ActiveType.Stun:
                    Stun(targetf,activeEffect, hit);
                    break;
                case ActiveType.Freeze:
                    Freeze(targetf,activeEffect, hit);
                    break;
                case ActiveType.Poison:
                    Poison(targetf,activeEffect, hit);
                    break;
                case ActiveType.Armor:
                    Armor(targetf,activeEffect, hit);
                    break;
                case ActiveType.Cure:
                    Cure(targetf, hit);
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


    private void Damage(GameObject user, ActiveEffect activeEffect, int hit)
    {
        switch(hit)
        {
            case 0:
                AtkMiss();
                break;
            case 1:
                AtkHit();
                break;
            case 2:
                AtkCritical();
                break;
        }
        user.GetComponent<CombatStat>().CurrHp -= hit * activeEffect.value;
    }
    
    private void Heal(GameObject user, ActiveEffect activeEffect, int hit)
    {
        switch(hit)
        {
            case 0:
                AtkMiss();
                break;
            case 1:
                AtkHit();
                break;
            case 2:
                AtkCritical();
                break;
        }
        user.GetComponent<CombatStat>().CurrHp += hit*activeEffect.value;
    }

    private void Burn(GameObject user, ActiveEffect activeEffect, int hit)
    {
        switch(hit)
        {
            case 0:
                AtkMiss();
                break;
            case 1:
                AtkHit();
                user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Burn;
                user.GetComponent<CombatStat>().StatusValue = activeEffect.value;
                break;
            case 2:
                AtkCritical();
                user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Burn;
                user.GetComponent<CombatStat>().StatusValue = activeEffect.value*2;
                break;
        }
    }
    
    private void Freeze(GameObject user, ActiveEffect activeEffect, int hit)
    {
        switch(hit)
        {
            case 0:
                AtkMiss();
                break;
            case 1:
                AtkHit();
                user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Freeze;
                user.GetComponent<CombatStat>().StatusValue = activeEffect.value;
                break;
            case 2:
                AtkCritical();
                user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Freeze;
                user.GetComponent<CombatStat>().StatusValue = activeEffect.value*2;
                break;
        }
    }
    
    private void Poison(GameObject user, ActiveEffect activeEffect, int hit)
    {
        switch(hit)
        {
            case 0:
                AtkMiss();
                break;
            case 1:
                AtkHit();
                user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Poison;
                user.GetComponent<CombatStat>().StatusValue = activeEffect.value;
                break;
            case 2:
                AtkCritical();
                user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Poison;
                user.GetComponent<CombatStat>().StatusValue = activeEffect.value*2;
                break;
        }
    }
    
    private void Stun(GameObject user, ActiveEffect activeEffect, int hit)
    {
        switch(hit)
        {
            case 0:
                AtkMiss();
                break;
            case 1:
                AtkHit();
                user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Stun;
                user.GetComponent<CombatStat>().StatusValue = activeEffect.value;
                break;
            case 2:
                AtkCritical();
                user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Stun;
                user.GetComponent<CombatStat>().StatusValue = activeEffect.value*2;
                break;
        }
    }
    
    private void Armor(GameObject user, ActiveEffect activeEffect, int hit)
    {
        switch(hit)
        {
            case 0:
                AtkMiss();
                break;
            case 1:
                AtkHit();
                user.GetComponent<CombatStat>().armor = activeEffect.value;
                break;
            case 2:
                AtkCritical();
                user.GetComponent<CombatStat>().armor = activeEffect.value*2;
                break;
        }
    }

    private void Cure(GameObject user, int hit)
    {
        switch(hit)
        {
            case 0:
                AtkMiss();
                break;
            case 1 or 2:
                AtkHit();
                user.GetComponent<CombatStat>().ResetStatus();
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
