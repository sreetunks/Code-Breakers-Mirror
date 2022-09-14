using Units;

namespace Abilities
{
    public abstract class UnitTargetedAbility : AbilityBase
    {
        public abstract int UpdateRange(Unit owningUnit);
        public abstract bool Use(Unit owningUnit, Unit targetUnit);

        public override void Use(Unit owningUnit)
        {
            var range = UpdateRange(owningUnit);
            owningUnit.Controller.TargetAbility(owningUnit, this, range);
        }
    }
}
