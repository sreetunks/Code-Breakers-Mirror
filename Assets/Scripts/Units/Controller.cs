using Abilities;
using UnityEngine;

namespace Units
{
    public abstract class Controller : MonoBehaviour
    {
        public enum FactionType
        {
            Neutral,
            Player,
            Enemy
        }

        public abstract FactionType Faction { get; }

        public abstract void Initialize();

        public abstract void TargetAbility(Unit owningUnit, PositionTargetedAbility ability, int range);
        public abstract void TargetAbility(Unit owningUnit, UnitTargetedAbility ability, int range);

        public abstract void BeginTurn();
    }
}
