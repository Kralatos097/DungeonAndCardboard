using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PassiveCard", order = 4)]
public class Passive : Stuff
{
    [SerializeField] private PassiveTrigger passiveTrigger;
    [Header("Effects")]
    [SerializeField] private List<PassiveEffect> passiveEffectList;
    
    public void Effect(GameObject user)
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
                DoEffect(passiveEffect, user);
            }
        }
    }
    
    public int Effect(GameObject user, int hit)
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
                hit = DoEffect(passiveEffect, user, hit);
            }
        }

        return hit;
    }

    private void DoEffect(PassiveEffect passiveEffect, GameObject user)
    {
        CombatStat userCombatStat = user.GetComponent<CombatStat>();
        TacticsMovement userTacticsMovement = user.GetComponent<TacticsMovement>();
        switch (passiveEffect.passiveType)
        {
            case PassiveType.Damage:
                userCombatStat.CurrHp += passiveEffect.value;
                break;
            case PassiveType.Heal:
                userCombatStat.CurrHp += passiveEffect.value;
                break;
            case PassiveType.GainHolyShield:
                userCombatStat.holyShield = true;
                break;
            case PassiveType.ChangeArmor:
                userCombatStat.armor += passiveEffect.value;
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
                break;
            case PassiveType.GainStun:
                userCombatStat.StatusEffect = StatusEffect.Stun;
                userCombatStat.StatusValue = passiveEffect.value;
                break;
            case PassiveType.GainBurn:
                userCombatStat.StatusEffect = StatusEffect.Burn;
                userCombatStat.StatusValue = passiveEffect.value;
                break;
            case PassiveType.GainPoison:
                userCombatStat.StatusEffect = StatusEffect.Poison;
                userCombatStat.StatusValue = passiveEffect.value;
                break;
            case PassiveType.GainFreeze:
                userCombatStat.StatusEffect = StatusEffect.Freeze;
                userCombatStat.StatusValue = passiveEffect.value;
                break;
            case PassiveType.RemoveStun:
                if (userCombatStat.StatusEffect == StatusEffect.Stun)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemoveBurn:
                if (userCombatStat.StatusEffect == StatusEffect.Burn)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemovePoison:
                if (userCombatStat.StatusEffect == StatusEffect.Poison)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemoveFreeze:
                if (userCombatStat.StatusEffect == StatusEffect.Freeze)
                    userCombatStat.ResetStatus();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private int DoEffect(PassiveEffect passiveEffect, GameObject user, int hit)
    {
        CombatStat userCombatStat = user.GetComponent<CombatStat>();
        TacticsMovement userTacticsMovement = user.GetComponent<TacticsMovement>();
        switch (passiveEffect.passiveType)
        {
            case PassiveType.Damage:
                userCombatStat.CurrHp += passiveEffect.value;
                break;
            case PassiveType.Heal:
                userCombatStat.CurrHp += passiveEffect.value;
                break;
            case PassiveType.GainHolyShield:
                userCombatStat.holyShield = true;
                break;
            case PassiveType.ChangeArmor:
                userCombatStat.armor += passiveEffect.value;
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
                if (hit == 0) hit = 1;
                break;
            case PassiveType.GainStun:
                userCombatStat.StatusEffect = StatusEffect.Stun;
                userCombatStat.StatusValue = passiveEffect.value;
                break;
            case PassiveType.GainBurn:
                userCombatStat.StatusEffect = StatusEffect.Burn;
                userCombatStat.StatusValue = passiveEffect.value;
                break;
            case PassiveType.GainPoison:
                userCombatStat.StatusEffect = StatusEffect.Poison;
                userCombatStat.StatusValue = passiveEffect.value;
                break;
            case PassiveType.GainFreeze:
                userCombatStat.StatusEffect = StatusEffect.Freeze;
                userCombatStat.StatusValue = passiveEffect.value;
                break;
            case PassiveType.RemoveStun:
                if (userCombatStat.StatusEffect == StatusEffect.Stun)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemoveBurn:
                if (userCombatStat.StatusEffect == StatusEffect.Burn)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemovePoison:
                if (userCombatStat.StatusEffect == StatusEffect.Poison)
                    userCombatStat.ResetStatus();
                break;
            case PassiveType.RemoveFreeze:
                if (userCombatStat.StatusEffect == StatusEffect.Freeze)
                    userCombatStat.ResetStatus();
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