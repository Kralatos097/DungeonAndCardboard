using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PassiveCard", order = 4)]
public class Passive : Stuff
{
    [SerializeField] private PassiveType passiveType;
    
    
}