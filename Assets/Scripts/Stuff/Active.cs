using System;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ActiveCard", order = 2)]
public class Active : Stuff
{
    [SerializeField] private int range;
    [SerializeField] private int cd; //0 == pas de CD
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
                SelfAtkFx(user);
                break;
            case 1:
                CacAtkFx(user);
                break;
            case > 1:
                RangeAtkFx(user);
                break;
            default:
                Debug.LogWarning("Out of range");
                break;
        }

        bool pass = false;
        foreach(ActiveEffect activeEffect in activeEffectList)
        {
            GameObject go = activeEffect.onSelf ? user : target;
            int hit;
            if (activeEffect.noMiss && hitParam == 0) hit = 1;
            else if (activeEffect.critOnly && hitParam != 2) hit = 0;
            else if (activeEffect.critOnly && hitParam == 2) hit = 1;
            else hit = hitParam;
            
            if(!pass)
            {
                pass = true;
                switch (hit)
                {
                    case 0:
                        MissFx();
                        if(go.GetComponent<CombatStat>() != null)
                            go.GetComponent<CombatStat>().InstantiatePopUpDamage(0,0,false);
                        break;
                    case 1:
                        HitFx();
                        break;
                    case 2:
                        CritFx();
                        break;
                    default:
                        break;
                }
            }

            switch(activeEffect.activeType)
            {
                case ActiveType.Damage:
                    Damage(user, go,activeEffect, hit);
                    break;
                case ActiveType.Heal:
                    Heal(user, go,activeEffect, hit);
                    break;
                case ActiveType.Burn:
                    Burn(user, go,activeEffect, hit);
                    break;
                case ActiveType.Stun:
                    Stun(user, go,activeEffect, hit);
                    break;
                case ActiveType.Freeze:
                    Freeze(user, go,activeEffect, hit);
                    break;
                case ActiveType.Poison:
                    Poison(user, go,activeEffect, hit);
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
                case ActiveType.RandomPotionEffect:
                    RandomPotionEffect(user, activeEffect, hit);
                    break;
                case ActiveType.ElementaryPouet:
                    ElementaryPouet(user, go, activeEffect, hit);
                    break;
                case ActiveType.RandomStatusEffect:
                    RandomStatusEffect(go, activeEffect, hit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void ElementaryPouet(GameObject user, GameObject target, ActiveEffect activeEffect, int hit)
    {
        StatusEffect statusEffect = target.GetComponent<CombatStat>().GetStatusEffect();
        int statusValue = target.GetComponent<CombatStat>().GetStatusValue();

        activeEffect.value = statusValue * 2;

        switch (statusEffect)
        {
            case StatusEffect.Nothing:
                break;
            case StatusEffect.Poison:
                Poison(user, target, activeEffect, hit);
                break;
            case StatusEffect.Stun:
                Stun(user, target, activeEffect, hit);
                break;
            case StatusEffect.Burn:
                Burn(user, target, activeEffect, hit);
                break;
            case StatusEffect.Freeze:
                Freeze(user, target, activeEffect, hit);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CacAtkFx(GameObject user)
    {
        user.GetComponentInChildren<Animator>().SetTrigger("Attack");
        FindObjectOfType<AudioManager>().RandomPitch("CaCAction");
    }
    
    private void RangeAtkFx(GameObject user)
    {
        user.GetComponentInChildren<Animator>().SetTrigger("Distance");
        FindObjectOfType<AudioManager>().RandomPitch("RangeAction");
    }
    
    private void SelfAtkFx(GameObject user)
    {
        user.GetComponentInChildren<Animator>().SetTrigger("Distance");
        FindObjectOfType<AudioManager>().RandomPitch("SelfAction");
    }
    
    private void MissFx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("Miss");
    }

    private void HitFx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("Hit");
        CameraShaker.Instance.ShakeOnce(1f, 3f, .1f, 0.4f);
    }

    private void CritFx()
    {
        FindObjectOfType<AudioManager>().RandomPitch("Critical");
        CameraShaker.Instance.ShakeOnce(1f, 10f, .1f,1f);
    }

    private void Damage(GameObject user, GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnDamageGiven)
                {
                    passive.Effect(target);
                }
                break;
            case 2:
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnDamageGiven)
                {
                    passive.Effect(target, true);
                }
                break;
        }
        target.GetComponent<CombatStat>().TakeDamage(activeEffect.value, hit);
    }
    
    private void Heal(GameObject user, GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnHealGiven)
                {
                    passive.Effect(target);
                }
                break;
            case 2:
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnHealGiven)
                {
                    passive.Effect(target, true);
                }
                break;
        }
        target.GetComponent<CombatStat>().TakeHeal(activeEffect.value, hit);
    }

    private void Burn(GameObject user, GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Burn, activeEffect.value);
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                break;
            case 2:
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Burn, activeEffect.value*2);
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target, true);
                }
                break;
        }
    }
    
    private void Freeze(GameObject user, GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Freeze, activeEffect.value);
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                break;
            case 2:
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Freeze, activeEffect.value*2);
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target, true);
                }
                break;
        }
    }
    
    private void Poison(GameObject user, GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Poison, activeEffect.value);
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                break;
            case 2:
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Poison, activeEffect.value*2);
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target, true);
                }
                break;
        }
    }
    
    private void Stun(GameObject user, GameObject target, ActiveEffect activeEffect, int hit)
    {
        Passive passive;
        switch(hit)
        {
            case 0:
                break;
            case 1:
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Stun, activeEffect.value);
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target);
                }
                break;
            case 2:
                target.GetComponent<CombatStat>().ChangeStatus(StatusEffect.Stun, activeEffect.value*2);
                passive = user.GetComponent<TacticsMovement>().GetPassive();
                if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatusGiven)
                {
                    passive.Effect(target, true);
                }
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
                target.GetComponent<CombatStat>().GetCured();
                break;
        }
    }

    private void SacredMace(GameObject user, GameObject target, ActiveEffect activeEffect, int hit)
    {
        if(!user.CompareTag(target.tag))
            Damage(user, target,activeEffect,hit);
        else
        {
            ActiveEffect tempAE = new ActiveEffect(activeEffect.activeType, activeEffect.value / 2, activeEffect.onSelf,
                activeEffect.noMiss, activeEffect.critOnly);
            Heal(user, target, tempAE, hit);
        }
    }

    private void RandomPotionEffect(GameObject user, ActiveEffect activeEffect, int hit)
    {
        int rand = Random.Range(0, 6);

        switch (rand)
        {
            case 0: //Burn
                Burn(user, user, activeEffect, hit);
                break;
            case 1: //Freeze
                Freeze(user, user, activeEffect, hit);
                break;
            case 2: //Poison
                Poison(user, user, activeEffect, hit);
                break;
            case 3: //Heal
                Heal(user, user, activeEffect, hit);
                break;
            case 4: //Armor
                Armor(user, activeEffect, hit);
                break;
            case 5: //Cure
                Cure(user, hit);
                break;
            default:
                break;
        }
    }
    
    private void RandomStatusEffect(GameObject user, ActiveEffect activeEffect, int hit)
    {
        int rand = Random.Range(0, 4);

        switch (rand)
        {
            case 0: //Burn
                Burn(user, user, activeEffect, hit);
                break;
            case 1: //Freeze
                Freeze(user, user, activeEffect, hit);
                break;
            case 2: //Poison
                Poison(user, user, activeEffect, hit);
                break;
            case 3: //Stun
                Stun(user, user, activeEffect, hit);
                break;
            default:
                break;
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
