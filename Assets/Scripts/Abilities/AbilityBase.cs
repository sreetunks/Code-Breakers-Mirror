using Units;
using UnityEngine;

namespace Abilities
{
    public abstract class AbilityBase : ScriptableObject
    {
        public Sprite HUDIconSprite { get; }
        public abstract int ActionPointCost { get; }
        public abstract int CooldownDuration { get; }

        public abstract string ToolTipDescriptionString { get; }
        public abstract string ToolTipCooldownString { get; }
        public abstract string ToolTipAPCostString { get; }

        public abstract void Use(Unit owningUnit);
    }
}
