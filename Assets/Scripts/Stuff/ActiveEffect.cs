using System;

[Serializable]
public class ActiveEffect
{
    public ActiveType activeType;
    public int value;
    public bool onSelf;
    public bool noMiss;
    public bool critOnly;

    public ActiveEffect(ActiveType activeType,  int value, bool onSelf, bool noMiss, bool critOnly)
    {
        this.activeType = activeType;
        this.value = value;
        this.onSelf = onSelf;
        this.noMiss = noMiss;
        this.critOnly = critOnly;
    }
}
