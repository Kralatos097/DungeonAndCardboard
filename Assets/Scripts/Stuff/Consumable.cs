using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ConsumableCard", order = 3)]
public class Consumable : Active
{
    public override void OnEnable()
    {
        base.OnEnable();
        stuffType = "Consumable";
    }
}
