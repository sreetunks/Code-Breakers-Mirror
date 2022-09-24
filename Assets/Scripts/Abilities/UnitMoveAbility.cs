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
        [SerializeField, TextArea(5, 10)] private string description;

        public override int CooldownDuration => 0;

        public override int ActionPointCost => apCostPerTile;

        public override string ToolTipCooldownString => "Cooldown - N/A";

        public override string ToolTipAPCostString => $"AP Cost - {apCostPerTile} per Tile Moved";

        public override string ToolTipDescriptionString => description;

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
            var range = (owningUnit.CurrentAP / apCostPerTile);

            var movePath = new List<GridPosition>();
            PathFinding.GetPath(owningUnit.Position, targetPosition, ref movePath);
            var distanceMoved = movePath.Count;

            if (distanceMoved > range)
            {
                if (owningUnit.Controller.Faction == Controller.FactionType.Player)
                    return false;

                movePath.RemoveRange(range, movePath.Count - range);
                distanceMoved = range;
            }

            if (distanceMoved == 0)
                return false;

            owningUnit.ConsumeAP(distanceMoved * apCostPerTile);
            owningUnit.Move(movePath);

            return true;
        }
    }
}
