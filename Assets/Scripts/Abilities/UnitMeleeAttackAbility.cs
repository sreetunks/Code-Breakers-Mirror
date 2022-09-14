using Units;
using Grid;
using UnityEngine;
using Unity.Collections;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Melee Attack")]
    public class UnitMeleeAttackAbility : UnitTargetedAbility
    {
        [SerializeField] int attackDamage;
        [SerializeField] int cooldownDuration;
        [SerializeField] int actionPointCost;
        [SerializeField] int range = 1;

        public override int CooldownDuration => cooldownDuration;

        public override int ActionPointCost => actionPointCost;

        public override int UpdateRange(Unit owningUnit)
        {
            var levelGrid = GridSystem.ActiveLevelGrid;
            var cellCount = levelGrid.GridWidth * levelGrid.GridHeight;
            var cellRangeArray = new NativeArray<float>(cellCount, Allocator.Temp);
            for (var y = 0; y < levelGrid.GridHeight; ++y)
            {
                for (var x = 0; x < levelGrid.GridWidth; ++x)
                {
                    var idx = (y * levelGrid.GridWidth) + x;
                    var distanceToTarget = Mathf.Abs(x - owningUnit.Position.X) + Mathf.Abs(y - owningUnit.Position.Z);
                    cellRangeArray[idx] = (distanceToTarget > range) ? 0 : distanceToTarget;
                }
            }

            GridSystem.UpdateGridRangeInfo(cellRangeArray);
            return range;
        }

        public override bool Use(Unit owningUnit, Unit targetUnit)
        {
            var distanceToTarget = Mathf.Abs(targetUnit.Position.X - owningUnit.Position.X) + Mathf.Abs(targetUnit.Position.Z - owningUnit.Position.Z);
            if (distanceToTarget > range || targetUnit.Controller.Faction == Controller.FactionType.Player)
                return false;

            targetUnit.TakeDamage(attackDamage);
            owningUnit.ConsumeAP(actionPointCost);
            owningUnit.OnAbilityUsed(this);

            return true;
        }
    }
}