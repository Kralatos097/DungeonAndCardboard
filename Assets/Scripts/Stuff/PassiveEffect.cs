using System;
using UnityEngine.Serialization;

[Serializable]
public class PassiveEffect
{
        public int value;
        public bool onEveryAllies;
        public PassiveType passiveType;

        public PassiveEffect(int value, bool onEveryAllies, PassiveType passiveType)
        {
                this.value = value;
                this.onEveryAllies = onEveryAllies;
                this.passiveType = passiveType;
        }
}
