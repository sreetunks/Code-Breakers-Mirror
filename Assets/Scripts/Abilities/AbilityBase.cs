using UnityEngine;

public abstract class AbilityBase : ScriptableObject
{
    public Sprite HUDIconSprite { get; }
    public abstract int ActionPointCost { get; }
    public abstract int CooldownDuration { get; }

    public abstract void Use(Unit owningUnit);
}
