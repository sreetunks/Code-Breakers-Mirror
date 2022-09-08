using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public enum FactionType
    {
        Neutral,
        Player,
        Enemy
    }

    public abstract FactionType Faction { get; }

    public abstract void TargetAbility(Unit owningUnit, PositionTargetedAbility ability);
    public abstract void TargetAbility(Unit owningUnit, UnitTargetedAbility ability);

    public abstract void BeginTurn();
}
