using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Crate : MonoBehaviour
{
    [SerializeField] private int maxHp;
    
    private int _hp;
    public int CurrHp
    {
        get => _hp;

        set
        {
            _hp = value;

            if (_hp > maxHp) _hp = maxHp;
                
            if(_hp <= 0)
            {
                DestroySelf();
            }
        }
    }
    
    void Start()
    {
        CurrHp = maxHp;
    }

    private void DestroySelf()
    {
        //todo: destroy
    }
}
