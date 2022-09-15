using System.Collections;
using System.Collections.Generic;
using Abilities;
using Grid;
using UnityEngine;


namespace Units
{
    public class WiperEnemyController : Controller
    {
        public override FactionType Faction => FactionType.Enemy;

        public override bool IsActive => controlledUnits.Count > 0;

        [SerializeField] private float waitDurationBetweenUnitTurns = 0.8f;

        [SerializeField] private List<Unit> controlledUnits;

        [SerializeField] UnitMoveAbility moveAbility;
        [SerializeField] UnitMeleeAttackAbility meleeAbility;

        private List<Unit>.Enumerator _unitEnumerator;

        private void Awake()
        {
            foreach (var controlledUnit in controlledUnits)
            {
                controlledUnit.Controller = this;
                controlledUnit.OnUnitDeath += OnControlledUnitDeath;
            }
        }

        public override void Initialize()
        {
            foreach (var controlledUnit in controlledUnits)
                controlledUnit.Spawn();
        }

        public override void BeginTurn()
        {
            _unitEnumerator = controlledUnits.GetEnumerator();
            if (!_unitEnumerator.MoveNext())
            {
                TurnOrderSystem.DeregisterController(this);
                TurnOrderSystem.MoveNext();
                return;
            }
            var controlledUnit = _unitEnumerator.Current;
            if (controlledUnit != null) controlledUnit.BeginTurn(); // Comparison to Null is Expensive

            StartCoroutine(DelayedContinueTurn());
        }

        private void ContinueTurn()
        {
            print("Continue Turn");
            var controlledUnit = _unitEnumerator.Current;
            var playerCharacter = PlayerScript.PlayerCharacter;
            if (controlledUnit == null) return; // Comparison to Null is Expensive
            var distanceToPlayer = (int) FunctionHelper.PyThag(playerCharacter.Position.X - controlledUnit.Position.X, playerCharacter.Position.Z - controlledUnit.Position.Z);
            if (distanceToPlayer > 1 && controlledUnit.CurrentAP >= moveAbility.ActionPointCost)
            {
                print("Moving");
                controlledUnit.OnUnitActionFinished += OnControlledUnitActionFinished;
                moveAbility.Use(controlledUnit);
            }
            else if (distanceToPlayer == 1 && controlledUnit.CurrentAP >= meleeAbility.ActionPointCost && controlledUnit.GetAbilityCooldown(meleeAbility) == 0)
            {
                print("Attacking");
                controlledUnit.OnUnitActionFinished += OnControlledUnitActionFinished;
                meleeAbility.Use(controlledUnit, playerCharacter);
            }
            else
            {
                if (_unitEnumerator.MoveNext())
                {
                    controlledUnit = _unitEnumerator.Current;
                    if (controlledUnit != null) // Comparison to Null is Expensive
                        controlledUnit.BeginTurn();
                    StartCoroutine(DelayedContinueTurn());
                }
                else
                    TurnOrderSystem.MoveNext();
            }
            print(controlledUnit);
        }

        void OnControlledUnitDeath(Unit unit)
        {
            unit.OnUnitDeath -= OnControlledUnitDeath;
            controlledUnits.Remove(unit);

            if (controlledUnits.Count == 0)
                TurnOrderSystem.DeregisterController(this);
        }

        private void OnControlledUnitActionFinished()
        {
            var controlledUnit = _unitEnumerator.Current;
            if (controlledUnit != null) controlledUnit.OnUnitActionFinished -= OnControlledUnitActionFinished; // Comparison to Null is Expensive
            StartCoroutine(DelayedContinueTurn());
        }

        private IEnumerator DelayedContinueTurn()
        {
            print("Delaying");
            yield return new WaitForSeconds(waitDurationBetweenUnitTurns);

            ContinueTurn();
        }

        public override void TargetAbility(Unit owningUnit, PositionTargetedAbility ability, int range)
        {
            var controlledUnit = _unitEnumerator.Current;
            if (ability != moveAbility) return;
            var playerCharacter = PlayerScript.PlayerCharacter;

            var targetGridPosition = GridPosition.Invalid;
            var currentShortestDistance = int.MaxValue;
            for (var x = -1; x < 2; ++x)
            {
                for (var y = -1; y < 2; ++y)
                {
                    var tempPosition = new GridPosition(playerCharacter.Position.X + x, playerCharacter.Position.Z + y);
                    if (!GridSystem.TryGetGridCellState(tempPosition, out var tempGridCellState)) continue;
                    if (tempGridCellState is GridCellState.Impassable or GridCellState.Occupied or GridCellState.OccupiedEnemy) continue;
                    if (controlledUnit != null) // Comparison to Null is Expensive
                    {
                        var distanceToPosition = Mathf.Max(Mathf.Abs(tempPosition.X - controlledUnit.Position.X), Mathf.Abs(tempPosition.Z - controlledUnit.Position.Z));
                        if (distanceToPosition >= currentShortestDistance) continue;
                        currentShortestDistance = distanceToPosition;
                    }

                    targetGridPosition = tempPosition;
                }
            }

            GridSystem.ResetGridRangeInfo();
            if (!ability.Use(controlledUnit, targetGridPosition))
            {
                controlledUnit.ConsumeAP(controlledUnit.CurrentAP); // HACK: We need to handle this with better path-finding failcases.
                controlledUnit.OnAbilityUsed(ability);
            }
        }

        public override void TargetAbility(Unit owningUnit, UnitTargetedAbility ability, int range)
        {

        }
    }
}
