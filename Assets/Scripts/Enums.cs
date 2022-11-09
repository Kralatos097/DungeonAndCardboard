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
    Status,
}

public enum ActiveTarget
{
    Self,
    Other,
    Tile,
}
