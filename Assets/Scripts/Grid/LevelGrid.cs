using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Contains the Grid Information for a Single Room.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class LevelGrid : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float gridCellSize;
    [SerializeField] GridCellState[] gridCellStates;
    [SerializeField] Vector3 gridOffset;

    public int GridWidth { get => gridWidth; }
    public int GridHeight { get => gridHeight; }
    public float GridCellSize { get => gridCellSize; }
    public Vector3 GridOffset { get => gridOffset; }

    MeshFilter _meshFilter;
    MeshCollider _meshCollider;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
    }

    int GetCellStateVertexIndex(int x, int y)
    {
        return ((((y * 2) + 1) * (gridWidth * 2)) + ((x + y + 1) * 2));
    }

    void UpdateCellState(Mesh mesh)
    {
        int vertexCount = ((gridWidth * 2) + 1) * ((gridHeight * 2) + 1);
        var cellStateArray = new NativeArray<float>(vertexCount, Allocator.Temp);
        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                int idx = GetCellStateVertexIndex(x, y);
                cellStateArray[idx] = (float)gridCellStates[(y  * gridWidth) + x] + 0.5f;
            }
        }
        mesh.SetVertexBufferData(cellStateArray, 0, 0, vertexCount, 2);
    }

    void UpdateGridMeshData()
    {
        if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
        if (_meshCollider == null) _meshCollider = GetComponent<MeshCollider>();

        Mesh gridMesh = new Mesh();
        gridMesh.Clear();

        int vertexCount = ((gridWidth * 2) + 1) * ((gridHeight * 2) + 1);
        int quadCount = gridWidth * gridHeight * 4;
        var vertexLayout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 1),
            new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 1, 2)
        };

        var posArray = new NativeArray<Vector3>(vertexCount, Allocator.Temp);
        var uvArray = new NativeArray<Vector2>(vertexCount, Allocator.Temp);

        for (int y = 0; y < (gridHeight * 2) + 1; ++y)
        {
            for (int x = 0; x < (gridWidth * 2) + 1; ++x)
            {
                int idx = (y * ((gridWidth * 2) + 1)) + x;
                posArray[idx] = gridOffset + (new Vector3(x, 0, y) * gridCellSize * 0.5f);
                uvArray[idx] = new Vector2((float)x / (gridWidth * 2), (float)y / (gridHeight * 2));
            }
        }


        gridMesh.SetVertexBufferParams(vertexCount, vertexLayout);
        UpdateCellState(gridMesh);
        gridMesh.SetVertexBufferData(posArray, 0, 0, vertexCount, 0);
        gridMesh.SetVertexBufferData(uvArray, 0, 0, vertexCount, 1);

        var indices = new NativeArray<ushort>(quadCount * 6, Allocator.Temp);

        ushort vertIndex = (ushort)((gridWidth + 1) * 2);

        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                int idx = (((y * 2) * gridWidth) + (x * 2) + 1) * 12;

                indices[idx - 12] = (ushort)(vertIndex - 1);
                indices[idx - 11] = (ushort)(vertIndex - ((gridWidth * 2) + 1));
                indices[idx - 10] = (ushort)(vertIndex - ((gridWidth * 2) + 1) - 1);

                indices[idx - 9] = (ushort)(vertIndex - 1);
                indices[idx - 8] = vertIndex;
                indices[idx - 7] = (ushort)(vertIndex - ((gridWidth * 2) + 1));

                indices[idx - 6] = vertIndex;
                indices[idx - 5] = (ushort)(vertIndex + 1);
                indices[idx - 4] = (ushort)(vertIndex - ((gridWidth * 2) + 1));

                indices[idx - 3] = (ushort)(vertIndex + 1);
                indices[idx - 2] = (ushort)(vertIndex - ((gridWidth * 2) + 1) + 1);
                indices[idx - 1] = (ushort)(vertIndex - ((gridWidth * 2) + 1));

                indices[idx] = (ushort)(vertIndex - 1);
                indices[idx + 1] = (ushort)(vertIndex + ((gridWidth * 2) + 1) - 1);
                indices[idx + 2] = (ushort)(vertIndex + ((gridWidth * 2) + 1));

                indices[idx + 3] = (ushort)(vertIndex + ((gridWidth * 2) + 1));
                indices[idx + 4] = vertIndex;
                indices[idx + 5] = (ushort)(vertIndex - 1);

                indices[idx + 6] = vertIndex;
                indices[idx + 7] = (ushort)(vertIndex + ((gridWidth * 2) + 1));
                indices[idx + 8] = (ushort)(vertIndex + 1);

                indices[idx + 9] = (ushort)(vertIndex + ((gridWidth * 2) + 1));
                indices[idx + 10] = (ushort)(vertIndex + ((gridWidth * 2) + 1) + 1);
                indices[idx + 11] = (ushort)(vertIndex + 1);

                vertIndex += 2;
            }
            vertIndex += (ushort)((gridWidth + 1) * 2);
        }

        gridMesh.SetIndexBufferParams(quadCount * 6, IndexFormat.UInt16);
        gridMesh.SetIndexBufferData(indices, 0, 0, quadCount * 6);

        SubMeshDescriptor subMeshDescriptor = new SubMeshDescriptor();
        subMeshDescriptor.baseVertex = 0;
        subMeshDescriptor.indexCount = quadCount * 6;
        subMeshDescriptor.indexStart = 0;
        subMeshDescriptor.topology = MeshTopology.Triangles;

        gridMesh.subMeshCount = 1;
        gridMesh.SetSubMesh(0, subMeshDescriptor);

        gridMesh.RecalculateBounds();
        gridMesh.UploadMeshData(false);

        _meshFilter.sharedMesh = gridMesh;
        _meshCollider.sharedMesh = gridMesh;
    }

    public void ResetGrid()
    {
        gridOffset = new Vector3(-0.5f * gridWidth, 0, -0.5f * gridHeight) * gridCellSize;
        if (gridWidth % 2.0f == 0) gridOffset.x += 0.5f * gridCellSize;
        if (gridHeight % 2.0f == 0) gridOffset.z += 0.5f * gridCellSize;

        gridCellStates = new GridCellState[gridWidth * gridHeight];
        for (int i = 0; i < gridWidth; ++i)
        {
            for (int j = 0; j < gridHeight; ++j)
            {
                gridCellStates[(i * gridHeight) + j] = GridCellState.Impassable;
            }
        }

        UpdateGridMeshData();

        GridSystem.RegisterLevelGrid(this);
    }
#if UNITY_EDITOR
    public void UpdateGrid()
    {
        UpdateCellState(_meshFilter.sharedMesh);
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
        if (!IsValidGridPosition(gridPosition)) return;
        gridCellStates[(gridPosition.Z * gridWidth) + gridPosition.X] = gridCellState;
        if (!Application.isPlaying) return;
        float[] gridState = { (float)gridCellState };
        _meshFilter.mesh.SetVertexBufferData(gridState, 0, GetCellStateVertexIndex(gridPosition.X, gridPosition.Z), 1, 2);
    }

    public bool TryGetGridCellState(GridPosition gridPosition, out GridCellState gridCellState)
    {
        if (IsValidGridPosition(gridPosition))
        {
            gridCellState = gridCellStates[(gridPosition.Z * gridWidth) + gridPosition.X];

            return true;
        }

        gridCellState = GridCellState.Impassable;

        return false;
    }
}
