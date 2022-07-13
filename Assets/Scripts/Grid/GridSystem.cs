using System;
using UnityEngine;


// ReSharper disable once CheckNamespace
public class GridSystem
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private int _width;
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private int _height;
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private float _cellSize;
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private GridObject[,] _gridObjectArray;

    public GridSystem(int width, int height, float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;

        _gridObjectArray = new GridObject[width, height];
        
        for (var x = 0; x < width; x++)
        {
            for (var z = 0; z < height; z++)
            {
                var gridPosition = new GridPosition(x, z);
                _gridObjectArray[x, z] = new GridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.X, 0, gridPosition.Z) * _cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / _cellSize), 
            Mathf.RoundToInt(worldPosition.z / _cellSize)
        );
    }

    public void CreateDebugObject(Transform debugPrefab)
    {
        for (var x = 0; x < _width; x++)
        {
            for (var z = 0; z < _height; z++)
            {
                var gridPosition = new GridPosition(x, z);
                // ReSharper disable once AccessToStaticMemberViaDerivedType
                var debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                var gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    public GridObject GetGridObject(GridPosition gridPosition)
    {
        return _gridObjectArray[gridPosition.X, gridPosition.Z];
    }
}
