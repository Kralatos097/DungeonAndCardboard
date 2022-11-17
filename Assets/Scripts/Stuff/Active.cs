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
                    break;
                case ActiveType.Heal:
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
