using System.Collections;
using System.Collections.Generic;
using Grid;
using UnityEngine;

public class Unit : MonoBehaviour, IGridObject
{
    
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private Animator unitAnimator;
    
    private Vector3 _targetPosition;
    private GridPosition _targetGridPosition;
    private List<Vector3> _path;
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking"); // Caching ID for Parameter

    public GridCellState GridCellPreviousState { get; set; }
    public GridPosition Position { get; private set; }
    public bool IsOnDoorGridCell { get; private set; }

    private void Awake()
    {
        _path = new List<Vector3>();
        _targetPosition = transform.position; // Makes sure the unit does not walk to Vector3(0, 0, 0) upon load.
    }

    private void Start()
    {
        var position = transform.position;
        Position = GridSystem.GetGridPosition(position);
        GridCellPreviousState = GridCellState.Impassable;
        GridSystem.UpdateGridObjectPosition(this, Position);
        position = GridSystem.GetWorldPosition(Position);
        transform.position = position;
        _targetPosition = position;
        IsOnDoorGridCell = CheckIsOnDoorGridCell(GridCellPreviousState);
    }

    private void Update()
    {
        if (_path.Count <= 0) return;
        print(_path.Count);
        var toTarget = _path[0] - transform.position;
        var dist = toTarget.magnitude;

        if (dist > 0)
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
            unitAnimator.SetBool(IsWalking, false); // Ends "Walk" Animations

            var newGridPosition = GridSystem.GetGridPosition(transform.position);
            if (newGridPosition == Position) return;
            GridSystem.UpdateGridObjectPosition(this, newGridPosition);
            Position = newGridPosition;
            IsOnDoorGridCell = CheckIsOnDoorGridCell(GridCellPreviousState);
            _path.RemoveAt(0);
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
        if (!forceMove && targetCellState is GridCellState.Impassable or GridCellState.Occupied) return;

        do
        {
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
            if (cellState is GridCellState.Impassable or GridCellState.Occupied)
            {
                newTargetPositionX = targetPosition - GridSystem.GetWorldPosition(gridPositionX);
            }

            // Z Logic
            var gridPositionZ = new GridPosition(position.X, position.Z + deltaZ);
            GridSystem.TryGetGridCellState(gridPositionZ, out cellState);
            var newTargetPositionZ = new Vector3(10000, 10000, 10000);
            if (cellState is GridCellState.Impassable or GridCellState.Occupied)
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
        _targetGridPosition = targetGridPosition;
        _targetPosition = GridSystem.GetWorldPosition(_targetGridPosition);
        if (!forceMove) return;
        Position = targetGridPosition;
        IsOnDoorGridCell = CheckIsOnDoorGridCell(targetCellState);
    }
}