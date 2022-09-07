using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    // NOTE: We might add more states to deal with special hazards cells and similar behaviour
    public enum GridCellState
    {
        Impassable,
        Walkable,
        Occupied,
        DoorNorth,
        DoorEast,
        DoorSouth,
        DoorWest
    }

    public class GridSystem : MonoBehaviour
    {
        private const float CellSize = 2.0f;

        [SerializeField] private LevelGrid startingLevelGrid;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private Dictionary<GridPosition, IGridObject> _gridObjectMap;

        private LevelGrid _activeLevelGrid;

        public static LevelGrid ActiveLevelGrid { get => Instance?._activeLevelGrid; }

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
            if (Instance == null) Instance = FindObjectOfType<GridSystem>();
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
            ActiveLevelGrid.SetGridCellState(newGridPosition, GridCellState.Occupied);
        }

        public static GridPosition SwitchLevelGrid(GridPosition doorGridCellPosition, GridCellState doorGridCellState)
        {
            LevelGrid newLevelGrid = ActiveLevelGrid.GetRoomAdjacentToDoor(doorGridCellState);
            GridPosition adjacentGridDoorPosition = GridPosition.Invalid;
            if (newLevelGrid)
            {
                Instance._gridObjectMap.Remove(doorGridCellPosition);
                ActiveLevelGrid.SetGridCellState(doorGridCellPosition, doorGridCellState);
                RegisterLevelGrid(newLevelGrid);

                if (doorGridCellState == GridCellState.DoorNorth)
                    adjacentGridDoorPosition = newLevelGrid.DoorSouth;
                else if (doorGridCellState == GridCellState.DoorEast)
                    adjacentGridDoorPosition = newLevelGrid.DoorWest;
                else if (doorGridCellState == GridCellState.DoorSouth)
                    adjacentGridDoorPosition = newLevelGrid.DoorNorth;
                else if (doorGridCellState == GridCellState.DoorWest)
                    adjacentGridDoorPosition = newLevelGrid.DoorEast;
            }

            return adjacentGridDoorPosition;
        }
    }
}