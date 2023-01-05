//Dungeon
public enum RoomType
{
    Normal,
    Boss,
    Treasure,
    Fighting,
    FirstRoom,
    Starting,
}

public enum RoomEffect
{
    Default,
    Boss,
    Treasure,
    Fight,
    Rest,
    Loot,
}

public enum LootEffect
{
    Trap,
    Ambush,
    Stuff,
    Consumable,
    Nothing,
    Default,
}

public enum TreasureEffect
{
    Stuff,
    Consumable,
    Default,
}

public enum Action
{
    Default,
    Move,
    Attack,
    Stay,
    Equip,
    CancelMove,
}
public enum StuffSelected
{
    Default,
    EquipOne,
    EquipTwo,
    Consum,
}

public enum Perso
{
    Default,
    Warrior,
    Thief,
    Cleric,
    Wizard,
}

public enum StatusEffect
{
    Nothing,
    Poison,
    Stun,
    Burn,
    Freeze,
}

public enum ActiveType
{
    Damage,
    Heal,
    Burn,
    Stun,
    Freeze,
    Poison,
    Armor,
    Cure,
    Splash,
    TwoHit,
    ThreeHit,
}

public enum PassiveTrigger
{
    OnObtained,
    OnCombatStart,
    OnCombatEnd,
    OnTurnStart,
    OnTurnEnd,
    OnDamageGiven,
    OnDamageTaken,
    OnHealGiven,
    OnHealTaken,
    OnAttack,
    OnStatusGiven,
    OnStatueTaken,
    OnStatueClean,
    OnDeath,
}

public enum PassiveType
{
    Damage,
    Heal,
    GainHolyShield,
    GainRevive,
    ChangeArmor,
    ChangeMovement,
    ChangeMaxHp,
    ChangeInitiative,
    ChangeRange,
    ReRollDice,
    GainStun,
    GainBurn,
    GainPoison,
    GainFreeze,
    RemoveStun,
    RemoveBurn,
    RemovePoison,
    RemoveFreeze,
}

public enum ActiveTarget
{
    SelfOnly, //only the player or the tile he is in
    OthersOnly, //Can target everything in range except for the current tile 
    Everything, //the two other combined
}
