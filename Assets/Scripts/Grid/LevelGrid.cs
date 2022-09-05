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
            return ((((y * 2) + 1) * (gridWidth * 2)) + ((x + y + 1) * 2));
        }

        private void UpdateCellState(Mesh mesh)
        {
            var vertexCount = ((gridWidth * 2) + 1) * ((gridHeight * 2) + 1);
            var cellStateArray = new NativeArray<float>(vertexCount, Allocator.Temp);
            for (var y = 0; y < gridHeight; ++y)
            {
                for (var x = 0; x < gridWidth; ++x)
                {
                    var idx = GetCellStateVertexIndex(x, y);
                    cellStateArray[idx] = (float)gridCellStates[(y  * gridWidth) + x] + 0.5f;
                }
            }
            mesh.SetVertexBufferData(cellStateArray, 0, 0, vertexCount, 2);
        }

        private void UpdateGridMeshData()
        {
            if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
            if (_meshCollider == null) _meshCollider = GetComponent<MeshCollider>();

            var gridMesh = new Mesh();
            gridMesh.Clear();

            var vertexCount = ((gridWidth * 2) + 1) * ((gridHeight * 2) + 1);
            var quadCount = gridWidth * gridHeight * 4;
            var vertexLayout = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 1),
                new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 1, 2)
            };

            var posArray = new NativeArray<Vector3>(vertexCount, Allocator.Temp);
            var uvArray = new NativeArray<Vector2>(vertexCount, Allocator.Temp);

            Vector3 gridVertexOffset = gridOffset - transform.position;

            for (var y = 0; y < (gridHeight * 2) + 1; ++y)
            {
                for (var x = 0; x < (gridWidth * 2) + 1; ++x)
                {
                    var idx = (y * ((gridWidth * 2) + 1)) + x;
                    posArray[idx] = gridVertexOffset + (new Vector3(x, 0, y) * gridCellSize * 0.5f);
                    uvArray[idx] = new Vector2((float)x / (gridWidth * 2), (float)y / (gridHeight * 2));
                }
            }

            gridMesh.SetVertexBufferParams(vertexCount, vertexLayout);
            UpdateCellState(gridMesh);
            gridMesh.SetVertexBufferData(posArray, 0, 0, vertexCount, 0);
            gridMesh.SetVertexBufferData(uvArray, 0, 0, vertexCount, 1);

            var indices = new NativeArray<ushort>(quadCount * 6, Allocator.Temp);

            var vertIndex = (ushort)((gridWidth + 1) * 2);

            for (var y = 0; y < gridHeight; ++y)
            {
                for (var x = 0; x < gridWidth; ++x)
                {
                    var idx = (((y * 2) * gridWidth) + (x * 2) + 1) * 12;

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

            var subMeshDescriptor = new SubMeshDescriptor();
            subMeshDescriptor.baseVertex = 0;
            subMeshDescriptor.indexCount = quadCount * 6;
            subMeshDescriptor.indexStart = 0;
            subMeshDescriptor.topology = MeshTopology.Triangles;

            gridMesh.subMeshCount = 1;
            gridMesh.SetSubMesh(0, subMeshDescriptor);

            gridMesh.name = "Grid";
            gridMesh.RecalculateBounds();
            gridMesh.UploadMeshData(false);

            _meshFilter.sharedMesh = gridMesh;
            _meshCollider.sharedMesh = gridMesh;
        }
#if UNITY_EDITOR
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
                    gridCellStates[(i * gridHeight) + j] = GridCellState.Impassable;
                }
            }

            UpdateGridMeshData();
        }

        public void UpdateGrid()
        {
            if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
            if (_meshCollider == null) _meshCollider = GetComponent<MeshCollider>();
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

            if (Application.isPlaying)
            {
                float[] gridState = { (float)gridCellState };
                _meshFilter.mesh.SetVertexBufferData(gridState, 0, GetCellStateVertexIndex(gridPosition.X, gridPosition.Z), 1, 2);

                return;
            }

            if (gridCellState == GridCellState.DoorNorth)
            {
                if (northDoorGridPosition != GridPosition.Invalid)
                {
                    gridCellStates[(northDoorGridPosition.Z * gridWidth) + northDoorGridPosition.X] = GridCellState.Walkable;
                }
                northDoorGridPosition = gridPosition;
            }
            if (gridCellState == GridCellState.DoorEast)
            {
                if (eastDoorGridPosition != GridPosition.Invalid)
                {
                    gridCellStates[(eastDoorGridPosition.Z * gridWidth) + eastDoorGridPosition.X] = GridCellState.Walkable;
                }
                eastDoorGridPosition = gridPosition;
            }
            if (gridCellState == GridCellState.DoorSouth)
            {
                if (southDoorGridPosition != GridPosition.Invalid)
                {
                    gridCellStates[(southDoorGridPosition.Z * gridWidth) + southDoorGridPosition.X] = GridCellState.Walkable;
                }
                southDoorGridPosition = gridPosition;
            }
            if (gridCellState == GridCellState.DoorWest)
            {
                if (westDoorGridPosition != GridPosition.Invalid)
                {
                    gridCellStates[(westDoorGridPosition.Z * gridWidth) + westDoorGridPosition.X] = GridCellState.Walkable;
                }
                westDoorGridPosition = gridPosition;
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
