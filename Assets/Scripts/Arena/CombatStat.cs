using System;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CombatStat : MonoBehaviour
{
    [SerializeField] private GameObject popUpDamagePrefab;
    
    private int _initiative;

    [HideInInspector] public int baseMaxHp;

    public bool maxHpUpgraded = false;

    private int _maxHp;
    public int MaxHp
    {
        get => _maxHp;

        set
        {
            _maxHp = value;
            if(isUp)
            {
                if(_maxHp > _currHp)
                {
                    _currHp = _maxHp;
                }
            }
            if(_maxHp < 0)
            {
                _maxHp = 0;
            }

            isAlive = _maxHp > 0;

            if(!isAlive)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.position = new Vector3(-100, -100, -100);
                
                if(!isUp) //empeche aux effets de ce lancer au début de la scene
                    AllieDeathFX();
            }
        }
    }

    protected int _currHp;
    public virtual int CurrHp
    {
        get => _currHp ;
        set
        {
            if(value < _currHp)
            {
                if (_currHp <= 0)
                {
                    MaxHp-=1;
                    return;
                }
                if (holyShield)
                {
                    holyShield = false;
                    DamageHolyShieldFX();
                }
                else
                {
                    if (armor > 0)
                    {
                        armor -= (_currHp - value);
                        TakeArmorDamageFX();
                        if (armor < 0)
                        {
                            TakeDamageFX();
                            _currHp += armor;
                            armor = 0;
                        }
                    }
                    else
                    {
                        if(TurnManager.CombatStarted) TakeDamageFX();
                        _currHp = value;
                    }

                    if (_currHp <= 0)
                    {
                        if (_revive)
                        {
                            Debug.Log("Revived!");
                            _currHp = _reviveValue;
                            _revive = false;
                            _reviveValue = 0;
                        }
                        else
                        {
                            _currHp = 0;
                            isUp = false;
                            UnitDeath();
                        }
                    }
                }
            }
            else if(value > _currHp)
            {
                _currHp = value;
                if(TurnManager.CombatStarted) GetHealFX();
                if(_currHp > MaxHp)
                {
                    _currHp = MaxHp;
                }

                if (_currHp > 0 && !isUp)
                {
                    ReviveUnit();
                    isUp = true;
                }
            }
        }
    }

    private int armor = 0;
    private bool holyShield = false;
    private bool _revive = false;
    private int _reviveValue = 0;
    protected StatusEffect StatusEffect = StatusEffect.Nothing;
    protected int statusValue = 0;
    public virtual int StatusValue
    {
        get => statusValue;
        protected set
        {
            statusValue = value;
            if(statusValue <= 0)
            {
                ResetStatus();
            }
        }
    }

    [HideInInspector] public int currInit;

    [HideInInspector] public bool isUp = true;
    [HideInInspector] public bool isAlive = true;
    
    [HideInInspector] public int lastAtkReceivedInfoValue;
    [HideInInspector] public int lastAtkReceivedInfoIsCrit; //0 -> miss, 1 -> hit, 2 -> crit

    public void RollInit()
    {
        currInit = _initiative + Random.Range(1,7);
    }

    protected virtual void UnitDeath()
    {
        Passive passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
        if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnDeath)
        {
            passive.Effect(gameObject);
            if (isUp)
            {
                return;
            }
        }

        if(gameObject.CompareTag("Player"))
        {
            ResetStatus();
            transform.GetChild(0).GetComponent<Renderer>().material.color = Color.grey;
            MaxHp--;
            AllieDownFX();
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.position = new Vector3(-100, -100, -100);
            EnemyDeathFX();
            isAlive = false;
        }
    }

    private void ReviveUnit()
    {
        transform.GetChild(0).GetComponent<Renderer>().material.color = Color.white;
    }

    public void TakeDamage(int value, int hit)
    {
        InstantiatePopUpDamage(value*hit, hit, false);
        if (value * hit == 0) return;
        Passive passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
        if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnDamageTaken && hit != 0)
        {
            passive.Effect(gameObject);
        }
        
        CurrHp-=value * hit;
        
        Debug.Log(name+" is damaged for : " + value*hit + " remaining " + CurrHp +"/"+ MaxHp);
    }

    public void TakeDamagePassive(int value)
    {
        CurrHp-=value;
    }
    
    public void TakeHeal(int value, int hit)
    {
        InstantiatePopUpDamage(value*hit, hit, true);
        if (value * hit == 0) return;
        Passive passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
        if(passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnHealTaken && hit != 0)
        {
            passive.Effect(gameObject);
        }
        
        CurrHp+=value * hit;
        
        Debug.Log(name+" is healed for : " + value*hit + " remaining " + CurrHp +"/"+ MaxHp);
    }
    
    public void TakeHealPassive(int value)
    {
        if (value == 0) return;
        CurrHp+=value;
    }

    public void ChangeStatus(StatusEffect effect, int value)
    {
        FindObjectOfType<FXManager>().StopAll(transform);

        if(!isUp) return; //todo: a test dans le doute a clean
        
        Passive passive;
        switch(effect)
        {
            case StatusEffect.Poison:
                GetPoisonFX();
                if(StatusEffect == effect) StatusValue += value;
                else
                {
                    StatusValue = value;
                    StatusEffect = effect;
                }
                
                passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
                if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatueTaken)
                {
                    passive.Effect(gameObject);
                }
                break;
            case StatusEffect.Stun:
                GetStunFX();
                if(StatusEffect == effect) StatusValue += value;
                else
                {
                    StatusValue = value;
                    StatusEffect = effect;
                }

                StatusEffect = effect;
                StatusValue = value;
                passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
                if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatueTaken)
                {
                    passive.Effect(gameObject);
                }
                break;
            case StatusEffect.Burn:
                GetBurnFX();
                if(StatusEffect == effect) StatusValue += value;
                else
                {
                    StatusValue = value;
                    StatusEffect = effect;
                }

                StatusEffect = effect;
                StatusValue = value;
                passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
                if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatueTaken)
                {
                    passive.Effect(gameObject);
                }
                break;
            case StatusEffect.Freeze:
                GetFreezeFX();
                if(StatusEffect == effect) StatusValue += value;
                else
                {
                    StatusValue = value;
                    StatusEffect = effect;
                }

                StatusEffect = effect;
                StatusValue = value;
                passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
                if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatueTaken)
                {
                    passive.Effect(gameObject);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
        }
    }
    
    public int GetArmor()
    {
        return armor;
    }

    public void ChangeArmor(int value)
    {
        if (CurrHp <= 0) return;
        gameObject.GetComponent<CombatStat>().armor += value;
        GainArmorFX();
    }

    public void ActivatePoison()
    {
        TakeDamage(1, 1);
        ActivatePoisonFX();
        StatusValue--;
        if(StatusEffect == StatusEffect.Nothing)
        {
            FindObjectOfType<FXManager>().Stop("Poison", transform);
        }
    }
    
    public void ActivateBurn()
    {
        TakeDamage(StatusValue, 1);
        ActivateBurnFX();
        StatusValue = 0;
    }

    public void GetCured()
    {
        ResetStatus();
        GetCuredFX();
    }
    
    public void ResetStatus()
    {
        FindObjectOfType<FXManager>().StopAll(transform);
        Passive passive = gameObject.GetComponent<TacticsMovement>().GetPassive();
        if (passive != null && passive.GetPassiveTrigger() == PassiveTrigger.OnStatueClean)
        {
            passive.Effect(gameObject);
        }
        
        StatusEffect = StatusEffect.Nothing;
        statusValue = 0;
    }

    public int GetInit()
    {
        return _initiative;
    }

    public void ChangeInit(int value)
    {
        _initiative = value;
    }

    public StatusEffect GetStatusEffect()
    {
        return StatusEffect;
    }
    
    public int GetStatusValue()
    {
        return StatusValue;
    }

    public void ActivateHolyShield()
    {
        if (CurrHp <= 0) return;
        
        holyShield = true;
        FindObjectOfType<AudioManager>().RandomPitch("GainHolyShield");
        //Todo: Add VFX
    }

    public void ActivateRevive(int value)
    {
        _revive = true;
        _reviveValue = value;
    }

    private void DamageHolyShieldFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("DamageHolyShield");
        //Todo: Add VFX
    }

    private void TakeArmorDamageFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("ArmorDamaged");
        //Todo: Add VFX
    }
    
    private void GainArmorFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("ArmorGained");
        //Todo: Add VFX
    }

    private void TakeDamageFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch(gameObject.CompareTag("Player") ? "AllieDamaged" : "EnemyDamaged");
        FindObjectOfType<FXManager>().Play("Damaged", transform);
    }

    private void GetHealFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch(gameObject.CompareTag("Player") ? "AllieHealed" : "EnemyHealed");
        FindObjectOfType<FXManager>().Play("Healed", transform);
    }

    private void InstantiatePopUpDamage(int value, int hit, bool isHeal)
    {
        Vector3 inst = new Vector3(transform.position.x, popUpDamagePrefab.transform.position.y, transform.position.z);
        GameObject pop = Instantiate(popUpDamagePrefab, inst, Quaternion.identity);

        
        pop.transform.GetChild(0).GetChild(3).gameObject.SetActive(!isHeal);
        pop.transform.GetChild(0).GetChild(4).gameObject.SetActive(isHeal);
        switch (hit)
        {
            case 0:
                pop.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                pop.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                pop.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                pop.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
                pop.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
                break;
            case 1:
                pop.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                pop.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                pop.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                break;
            case 2:
                pop.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                pop.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                pop.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                break;
            default:
                break;
        }
        
        pop.transform.GetChild(0).GetChild(isHeal ? 4 : 3).GetComponent<TextMeshProUGUI>().text = value.ToString();
    }
    
    private void GetReviveFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("Revive");
        //Todo: Add VFX
    }

    private void GetBurnFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("GetBurn");
        FindObjectOfType<FXManager>().Play("GetBurn", transform);
        FindObjectOfType<FXManager>().Play("Burn", transform);
    }

    private void GetFreezeFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("GetFreeze");
        FindObjectOfType<FXManager>().Play("GetFreeze", transform);
        FindObjectOfType<FXManager>().Play("Freeze", transform);
    }

    private void GetStunFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("GetStun");
        FindObjectOfType<FXManager>().Play("GetStun", transform);
        FindObjectOfType<FXManager>().Play("Stun", transform);
    }

    private void GetPoisonFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("GetPoison");
        FindObjectOfType<FXManager>().Play("GetPoison", transform);
        FindObjectOfType<FXManager>().Play("Poison", transform);
    }
    
    private void GetCuredFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("GetCured");
        FindObjectOfType<FXManager>().Play("Cured", transform);
    }

    private void ActivatePoisonFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("ActivatePoison");
        FindObjectOfType<FXManager>().Play("GetPoison", transform);
    }
    
    private void ActivateBurnFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("ActivateBurn");
        FindObjectOfType<FXManager>().Play("GetBurn", transform);
    }

    private void AllieDownFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("AllieDown");
        FindObjectOfType<FXManager>().Play("Dead", transform);
    }

    private void AllieDeathFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("AllieDeath");
        FindObjectOfType<FXManager>().Play("Dead", transform);
    }

    private void EnemyDeathFX()
    {
        FindObjectOfType<AudioManager>().RandomPitch("EnemyDeath");
        FindObjectOfType<FXManager>().Play("Dead", transform);
    }
    
    //todo: Add more FX
    
    //-------------- TEST FUNCTION -------------
#if UNITY_EDITOR
    [ButtonMethod]
    [ContextMenu("Kill Unit")]
    public void KillUnit()
    {
        TakeDamage(100, 1);
    }
    
    [ContextMenu("Heal Unit")]
    public void HealUnit()
    {
        TakeHeal(100, 1);
    }
    
    [ContextMenu("Armor Unit")]
    public void ArmorTest()
    {;
        ChangeArmor(1);
    }
    
    [ContextMenu("Burn Unit")]
    public void BurnTest()
    {
        ChangeStatus(StatusEffect.Burn, 15);
    }
    
    [ContextMenu("Poison Unit")]
    public void PoisonTest()
    {
        ChangeStatus(StatusEffect.Poison, 2);
    }
    
    [ContextMenu("Stun Unit")]
    public void StunTest()
    {
        ChangeStatus(StatusEffect.Stun, 2);
    }
    
    [ContextMenu("Freeze Unit")]
    public void FreezeTest()
    {
        ChangeStatus(StatusEffect.Freeze, 5);
    }
    
    [ContextMenu("Cure Unit")]
    public void CureTest()
    {
        GetCured();
    }
#endif
}
