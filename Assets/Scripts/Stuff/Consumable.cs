using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ConsumableCard", order = 3)]
public class Consumable : Active
{
    protected override void Start()
    {
        base.Start();
        stuffType = "Consumable";
    }
}
