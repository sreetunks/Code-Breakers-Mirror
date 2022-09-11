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

        [SerializeField] private float waitDurationBetweenUnitTurns = 0.8f;

        [SerializeField] private List<Unit> controlledUnits;

        [SerializeField] UnitMoveAbility moveAbility;

        private List<Unit>.Enumerator _unitEnumerator;

        private void Awake()
        {
            foreach (var controlledUnit in controlledUnits)
                controlledUnit.Controller = this;
        }

        public override void BeginTurn()
        {
            _unitEnumerator = controlledUnits.GetEnumerator();
            if (!_unitEnumerator.MoveNext()) return;
            var controlledUnit = _unitEnumerator.Current;
            if (controlledUnit != null) controlledUnit.BeginTurn(); // Comparison to Null is Expensive
            ContinueTurn();
        }

        private void ContinueTurn()
        {
            var controlledUnit = _unitEnumerator.Current;
            var playerCharacter = PlayerScript.PlayerCharacter;
            if (controlledUnit == null) return; // Comparison to Null is Expensive
            var distanceToPlayer = Mathf.Abs(playerCharacter.Position.X - controlledUnit.Position.X) + Mathf.Abs(playerCharacter.Position.Z - controlledUnit.Position.Z);
            if (distanceToPlayer > 1 && controlledUnit.CurrentAP > 0)
            {
                controlledUnit.OnUnitMoveFinished += OnControlledUnitMoveFinished;
                moveAbility.Use(controlledUnit);
            }
            else
                StartCoroutine(DelayedEndTurn());
        }

        private void OnControlledUnitMoveFinished()
        {
            var controlledUnit = _unitEnumerator.Current;
            if (controlledUnit != null) controlledUnit.OnUnitMoveFinished -= OnControlledUnitMoveFinished; // Comparison to Null is Expensive
            ContinueTurn();
        }

        private IEnumerator DelayedEndTurn()
        {
            yield return new WaitForSeconds(waitDurationBetweenUnitTurns);

            if (_unitEnumerator.MoveNext())
            {
                var controlledUnit = _unitEnumerator.Current;
                if (controlledUnit != null) controlledUnit.BeginTurn(); // Comparison to Null is Expensive
                ContinueTurn();
            }
            else
                TurnOrderSystem.MoveNext();
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
                    if (tempGridCellState is GridCellState.Impassable or GridCellState.Occupied) continue;
                    if (controlledUnit != null) // Comparison to Null is Expensive
                    {
                        var distanceToPosition = Mathf.Abs(tempPosition.X - controlledUnit.Position.X) + Mathf.Abs(tempPosition.Z - controlledUnit.Position.Z);
                        if (distanceToPosition >= currentShortestDistance) continue;
                        currentShortestDistance = distanceToPosition;
                    }

                    targetGridPosition = tempPosition;
                }
            }

            ability.Use(controlledUnit, targetGridPosition);
        }

        public override void TargetAbility(Unit owningUnit, UnitTargetedAbility ability)
        {
        }
    }
}
