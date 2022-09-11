using System.Collections.Generic;
using UnityEngine;
using Grid;

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Move")]
public class UnitMoveAbility : PositionTargetedAbility
{
    [SerializeField] private int apCostPerTile = 1;

    public override int CooldownDuration => 0;

    public override int ActionPointCost => 1;

    public override int UpdateRange(Unit owningUnit)
    {
        return (owningUnit.CurrentAP / apCostPerTile);
    }

    public override bool Use(Unit owningUnit, GridPosition targetPosition)
    {
        List<GridPosition> movePath = new List<GridPosition>();
        GridSystem.GetPath(owningUnit.Position, targetPosition, ref movePath);
        if (movePath.Count == 0) return false;

        var start = owningUnit.Position;
        var end = movePath[movePath.Count - 1];

        var distanceMoved = Mathf.Abs(end.X - start.X) + Mathf.Abs(end.Z - start.Z);
        var range = (owningUnit.CurrentAP / apCostPerTile);
        if (distanceMoved > range)
        {
            movePath.RemoveRange(range, movePath.Count - range);
            distanceMoved = range;
        }

        owningUnit.ConsumeAP(distanceMoved * apCostPerTile);
        owningUnit.Move(movePath);

        return true;
    }
}
