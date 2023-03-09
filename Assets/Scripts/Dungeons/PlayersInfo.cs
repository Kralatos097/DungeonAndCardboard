public class WarriorInfo
{
    public static int BaseMaxHp = 0;
    public static int MaxHp = 0;
    private static int _currentHp = 0;
    public static int CurrentHp
    {
        get => _currentHp;

        set
        {
            _currentHp = value;
            if (CurrentHp > MaxHp)
                CurrentHp = MaxHp;
            else if (CurrentHp < 0)
                CurrentHp = 0;
        }
    }

    public static int Armor = 0;
    public static StatusEffect StatusEffect = StatusEffect.Nothing;

    public static int Init = 0;
    public static int Movement = 0;

    public static Active ActiveOne = null;
    public static Active ActiveTwo = null;
    public static Passive Passive = null;
    public static Consumable Consumable = null;
}

public class ThiefInfo
{
    public static int BaseMaxHp = 0;
    public static int MaxHp = 0;
    private static int _currentHp = 0;
    public static int CurrentHp
    {
        get => _currentHp;

        set
        {
            _currentHp = value;
            if (CurrentHp > MaxHp)
                CurrentHp = MaxHp;
            else if (CurrentHp < 0)
                CurrentHp = 0;
        }
    }

    public static int Init = 0;
    public static int Movement = 0;

    public static Active ActiveOne = null;
    public static Active ActiveTwo = null;
    public static Passive Passive = null;
    public static Consumable Consumable = null;
}

public class ClericInfo
{
    public static int BaseMaxHp = 0;
    public static int MaxHp = 0;
    private static int _currentHp = 0;
    public static int CurrentHp
    {
        get => _currentHp;

        set
        {
            _currentHp = value;
            if (CurrentHp > MaxHp)
                CurrentHp = MaxHp;
            else if (CurrentHp < 0)
                CurrentHp = 0;
        }
    }

    public static int Init = 0;
    public static int Movement = 0;

    public static Active ActiveOne = null;
    public static Active ActiveTwo = null;
    public static Passive Passive = null;
    public static Consumable Consumable = null;
}

public class WizardInfo
{
    public static int BaseMaxHp = 0;
    public static int MaxHp = 0;
    private static int _currentHp = 0;
    public static int CurrentHp
    {
        get => _currentHp;

        set
        {
            _currentHp = value;
            if (CurrentHp > MaxHp)
                CurrentHp = MaxHp;
            else if (CurrentHp < 0)
                CurrentHp = 0;
        }
    }

    public static int Init = 0;
    public static int Movement = 0;

    public static Active ActiveOne = null;
    public static Active ActiveTwo = null;
    public static Passive Passive = null;
    public static Consumable Consumable = null;
}

public static class EndGameInfo
{
    public static bool IsVictory;
}
