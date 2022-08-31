using UnityEngine;

/// <summary>
/// Contains the Grid Information for a Single Room.
/// </summary>
public class LevelGrid : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float gridCellSize;
    [SerializeField, HideInInspector] GridCellState[,] gridCellStates;

    public int GridWidth { get => gridWidth; }
    public int GridHeight { get => gridHeight; }
    public float GridCellSize { get => gridCellSize; }
    public GridCellState[,] GridCellStates { get => gridCellStates; }

    private void Awake()
    {
        gridCellStates = new GridCellState[gridWidth, gridHeight];
        for (int i = 0; i < gridCellStates.GetLength(0); ++i)
        {
            for (int j = 0; j < gridCellStates.GetLength(1); ++j)
            {
                gridCellStates[i, j] = GridCellState.Walkable;
            }
        }
        GridSystem.RegisterLevelGrid(this);
    }

#if false
    public void Reset()
    {
        gridCellStates = new GridCellState[gridWidth, gridHeight];
        for (int i = 0; i < gridCellStates.GetLength(0); ++i)
        {
            for (int j = 0; j < gridCellStates.GetLength(1); ++j)
            {
                gridCellStates[i, j] = GridCellState.Walkable;
            }
        }

        gridCellStates[0, 0] = GridCellState.Walkable;

        GridSystem.RegisterLevelGrid(this);
    }
#endif

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return (
            gridPosition.X > -1 &&
            gridPosition.X < gridWidth &&
            gridPosition.Z > -1 &&
            gridPosition.Z < gridHeight);
    }

    public void SetGridCellState(GridPosition gridPosition, GridCellState gridCellState)
    {
        if (IsValidGridPosition(gridPosition))
        {
            gridCellStates[gridPosition.X, gridPosition.Z] = gridCellState;
        }
    }

    public bool TryGetGridCellState(GridPosition gridPosition, out GridCellState gridCellState)
    {
        if (IsValidGridPosition(gridPosition))
        {
            gridCellState = gridCellStates[gridPosition.X, gridPosition.Z];

            return true;
        }

        gridCellState = GridCellState.Impassable;

        return false;
    }
}
