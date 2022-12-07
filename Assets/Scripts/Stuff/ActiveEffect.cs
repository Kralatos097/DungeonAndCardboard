using System;

[Serializable]
public class ActiveEffect
{
    public ActiveType activeType;
    public int value;
    public bool onSelf;
    public bool noMiss;
    public bool critOnly;
}
