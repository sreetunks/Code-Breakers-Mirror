using System.Collections.Generic;
using UnityEngine;
using Units;
using Unity.Collections;

namespace Grid
{
    // NOTE: We might add more states to deal with special hazards cells and similar behaviour
    public enum GridCellState
    {
        Impassable,
        Walkable,
        Occupied,
        OccupiedEnemy,
        DoorNorth,
        DoorEast,
        DoorSouth,
        DoorWest,
        LevelExit
    }

    public class GridSystem : MonoBehaviour
    {
        [SerializeField] private LevelGrid startingLevelGrid;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private Dictionary<GridPosition, IGridObject> _gridObjectMap;

        private LevelGrid _activeLevelGrid;

        public static LevelGrid ActiveLevelGrid { get => Instance?._activeLevelGrid; }
        public static GridPosition HighlightPosition { get; set; } = GridPosition.Invalid;
        public static int HighlightRange { get; set; } = -1;

        private static GridSystem Instance { get; set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one GridSystem! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }

            Instance = this;

            RegisterLevelGrid(startingLevelGrid);
        }

        /// <summary>
        /// Called on <c>LevelGrid.Awake()</c>.
        /// Registers a Room containing a LevelGrid object.
        /// </summary>
        public static void RegisterLevelGrid(LevelGrid levelGrid)
        {
#if UNITY_EDITOR
            if (Instance == null) Instance = FindObjectOfType<GridSystem>(); // Comparison with null & Find Object Of Type is considered Expensive
#endif
            Instance._activeLevelGrid = levelGrid;
            Instance._gridObjectMap = new Dictionary<GridPosition, IGridObject>(levelGrid.GridWidth * levelGrid.GridHeight);
        }

        public static Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return ActiveLevelGrid.GridOffset + (new Vector3(gridPosition.X + 0.5f, 0, gridPosition.Z + 0.5f) * Instance._activeLevelGrid.GridCellSize);
        }

        public static GridPosition GetGridPosition(Vector3 worldPosition)
        {
            worldPosition -= ActiveLevelGrid.GridOffset;
            return new GridPosition(
                Mathf.FloorToInt(worldPosition.x / Instance._activeLevelGrid.GridCellSize),
                Mathf.FloorToInt(worldPosition.z / Instance._activeLevelGrid.GridCellSize)
            );
        }

        public static bool TryGetGridCellState(GridPosition gridPosition, out GridCellState gridCellState)
        {
            return Instance._activeLevelGrid.TryGetGridCellState(gridPosition, out gridCellState);
        }

        public static void SetGridCellState(GridPosition gridPosition, GridCellState gridCellState)
        {
            Instance._activeLevelGrid.SetGridCellState(gridPosition, gridCellState);
        }

        public static bool TryGetGridObject(GridPosition gridPosition, out IGridObject gridObject)
        {
            return Instance._gridObjectMap.TryGetValue(gridPosition, out gridObject);
        }

        public static bool CheckGridObjectMoveToPosition(IGridObject gridObject, GridPosition targetGridPosition)
        {
            if (TryGetGridCellState(targetGridPosition, out var newGridCellState) && newGridCellState != GridCellState.Impassable)
            {
                return !TryGetGridObject(targetGridPosition, out _);
            }

            return false;
        }

        public static void UpdateGridObjectPosition(IGridObject gridObject, GridPosition newGridPosition)
        {
            if (TryGetGridObject(newGridPosition, out _)) return;

            Instance._gridObjectMap.Remove(gridObject.Position);
            Instance._gridObjectMap[newGridPosition] = gridObject;

            ActiveLevelGrid.TryGetGridCellState(newGridPosition, out GridCellState newGridCellState);
            ActiveLevelGrid.SetGridCellState(gridObject.Position, gridObject.GridCellPreviousState);
            gridObject.GridCellPreviousState = newGridCellState;
            var gridUnit = gridObject as Unit;
            if (gridUnit != null && gridUnit.Controller.Faction == Controller.FactionType.Enemy)
                ActiveLevelGrid.SetGridCellState(newGridPosition, GridCellState.OccupiedEnemy);
            else
                ActiveLevelGrid.SetGridCellState(newGridPosition, GridCellState.Occupied);
        }

        public static void UpdateGridRangeInfo(NativeArray<float> rangeArray)
        {
            ActiveLevelGrid.UpdateCellRangeInfo(rangeArray);
        }

        public static void ResetGridRangeInfo()
        {
            ActiveLevelGrid.ResetCellRangeInfo();
        }

        public static float GetDistance(GridPosition start, GridPosition end)
        {
            return Mathf.Sqrt(Mathf.Pow(start.X - end.X,2) + Mathf.Pow(start.Z - end.Z, 2));
        }

        public static GridPosition SwitchLevelGrid(GridPosition doorGridCellPosition, GridCellState doorGridCellState)
        {
            var newLevelGrid = ActiveLevelGrid.GetRoomAdjacentToDoor(doorGridCellState);
            var adjacentGridDoorPosition = GridPosition.Invalid;
            if (!newLevelGrid) return adjacentGridDoorPosition;
            Instance._gridObjectMap.Remove(doorGridCellPosition);
            ActiveLevelGrid.SetGridCellState(doorGridCellPosition, doorGridCellState);
            RegisterLevelGrid(newLevelGrid); // Register Level Grid is considered Expensive

            adjacentGridDoorPosition = doorGridCellState switch
            {
                GridCellState.DoorNorth => newLevelGrid.DoorSouth,
                GridCellState.DoorEast => newLevelGrid.DoorWest,
                GridCellState.DoorSouth => newLevelGrid.DoorNorth,
                GridCellState.DoorWest => newLevelGrid.DoorEast,
                _ => adjacentGridDoorPosition
            };

            return adjacentGridDoorPosition;
        }
    }
}
