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
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking"); // Caching ID for Parameter

    public GridPosition Position { get; private set; }

    private void Awake()
    {
        _targetPosition = transform.position; // Makes sure the unit does not walk to Vector3(0, 0, 0) upon load.
    }

    private void Start()
    {
        var position = transform.position;
        Position = GridSystem.GetGridPosition(position);
        GridSystem.UpdateGridObjectPosition(this, Position);
        position = GridSystem.GetWorldPosition(Position);
        transform.position = position;
        _targetPosition = position;
    }

    private void Update()
    {
        var toTarget = _targetPosition - transform.position;
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
        }
    }

    public void Move(Vector3 targetPosition)
    {
        var position = Position;
        var targetGridPosition = GridSystem.GetGridPosition(targetPosition);
        
        if (!GridSystem.TryGetGridCellState(targetGridPosition, out var targetCellState) || targetCellState != GridCellState.Walkable) return;
        
        print("Target: " + targetGridPosition); // TODO: Remove
        
        do
        {
            //Pathfinding Logic
            var deltaX = targetGridPosition.X - position.X;
            var deltaZ = targetGridPosition.Z - position.Z;

            if (deltaX != 0) deltaX /= Mathf.Abs(deltaX);
            if (deltaZ != 0) deltaZ /= Mathf.Abs(deltaZ);
            
            // X Logic
            var gridPositionX = new GridPosition(position.X + deltaX, position.Z);
            GridSystem.TryGetGridCellState(gridPositionX, out var cellState);
            var newTpx = new Vector3(10000, 10000, 10000);
            if (cellState == GridCellState.Walkable)
            {
                newTpx = targetPosition - GridSystem.GetWorldPosition(gridPositionX);
            }

            // Z Logic
            var gridPositionZ = new GridPosition(position.X, position.Z + deltaZ);
            GridSystem.TryGetGridCellState(gridPositionZ, out cellState);
            var newTpz = new Vector3(10000, 10000, 10000);
            if (cellState == GridCellState.Walkable)
            {
                newTpz = targetPosition - GridSystem.GetWorldPosition(gridPositionZ);
            }

            if (newTpx == new Vector3(10000, 10000, 10000) && newTpz == new Vector3(10000, 10000, 10000)) break; // NOTE: Simple path not found, this is where we expand the logic to find a path.
            
            // Compare Logic
            position = newTpx.magnitude < newTpz.magnitude ? gridPositionX : gridPositionZ;
            
            print("Position: " + position); // TODO: Remove
        } 
        while (position != targetGridPosition);
        
        // Move Logic
        _targetGridPosition = targetGridPosition;
        _targetPosition = GridSystem.GetWorldPosition(_targetGridPosition);
    }
}