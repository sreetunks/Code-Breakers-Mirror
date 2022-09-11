using Units;

namespace Abilities
{
    public abstract class UnitTargetedAbility : AbilityBase
    {
        public abstract bool Use(Unit owningUnit, Unit targetUnit);

        public override void Use(Unit owningUnit)
        {
            owningUnit.Controller.TargetAbility(owningUnit, this);
        }
    }
}
