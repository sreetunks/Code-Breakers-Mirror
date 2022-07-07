using System;
using UnityEngine;


public class GridSystem
{
    private int _width;
    private int _height;
    private float _cellSize;
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

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * _cellSize;
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
                GameObject.Instantiate(debugPrefab, GetWorldPosition(x, z), Quaternion.identity);
            }
        }
    }
}
