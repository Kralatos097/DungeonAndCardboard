using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemieCard", order = 2)]
public class EnemieBaseInfo : ScriptableObject
{
    [MinMaxSlider(0, 100)]
    public Vector2Int maxHp;
    public int initiative;
    public int movement;
    public Active activeOne;
    public Active activeTwo;
    public Passive passive;
    public Consumable consumable;
    public IaType iaType;
    
    public Sprite charaUiIcon;
}
