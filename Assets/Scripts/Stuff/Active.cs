using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ActiveCard", order = 2)]
public class Active : Stuff
{
    [SerializeField] private ActiveTarget activeTarget;

    [SerializeField] private List<ActiveEffect> activeEffectList;

    public override void Effect(GameObject user)
    {
        foreach(ActiveEffect activeEffect in activeEffectList)
        {
            switch(activeEffect.activeType)
            {
                case ActiveType.Damage:
                    break;
                case ActiveType.Heal:
                    break;
                case ActiveType.Status:
                    switch (activeEffect.statusEffect)
                    {
                        case StatusEffect.Poison:
                            break;
                        case StatusEffect.Stun:
                            break;
                        case StatusEffect.Burn:
                            break;
                        case StatusEffect.Freeze:
                            break;
                        case StatusEffect.Nothing:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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
}
