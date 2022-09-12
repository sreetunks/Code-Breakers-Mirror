using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Grid
{
    /// <summary>
    /// Contains the Grid Information for a Single Room.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
    public class LevelGrid : MonoBehaviour
    {
        [SerializeField] private int gridWidth;
        [SerializeField] private int gridHeight;
        [SerializeField] private float gridCellSize;
        [SerializeField] private GridCellState[] gridCellStates;
        [SerializeField] private Vector3 gridOffset;

        [SerializeField] private LevelGrid adjacentGridNorth;
        [SerializeField] private LevelGrid adjacentGridEast;
        [SerializeField] private LevelGrid adjacentGridSouth;
        [SerializeField] private LevelGrid adjacentGridWest;

        [SerializeField] private GridPosition northDoorGridPosition = GridPosition.Invalid;
        [SerializeField] private GridPosition eastDoorGridPosition = GridPosition.Invalid;
        [SerializeField] private GridPosition southDoorGridPosition = GridPosition.Invalid;
        [SerializeField] private GridPosition westDoorGridPosition = GridPosition.Invalid;

        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public float GridCellSize => gridCellSize;
        public Vector3 GridOffset => gridOffset;

        public GridPosition DoorNorth => northDoorGridPosition;
        public GridPosition DoorEast => eastDoorGridPosition;
        public GridPosition DoorSouth => southDoorGridPosition;
        public GridPosition DoorWest => westDoorGridPosition;

        public bool AreDoorsLocked => _areDoorsLocked;

        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private bool _areDoorsLocked = false;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }

        private int GetCellStateVertexIndex(int x, int y)
        {
            return ((y * gridWidth) + x) * 4;
        }

        private void UpdateCellState(Mesh mesh)
        {
            var vertexCount = gridWidth * gridHeight * 4;
            var cellStateArray = new NativeArray<float>(vertexCount, Allocator.Temp);
            for (var y = 0; y < gridHeight; ++y)
            {
                for (var x = 0; x < gridWidth; ++x)
                {
                    var idx = GetCellStateVertexIndex(x, y);
                    cellStateArray[idx] = (float)gridCellStates[(y  * gridWidth) + x] + 0.5f;
                    cellStateArray[idx + 1] = (float)gridCellStates[(y  * gridWidth) + x] + 0.5f;
                    cellStateArray[idx + 2] = (float)gridCellStates[(y  * gridWidth) + x] + 0.5f;
                    cellStateArray[idx + 3] = (float)gridCellStates[(y  * gridWidth) + x] + 0.5f;
                }
            }
            mesh.SetVertexBufferData(cellStateArray, 0, 0, vertexCount, 2);
        }

        public void UpdateCellRangeInfo(NativeArray<float> rangeArray)
        {
            var mesh = _meshFilter.mesh;
            var vertexCount = gridWidth * gridHeight * 4;
            var cellStateArray = new NativeArray<float>(vertexCount, Allocator.Temp);
            for (var y = 0; y < gridHeight; ++y)
            {
                for (var x = 0; x < gridWidth; ++x)
                {
                    var idx = (y * gridWidth) + x;
                    var vertIndex = GetCellStateVertexIndex(x, y);
                    cellStateArray[vertIndex] = rangeArray[idx];
                    cellStateArray[vertIndex + 1] = rangeArray[idx];
                    cellStateArray[vertIndex + 2] = rangeArray[idx];
                    cellStateArray[vertIndex + 3] = rangeArray[idx];
                }
            }
            mesh.SetVertexBufferData(cellStateArray, 0, 0, vertexCount, 3);
        }

        public void ResetCellRangeInfo()
        {
            var mesh = _meshFilter.mesh;
            var vertexCount = gridWidth * gridHeight * 4;
            var cellStateArray = new NativeArray<float>(vertexCount, Allocator.Temp);
            for (var y = 0; y < gridHeight; ++y)
            {
                for (var x = 0; x < gridWidth; ++x)
                {
                    var vertIndex = GetCellStateVertexIndex(x, y);
                    cellStateArray[vertIndex] = 0;
                    cellStateArray[vertIndex + 1] = 0;
                    cellStateArray[vertIndex + 2] = 0;
                    cellStateArray[vertIndex + 3] = 0;
                }
            }
            mesh.SetVertexBufferData(cellStateArray, 0, 0, vertexCount, 3);
        }
#if UNITY_EDITOR
        public void UpdateGridMeshData()
        {
            if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>(); // Get Components is considered Expensive
            if (_meshCollider == null) _meshCollider = GetComponent<MeshCollider>(); // Get Components is considered Expensive

            var gridMesh = new Mesh();
            gridMesh.Clear();

            var vertexCount = gridWidth * gridHeight * 4;
            var quadCount = gridWidth * gridHeight;
            var vertexLayout = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 1),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float32, 1, 2),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.Float32, 1, 3)
            };
            gridMesh.SetVertexBufferParams(vertexCount, vertexLayout);

            var posArray = new NativeArray<Vector3>(vertexCount, Allocator.Temp);
            var uvArray = new NativeArray<Vector2>(vertexCount, Allocator.Temp);
            var pathfindingArray = new NativeArray<float>(vertexCount, Allocator.Temp);
            var gridVertexOffset = gridOffset - transform.position;

            for (var y = 0; y < gridHeight; ++y)
            {
                for (var x = 0; x < gridWidth; ++x)
                {
                    var cellCenter = new Vector3(x, 0, y) * gridCellSize;
                    var idx = ((y * gridWidth) + x) * 4;

                    posArray[idx] = gridVertexOffset + cellCenter;
                    uvArray[idx] = new Vector2(0, 0);

                    posArray[idx + 1] = gridVertexOffset + cellCenter + new Vector3(0, 0, gridCellSize);
                    uvArray[idx + 1] = new Vector2(0, 1);

                    posArray[idx + 2] = gridVertexOffset + cellCenter + new Vector3(gridCellSize, 0, gridCellSize);
                    uvArray[idx + 2] = new Vector2(1, 1);

                    posArray[idx + 3] = gridVertexOffset + cellCenter + new Vector3(gridCellSize, 0, 0);
                    uvArray[idx + 3] = new Vector2(1, 0);
                }
            }

            UpdateCellState(gridMesh);
            gridMesh.SetVertexBufferData(posArray, 0, 0, vertexCount, 0);
            gridMesh.SetVertexBufferData(uvArray, 0, 0, vertexCount, 1);
            gridMesh.SetVertexBufferData(pathfindingArray, 0, 0, vertexCount, 3);

            var indices = new NativeArray<ushort>(quadCount * 6, Allocator.Temp);

            ushort vertIndex = 0;

            for (var y = 0; y < gridHeight; ++y)
            {
                for (var x = 0; x < gridWidth; ++x)
                {

                    var idx = ((y * gridWidth) + x) * 6;
                    indices[idx] = vertIndex;
                    indices[idx + 1] = (ushort)(vertIndex + 1);
                    indices[idx + 2] = (ushort)(vertIndex + 2);

                    indices[idx + 3] = (ushort)(vertIndex + 2);
                    indices[idx + 4] = (ushort)(vertIndex + 3);
                    indices[idx + 5] = vertIndex;

                    vertIndex += 4;

                }
            }

            gridMesh.SetIndexBufferParams(quadCount * 6, IndexFormat.UInt16);
            gridMesh.SetIndexBufferData(indices, 0, 0, quadCount * 6);

            var subMeshDescriptor = new SubMeshDescriptor
            {
                baseVertex = 0,
                indexCount = quadCount * 6,
                indexStart = 0,
                topology = MeshTopology.Triangles
            };

            gridMesh.subMeshCount = 1;
            gridMesh.SetSubMesh(0, subMeshDescriptor);

            gridMesh.name = "Grid";
            gridMesh.RecalculateBounds();
            gridMesh.UploadMeshData(false);

            _meshFilter.sharedMesh = gridMesh;
            _meshCollider.sharedMesh = gridMesh;
        }

        public void ResetGrid()
        {
            gridOffset = transform.position + (new Vector3(-0.5f * gridWidth, 0, -0.5f * gridHeight) * gridCellSize);
            if (gridWidth % 2.0f == 0) gridOffset.x += 0.5f * gridCellSize;
            if (gridHeight % 2.0f == 0) gridOffset.z += 0.5f * gridCellSize;

            gridCellStates = new GridCellState[gridWidth * gridHeight];
            for (var i = 0; i < gridWidth; ++i)
            {
                for (var j = 0; j < gridHeight; ++j)
                {
                    gridCellStates[(i * gridHeight) + j] = GridCellState.Walkable;
                }
            }

            northDoorGridPosition = GridPosition.Invalid;
            eastDoorGridPosition = GridPosition.Invalid;
            southDoorGridPosition = GridPosition.Invalid;
            westDoorGridPosition = GridPosition.Invalid;

            UpdateGridMeshData(); // Update the Mesh is considered Expensive
        }

        public void UpdateGrid()
        {
            if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>(); // Get Components is considered Expensive
            if (_meshCollider == null) _meshCollider = GetComponent<MeshCollider>(); // Get Components is considered Expensive
            UpdateCellState(_meshFilter.sharedMesh);
            _meshCollider.sharedMesh = _meshFilter.sharedMesh;
        }
