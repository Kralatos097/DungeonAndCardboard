using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : TacticsMovement
{
    private GameObject target;
    private bool _alreadyMoved = false;
    
    [SerializeField] protected PlayerBaseInfo UnitInfo;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    protected override void GetUnitInfo()
    {
        CombatStat combatStat = gameObject.GetComponent<CombatStat>();
        
        combatStat.MaxHp = UnitInfo.maxHp;
        combatStat.currHp = UnitInfo.maxHp;
        combatStat.initiative = UnitInfo.initiative;

        move = UnitInfo.movement;
        equipmentOne = UnitInfo.activeOne;
        equipmentTwo = UnitInfo.activeTwo;
        passif = UnitInfo.passive;
        consummable = UnitInfo.consumable;
    }

    // Update is called once per frame
    void Update()
    {
        if(!Turn) return;
        PlayersTurn = false;

        atkRange = equipmentOne.GetAtkRange();
        bool canAtk = true;

        GameObject temp = AlliesInAttackRange();
        RemoveSelectableTile();
        if (temp != null && !moving)
        {
            Debug.Log("Ennemi Atk !");
            attacking = true;
            Attack(temp, 1);
            return;
        }
        else
        {
            canAtk = false;
        }
        if(!_alreadyMoved && !attacking)
        {
            if (!moving)
            {
                //Début du Soulevement du pion lors du mouvement
                if (!passM)
                {
                    transform.GetChild(0).Translate(0, MoveY, 0);
                    passM = true;
                }

                FindNearestTarget();
                CalculatePath();
                FindSelectableTile();
                actualTargetTile.target = true;
            }
            else
            {
                Move();
                canAtk = true;
            }
        }

        if (_alreadyMoved && !canAtk)
        {
            EndTurnT();
        }
    }

    private void FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        GameObject nearest = null;
        float distance = Mathf.Infinity;

        foreach (GameObject obj in targets)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);

            if (d < distance)
            {
                distance = d;
                nearest = obj;
            }
        }
        
        target = nearest; 
    }

    private void CalculatePath()
    {
        ArenaTile targetTile = GetTargetTile(target);
        FindPath(targetTile);
    }

    protected override void EndOfMovement()
    {
        _alreadyMoved = true;
        base.EndOfMovement();
    }
    
    protected override void EndOfAttack()
    {
        base.EndOfAttack();
        EndTurnT();
    }

    protected void EndTurnT()
    {
        TurnManager.EndTurn();
        attacking = false;
        _alreadyMoved = false;
    }
}
