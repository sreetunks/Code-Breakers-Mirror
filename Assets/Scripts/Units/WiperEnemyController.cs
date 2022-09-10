using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Grid;

public class WiperEnemyController : Controller
{
    public override FactionType Faction => FactionType.Enemy;

    [SerializeField] float waitDurationBetweenUnitTurns = 0.8f;

    [SerializeField] private List<Unit> controlledUnits;

    [SerializeField] UnitMoveAbility moveAbility;

    List<Unit>.Enumerator unitEnumerator;

    private void Awake()
    {
        foreach (var controlledUnit in controlledUnits)
            controlledUnit.Controller = this;
    }

    public override void BeginTurn()
    {
        unitEnumerator = controlledUnits.GetEnumerator();
        if (unitEnumerator.MoveNext())
        {
            var controlledUnit = unitEnumerator.Current;
            controlledUnit.BeginTurn();
            ContinueTurn();
        }
    }

    void ContinueTurn()
    {
        var controlledUnit = unitEnumerator.Current;
        var playerCharacter = PlayerScript.PlayerCharacter;
        var distanceToPlayer = Mathf.Max(Mathf.Abs(playerCharacter.Position.X - controlledUnit.Position.X), Mathf.Abs(playerCharacter.Position.Z - controlledUnit.Position.Z));
        if (distanceToPlayer > 1 && controlledUnit.CurrentAP > 0)
        {
            controlledUnit.OnUnitMoveFinished += OnControlledUnitMoveFinished;
            moveAbility.Use(controlledUnit);
        }
        else
            StartCoroutine(DelayedEndTurn());
    }

    void OnControlledUnitMoveFinished()
    {
        var controlledUnit = unitEnumerator.Current;
        controlledUnit.OnUnitMoveFinished -= OnControlledUnitMoveFinished;
        ContinueTurn();
    }

    IEnumerator DelayedEndTurn()
    {
        yield return new WaitForSeconds(waitDurationBetweenUnitTurns);

        if (unitEnumerator.MoveNext())
        {
            var controlledUnit = unitEnumerator.Current;
            controlledUnit.BeginTurn();
            ContinueTurn();
        }
        else
            TurnOrderSystem.MoveNext();
    }

    public override void TargetAbility(Unit owningUnit, PositionTargetedAbility ability, int range)
    {
        var controlledUnit = unitEnumerator.Current;
        if (ability == moveAbility)
        {
            var playerCharacter = PlayerScript.PlayerCharacter;

            GridPosition targetGridPosition = GridPosition.Invalid;
            int currentShortestDistance = int.MaxValue;
            for (var x = -1; x < 2; ++x)
            {
                for (var y = -1; y < 2; ++y)
                {
                    GridPosition tempPosition = new GridPosition(playerCharacter.Position.X + x, playerCharacter.Position.Z + y);
                    if (GridSystem.TryGetGridCellState(tempPosition, out var tempGridCellState))
                    {
                        if (tempGridCellState == GridCellState.Impassable || tempGridCellState == GridCellState.Occupied) continue;
                        var distanceToPosition = Mathf.Max(Mathf.Abs(tempPosition.X - controlledUnit.Position.X), Mathf.Abs(tempPosition.Z - controlledUnit.Position.Z));
                        if (distanceToPosition < currentShortestDistance)
                        {
                            currentShortestDistance = distanceToPosition;
                            targetGridPosition = tempPosition;
                        }
                    }
                }
            }
        }
    }

    public override void TargetAbility(Unit owningUnit, UnitTargetedAbility ability)
    {
    }
}
