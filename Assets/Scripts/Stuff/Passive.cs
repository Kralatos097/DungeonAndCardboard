using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PassiveCard", order = 4)]
public class Passive : Stuff
{
    public override void Effect(GameObject user)
    {
        throw new System.NotImplementedException();
    }

    public override void Effect(GameObject user, int touchStatus)
    {
        throw new System.NotImplementedException();
    }

    public override void Effect(GameObject user, GameObject target, int hit)
    {
        throw new System.NotImplementedException();
    }
}
