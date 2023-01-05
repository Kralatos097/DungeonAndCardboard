using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateCombatState : CombatStat
{
    [SerializeField] private int maxHp;
    
    public override int CurrHp
    {
        get => _currHp;

        set
        {
            _currHp = value;

            if(_currHp > MaxHp)
            {
                _currHp = MaxHp;
            }

            if (_currHp <= 0)
            {
                UnitDeath();
            }
        }
    }

    public override int StatusValue
    {
        get => statusValue;
        set => StatusEffect = StatusEffect.Nothing;
    }
    
    private void Start()
    {
        MaxHp = maxHp;
        CurrHp = maxHp;
    }

    protected override void UnitDeath()
    {
        Destroy(gameObject);
    }
}
