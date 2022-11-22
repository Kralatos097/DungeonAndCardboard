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
    [SerializeField] private ActiveTarget target;
    
    //[SerializeField] private ActiveTarget activeTarget;

    [Header("Effects")]
    [SerializeField] private List<ActiveEffect> activeEffectList;
    
    public override void Effect(GameObject user)
    {
        foreach(ActiveEffect activeEffect in activeEffectList)
        {
            switch(activeEffect.activeType)
            {
                case ActiveType.Damage:
                    user.GetComponent<CombatStat>().currHp -= activeEffect.value;
                    break;
                case ActiveType.Heal:
                    user.GetComponent<CombatStat>().currHp += activeEffect.value;
                    break;
                case ActiveType.Burn:
                    user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Burn;
                    user.GetComponent<CombatStat>().statusValue = activeEffect.value;
                    break;
                case ActiveType.Stun:
                    user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Stun;
                    user.GetComponent<CombatStat>().statusValue = activeEffect.value;
                    break;
                case ActiveType.Freeze:
                    user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Freeze;
                    user.GetComponent<CombatStat>().statusValue = activeEffect.value;
                    break;
                case ActiveType.Poison:
                    user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Poison;
                    user.GetComponent<CombatStat>().statusValue = activeEffect.value;
                    break;
                case ActiveType.Armor:
                    user.GetComponent<CombatStat>().armor = activeEffect.value;
                    break;
                case ActiveType.Cure:
                    user.GetComponent<CombatStat>().StatusEffect = StatusEffect.Nothing;
                    user.GetComponent<CombatStat>().statusValue = 0;
                    break;
                case ActiveType.CritOnly:
                    //todo
                    break;
                case ActiveType.NoMiss:
                    //todo
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

    private void DamageSelf(GameObject user)
    {
        //todo
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
