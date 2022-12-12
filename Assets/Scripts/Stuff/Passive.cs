using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PassiveCard", order = 4)]
public class Passive : Stuff
{
    [SerializeField] private PassiveTrigger passiveTrigger;
    [Header("Effects")]
    [SerializeField] private List<PassiveEffect> passiveEffectList;

    public override void OnEnable()
    {
        base.OnEnable();
        stuffType = "Passive";
    }
    
    public void Effect(GameObject target)
    {

        foreach (PassiveEffect passiveEffect in passiveEffectList)
        {
            if (passiveEffect.onEveryAllies)
            {
                foreach (var playerMovement in TurnManager._playerList)
                {
                    DoEffect(passiveEffect, playerMovement.gameObject);
                }
            }
            else
            {
                DoEffect(passiveEffect, target);
            }
        }
    }
    
    public void Effect(GameObject target, bool crit)
    {

        foreach (PassiveEffect passiveEffect in passiveEffectList)
        {
            if (passiveEffect.onEveryAllies)
            {
                foreach (var playerMovement in TurnManager._playerList)
                {
                    DoEffect(passiveEffect, playerMovement.gameObject, crit);
                }
            }
            else
            {
                DoEffect(passiveEffect, target, crit);
            }
        }
    }
    
    public int Effect(GameObject target, int hit)
    {
        foreach (PassiveEffect passiveEffect in passiveEffectList)
        {
            if (passiveEffect.onEveryAllies)
            {
                foreach (var playerMovement in TurnManager._playerList)
                {
                    hit = DoEffect(passiveEffect, playerMovement.gameObject, hit);
                }
            }
            else
            {
                hit = DoEffect(passiveEffect, target, hit);
            }
        }

        return hit;
    }

    private void DoEffect(PassiveEffect passiveEffect, GameObject target)
    {
        CombatStat userCombatStat = target.GetComponent<CombatStat>();
        TacticsMovement userTacticsMovement = target.GetComponent<TacticsMovement>();
        switch (passiveEffect.passiveType)
        {
            case PassiveType.Damage:
                userCombatStat.TakeDamage(passiveEffect.value);
                break;
            case PassiveType.Heal:
                userCombatStat.TakeHeal(passiveEffect.value);
                break;
            case PassiveType.GainHolyShield:
                userCombatStat.ActivateHolyShield();
                break;
            case PassiveType.GainRevive:
                userCombatStat.ActivateRevive(passiveEffect.value);
                break;
            case PassiveType.ChangeArmor:
                userCombatStat.ChangeArmor(passiveEffect.value);
                break;
            case PassiveType.ChangeMovement:
                userTacticsMovement.ChangeMove(passiveEffect.value);
                break;
            case PassiveType.ChangeMaxHp:
                userCombatStat.MaxHp += passiveEffect.value;
                break;
            case PassiveType.ChangeInitiative:
                userCombatStat.currInit += passiveEffect.value;
                break;
            case PassiveType.ChangeRange:
                userTacticsMovement.atkRange += passiveEffect.value;
                break;
            case PassiveType.ReRollDice:
                throw new Exception();
            case PassiveType.GainStun:
                userCombatStat.ChangeStatus(StatusEffect.Stun, 1);
                break;
            case PassiveType.GainBurn:
                userCombatStat.ChangeStatus(StatusEffect.Burn, passiveEffect.value);
                break;
            case PassiveType.GainPoison:
                userCombatStat.ChangeStatus(StatusEffect.Poison, passiveEffect.value);
                break;
            case PassiveType.GainFreeze:
                userCombatStat.ChangeStatus(StatusEffect.Freeze, passiveEffect.value);
                break;
            case PassiveType.RemoveStun:
                if (userCombatStat.GetStatusEffect() == StatusEffect.Stun)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemoveBurn:
                if (userCombatStat.GetStatusEffect() == StatusEffect.Burn)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemovePoison:
                if (userCombatStat.GetStatusEffect() == StatusEffect.Poison)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemoveFreeze:
                if (userCombatStat.GetStatusEffect() == StatusEffect.Freeze)
                    userCombatStat.ResetStatus();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void DoEffect(PassiveEffect passiveEffect, GameObject target, bool crit)
    {
        CombatStat userCombatStat = target.GetComponent<CombatStat>();
        TacticsMovement userTacticsMovement = target.GetComponent<TacticsMovement>();

        int effectValue = passiveEffect.value;
        if (crit) effectValue *= 2;
        
        switch (passiveEffect.passiveType)
        {
            case PassiveType.Damage:
                userCombatStat.TakeDamage(effectValue);
                break;
            case PassiveType.Heal:
                userCombatStat.TakeHeal(effectValue);
                break;
            case PassiveType.ChangeArmor:
                userCombatStat.ChangeArmor(effectValue);
                break;
            case PassiveType.ChangeMovement:
                userTacticsMovement.ChangeMove(effectValue);
                break;
            case PassiveType.ChangeMaxHp:
                userCombatStat.MaxHp += effectValue;
                break;
            case PassiveType.ChangeInitiative:
                userCombatStat.currInit += effectValue;
                break;
            case PassiveType.ChangeRange:
                userTacticsMovement.atkRange += effectValue;
                break;
            case PassiveType.ReRollDice:
                throw new Exception();
            case PassiveType.GainStun:
                userCombatStat.ChangeStatus(StatusEffect.Stun, 1);
                break;
            case PassiveType.GainBurn:
                userCombatStat.ChangeStatus(StatusEffect.Burn, effectValue);
                break;
            case PassiveType.GainPoison:
                userCombatStat.ChangeStatus(StatusEffect.Poison, effectValue);
                break;
            case PassiveType.GainFreeze:
                userCombatStat.ChangeStatus(StatusEffect.Freeze, effectValue);
                break;
            case PassiveType.RemoveStun:
                if (userCombatStat.GetStatusEffect() == StatusEffect.Stun)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemoveBurn:
                if (userCombatStat.GetStatusEffect() == StatusEffect.Burn)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemovePoison:
                if (userCombatStat.GetStatusEffect() == StatusEffect.Poison)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemoveFreeze:
                if (userCombatStat.GetStatusEffect() == StatusEffect.Freeze)
                    userCombatStat.ResetStatus();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private int DoEffect(PassiveEffect passiveEffect, GameObject target, int hit)
    {
        switch (passiveEffect.passiveType)
        {
            case PassiveType.ReRollDice:
                if (hit == 0)
                {
                    hit = Random.Range(1,7);
                    return hit switch
                    {
                        1 => 0,
                        6 => 2,
                        _ => 1
                    };
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return hit;
    }

    public PassiveTrigger GetPassiveTrigger()
    {
        return passiveTrigger;
    }
}