#endif
        public void SetDoorLock(bool shouldLock)
        {
            _areDoorsLocked = shouldLock;
            // TODO: If locked, hide Door UI elements if any
        }

        public LevelGrid GetRoomAdjacentToDoor(GridCellState doorCellState)
        {
            switch (doorCellState)
            {
                case GridCellState.DoorNorth: return adjacentGridNorth;
                case GridCellState.DoorEast: return adjacentGridEast;
                case GridCellState.DoorSouth: return adjacentGridSouth;
                case GridCellState.DoorWest: return adjacentGridWest;
                default: return null;
            }
        }

        private bool IsValidGridPosition(GridPosition gridPosition)
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

            if (Application.isPlaying)
            {
                float[] gridState = { (float)gridCellState + 0.5f, (float)gridCellState + 0.5f, (float)gridCellState + 0.5f, (float)gridCellState + 0.5f };
                _meshFilter.mesh.SetVertexBufferData(gridState, 0, GetCellStateVertexIndex(gridPosition.X, gridPosition.Z), 4, 2);

                return;
            }

            switch (gridCellState)
            {
                case GridCellState.DoorNorth:
                {
                    if (northDoorGridPosition != GridPosition.Invalid)
                    {
                        gridCellStates[(northDoorGridPosition.Z * gridWidth) + northDoorGridPosition.X] = GridCellState.Walkable;
                    }
                    northDoorGridPosition = gridPosition;
                    break;
                }
                case GridCellState.DoorEast:
                {
                    if (eastDoorGridPosition != GridPosition.Invalid)
                    {
                        gridCellStates[(eastDoorGridPosition.Z * gridWidth) + eastDoorGridPosition.X] = GridCellState.Walkable;
                    }
                    eastDoorGridPosition = gridPosition;
                    break;
                }
                case GridCellState.DoorSouth:
                {
                    if (southDoorGridPosition != GridPosition.Invalid)
                    {
                        gridCellStates[(southDoorGridPosition.Z * gridWidth) + southDoorGridPosition.X] = GridCellState.Walkable;
                    }
                    southDoorGridPosition = gridPosition;
                    break;
                }
                case GridCellState.DoorWest:
                {
                    if (westDoorGridPosition != GridPosition.Invalid)
                    {
                        gridCellStates[(westDoorGridPosition.Z * gridWidth) + westDoorGridPosition.X] = GridCellState.Walkable;
                    }
                    westDoorGridPosition = gridPosition;
                    break;
                }
                case GridCellState.Impassable:
                    break;
                case GridCellState.Walkable:
                    break;
                case GridCellState.Occupied:
                    break;
                case GridCellState.LevelExit:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gridCellState), gridCellState, null);
            }
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
}
