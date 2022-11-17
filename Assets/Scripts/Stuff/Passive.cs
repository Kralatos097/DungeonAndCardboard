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
}
