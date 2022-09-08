public abstract class PositionTargetedAbility : AbilityBase
{
    public abstract bool Use(Unit owningUnit, Grid.GridPosition targetPosition);

    public override void Use(Unit owningUnit)
    {
        owningUnit.Controller.TargetAbility(owningUnit, this);
    }
}
