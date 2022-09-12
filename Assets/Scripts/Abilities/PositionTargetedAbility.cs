using Units;

namespace Abilities
{
    public abstract class PositionTargetedAbility : AbilityBase
    {
        public abstract int UpdateRange(Unit owningUnit);
        public abstract bool Use(Unit owningUnit, Grid.GridPosition targetPosition);

        public override void Use(Unit owningUnit)
        {
            var range = UpdateRange(owningUnit);
            owningUnit.Controller.TargetAbility(owningUnit, this, range);
        }
    }
}
