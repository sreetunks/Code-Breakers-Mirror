using System.Collections.Generic;
using Grid;
using Units;
using Unity.Collections;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Move")]
    public class UnitMoveAbility : PositionTargetedAbility
    {
        [SerializeField] private int apCostPerTile = 1;

        public override int CooldownDuration => 0;

        public override int ActionPointCost => 1;

        public override int UpdateRange(Unit owningUnit)
        {
            var range = (owningUnit.CurrentAP / apCostPerTile);
            List<GridPosition> pathList = new List<GridPosition>();

            var levelGrid = GridSystem.ActiveLevelGrid;
            var cellCount = levelGrid.GridWidth * levelGrid.GridHeight;
            var cellRangeArray = new NativeArray<float>(cellCount, Allocator.Temp);
            for (var y = 0; y < levelGrid.GridHeight; ++y)
            {
                for (var x = 0; x < levelGrid.GridWidth; ++x)
                {
                    var idx = (y * levelGrid.GridWidth) + x;
                    PathFinding.GetPath(owningUnit.Position, new GridPosition(x, y), ref pathList);
                    cellRangeArray[idx] = (pathList.Count > range) ? 0 : pathList.Count;
                }
            }

            GridSystem.UpdateGridRangeInfo(cellRangeArray);
            return range;
        }

        public override bool Use(Unit owningUnit, GridPosition targetPosition)
        {
            var movePath = new List<GridPosition>();
            PathFinding.GetPath(owningUnit.Position, targetPosition, ref movePath);
            if (movePath.Count == 0) return false;

            var start = owningUnit.Position;
            var end = movePath[^1];

            var distanceMoved = movePath.Count;
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
}