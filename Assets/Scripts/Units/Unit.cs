using System.Collections.Generic;
using UnityEngine;
using Grid;

public class Unit : MonoBehaviour, IGridObject, IDamageable
{
    public delegate void OnUnitDeathEventHandler();
    public OnUnitDeathEventHandler OnUnitDeath;

    public delegate void OnUnitDamagedEventHandler(int damageDealt);
    public OnUnitDamagedEventHandler OnUnitDamaged;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private Animator unitAnimator;

    [SerializeField] private int maximumHealth = 4;

    private List<Vector3> _path;

    private int _currentHealth;

    private static readonly int IsWalking = Animator.StringToHash("IsWalking"); // Caching ID for Parameter

    public Controller Controller { get; set; }

    public GridCellState GridCellPreviousState { get; set; }
    public GridPosition Position { get; private set; }
    public bool IsOnDoorGridCell { get; private set; }

    public int MaximumHealth => maximumHealth;
    public int CurrentHealth => _currentHealth;

    private void Awake()
    {
        _currentHealth = maximumHealth;
        _path = new List<Vector3>();
    }

    private void Start()
    {
        Position = GridSystem.GetGridPosition(transform.position);
        GridCellPreviousState = GridCellState.Impassable;
        GridSystem.UpdateGridObjectPosition(this, Position);
        transform.position = GridSystem.GetWorldPosition(Position);
        IsOnDoorGridCell = CheckIsOnDoorGridCell(GridCellPreviousState);
    }

    private void Update()
    {
        UpdateMove();
    }

    void UpdateMove()
    {
        if (_path.Count <= 0) return;
        var toTarget = _path[0] - transform.position;
        var dist = toTarget.magnitude;

        if (dist > 0.01)
        {
            var move = moveSpeed * Time.deltaTime * toTarget.normalized; // Makes move speed the same for all frame-rate's

            if (move.magnitude > dist)
                move = toTarget;

            transform.position += move; // Sets target move location, EXACTLY (L: 15 - 26)
            unitAnimator.SetBool(IsWalking, true); // Starts "Walk" Animations

            var rotation = Quaternion.LookRotation(toTarget);
            var current = transform.localRotation;
            transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime * rotateSpeed); // Sets Rotation to be more Accurate (L: 34 - 36) for 180 Degree's
        }
        else
        {
            var newGridPosition = GridSystem.GetGridPosition(transform.position);
            if (newGridPosition == Position) return;
            GridSystem.UpdateGridObjectPosition(this, newGridPosition);
            Position = newGridPosition;
            IsOnDoorGridCell = CheckIsOnDoorGridCell(GridCellPreviousState);
            _path.RemoveAt(0);

            if (_path.Count == 0) unitAnimator.SetBool(IsWalking, false); // Ends "Walk" Animations^M
        }
    }

    private bool CheckIsOnDoorGridCell(GridCellState gridCellState)
    {
        return (gridCellState == GridCellState.DoorNorth ||
                gridCellState == GridCellState.DoorEast ||
                gridCellState == GridCellState.DoorSouth ||
                gridCellState == GridCellState.DoorWest);
    }

    public void Move(Vector3 targetPosition, bool forceMove = false)
    {
        var position = Position;
        var targetGridPosition = GridSystem.GetGridPosition(targetPosition);

        GridSystem.TryGetGridCellState(targetGridPosition, out var targetCellState);
        if (forceMove)
        {
            Position = targetGridPosition;
            IsOnDoorGridCell = CheckIsOnDoorGridCell(targetCellState);

            return;
        }
        else if (targetCellState is GridCellState.Impassable or GridCellState.Occupied)
            return;

        int numIterations = 0;
        do
        {
            if (numIterations++ > 32) return;
            // TODO: Implement Dijkstra Pathfinding
            // Currently just moves in a simplistic way

            //Pathfinding Logic
            var deltaX = targetGridPosition.X - position.X;
            var deltaZ = targetGridPosition.Z - position.Z;

            if (deltaX != 0) deltaX /= Mathf.Abs(deltaX);
            if (deltaZ != 0) deltaZ /= Mathf.Abs(deltaZ);

            // X Logic
            var gridPositionX = new GridPosition(position.X + deltaX, position.Z);
            GridSystem.TryGetGridCellState(gridPositionX, out var cellState);
            var newTargetPositionX = new Vector3(10000, 10000, 10000);
            if (cellState != GridCellState.Impassable && cellState != GridCellState.Occupied)
            {
                newTargetPositionX = targetPosition - GridSystem.GetWorldPosition(gridPositionX);
            }

            // Z Logic
            var gridPositionZ = new GridPosition(position.X, position.Z + deltaZ);
            GridSystem.TryGetGridCellState(gridPositionZ, out cellState);
            var newTargetPositionZ = new Vector3(10000, 10000, 10000);
            if (cellState != GridCellState.Impassable && cellState != GridCellState.Occupied)
            {
                newTargetPositionZ = targetPosition - GridSystem.GetWorldPosition(gridPositionZ);
            }

            if (newTargetPositionX == new Vector3(10000, 10000, 10000) && newTargetPositionZ == new Vector3(10000, 10000, 10000)) break; // NOTE: Simple path not found, this is where we expand the logic to find a path.

            // Compare Logic
            position = newTargetPositionX.magnitude < newTargetPositionZ.magnitude ? gridPositionX : gridPositionZ;
            _path.Add(GridSystem.GetWorldPosition(position));

            // TODO: Get 4 Way Pathfinding to work
        }
        while (position != targetGridPosition);

        // Move Logic - TODO: Update to use Pathfinding
    }

    public void TakeDamage(int damageDealt)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - damageDealt);
        OnUnitDamaged?.Invoke(damageDealt);
        if (_currentHealth == 0)
            OnUnitDeath?.Invoke();
    }

    public void Heal(int healthRestored)
    {
        _currentHealth = Mathf.Min(maximumHealth, _currentHealth + healthRestored);
        OnUnitDamaged?.Invoke(-healthRestored);
    }
}