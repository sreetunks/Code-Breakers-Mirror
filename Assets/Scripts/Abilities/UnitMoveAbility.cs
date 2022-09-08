using UnityEngine;
using Grid;

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Move")]
public class UnitMoveAbility : PositionTargetedAbility
{
    public override int CooldownDuration => 0;

    public override int ActionPointCost => 0;

    public override bool Use(Unit owningUnit, GridPosition targetPosition)
    {
        owningUnit.Move(GridSystem.GetWorldPosition(targetPosition));

        return true;
    }
}
