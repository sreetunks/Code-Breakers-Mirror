using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelGenerationSystem : MonoBehaviour
{
    public enum RoomGraphType
    {
        Unrooted,
        Rooted
    }

    [System.Serializable]
    public struct CriticalPathMinMax
    {
        public int Min;
        public int Max;

        public CriticalPathMinMax(int min, int max) { Min = min; Max = max; }
    }

    [SerializeField] Transform roomsParentTransform;
    [SerializeField] LayerMask roomLayerMask;
    [SerializeField] int maxGeneratorIterations = 8;

    [SerializeField] bool isBossLevel = false;
    [SerializeField] RoomGraphType roomGraphType;

    [SerializeField, Range(0.0f, 1.0f)] float criticalPathLinearity = 1.0f;

    [SerializeField] CriticalPathMinMax criticalPathLengthBounds = new CriticalPathMinMax(1, 1);
    [SerializeField] int criticalPathPivotCount = 0;
    [SerializeField] int criticalPathFreeDistance = 2;

    [SerializeField] List<Room> spawnRoomPrefabs;
    [SerializeField] List<Room> commonRoomPrefabsPivotNone;
    [SerializeField] List<Room> commonRoomPrefabsPivotRight;
    [SerializeField] List<Room> commonRoomPrefabsPivotLeft;
    [SerializeField] List<Room> exitRoomPrefabs;

    [SerializeField, Range(0.0f, 1.0f)] float levelSparsity = 0.0f;

    List<Room> _generatedRoomList;

    public void Clear()
    {
        if (_generatedRoomList == null) _generatedRoomList = new List<Room>();
        foreach (var generatedRoom in _generatedRoomList)
        {
            DestroyImmediate(generatedRoom.gameObject);
        }
        _generatedRoomList.Clear();
    }

    void GetRoomGridCoordinatesAtFreeDistance(int freeDistance, RoomGraphType roomGraphType, out int gridX, out int gridY)
    {
        gridX = 0;
        gridY = 0;
        int roomPos = 0;

        if (roomGraphType == RoomGraphType.Unrooted)
            roomPos = Random.Range(0, freeDistance * 8);
        else if (roomGraphType == RoomGraphType.Rooted)
            roomPos = Random.Range(1, freeDistance * 8);

        if (roomPos >= freeDistance * 6)
        {
            gridY = (freeDistance * 8) - roomPos;
        }
        else if (roomPos >= freeDistance * 4)
        {
            gridX = (freeDistance * 6) - roomPos;
            gridY = freeDistance * 2;
        }
        else if (roomPos >= freeDistance * 2)
        {
            gridX = freeDistance * 2;
            gridY = roomPos - (freeDistance * 2);
        }
        else
        {
            gridX = roomPos;
        }

        gridX = gridX - freeDistance;
        gridY = gridY - freeDistance;

        if (roomGraphType == RoomGraphType.Rooted) gridY = Mathf.Abs(gridY);
    }

    void AdvanceInDirection(Room.DoorDirection direction, ref int roomX, ref int roomY)
    {
        switch (direction)
        {
            case Room.DoorDirection.North: { ++roomY; break; }
            case Room.DoorDirection.East: { ++roomX; break; }
            case Room.DoorDirection.South: { --roomY; break; }
            case Room.DoorDirection.West: { --roomX; break; }
        }
    }

    int GenerateInitialPath(Room.DoorDirection currentDirection, Vector3 spawnRoomPosition, Quaternion currentRotation, int targetX, int targetY)
    {
        int roomGridX = 0, roomGridY = 0;
        int criticalPathLength = 0;
        AdvanceInDirection(currentDirection, ref roomGridX, ref roomGridY);
        while (!(roomGridX == targetX && roomGridY == targetY))
        {
            Room roomPrefab = commonRoomPrefabsPivotNone[0];
            Quaternion newRotation = currentRotation;

            if (currentDirection == Room.DoorDirection.North)
            {
                if (targetX > roomGridX)
                {
                    roomPrefab = commonRoomPrefabsPivotRight[0];
                    newRotation *= Quaternion.Euler(0, 90, 0);
                }
                else if (targetX < roomGridX)
                {
                    roomPrefab = commonRoomPrefabsPivotLeft[0];
                    newRotation *= Quaternion.Euler(0, -90, 0);
                }
            }
            if (currentDirection == Room.DoorDirection.East)
            {
                if (targetY > roomGridY)
                {
                    roomPrefab = commonRoomPrefabsPivotLeft[0];
                    newRotation *= Quaternion.Euler(0, -90, 0);
                }
                else if (targetY < roomGridY)
                {
                    roomPrefab = commonRoomPrefabsPivotRight[0];
                    newRotation *= Quaternion.Euler(0, 90, 0);
                }
            }
            if (currentDirection == Room.DoorDirection.South)
            {
                if (targetX > roomGridX)
                {
                    roomPrefab = commonRoomPrefabsPivotLeft[0];
                    newRotation *= Quaternion.Euler(0, -90, 0);
                }
                else if (targetX < roomGridX)
                {
                    roomPrefab = commonRoomPrefabsPivotRight[0];
                    newRotation *= Quaternion.Euler(0, 90, 0);
                }
            }
            if (currentDirection == Room.DoorDirection.West)
            {
                if (targetY > roomGridY)
                {
                    roomPrefab = commonRoomPrefabsPivotRight[0];
                    newRotation *= Quaternion.Euler(0, 90, 0);
                }
                else if (targetY < roomGridY)
                {
                    roomPrefab = commonRoomPrefabsPivotLeft[0];
                    newRotation *= Quaternion.Euler(0, -90, 0);
                }
            }

            Vector3 roomPosition = spawnRoomPosition + new Vector3(roomGridX * 10.0f, 0, roomGridY * 10.0f);
#if UNITY_EDITOR
            Room room = (Room)PrefabUtility.InstantiatePrefab(roomPrefab, roomsParentTransform);
            room.transform.position = roomPosition;
            room.transform.rotation = currentRotation;
#else
            Room room = Instantiate(roomPrefab, roomPosition, currentRotation, roomsParentTransform);
#endif
            _generatedRoomList.Add(room);
            currentDirection = room.ExitDirection;
            AdvanceInDirection(currentDirection, ref roomGridX, ref roomGridY);
            currentRotation = newRotation;
            ++criticalPathLength;
        }
        return criticalPathLength;
    }

    public void Generate()
    {
        if (!(isActiveAndEnabled &&
              spawnRoomPrefabs.Count > 0 &&
              commonRoomPrefabsPivotNone.Count > 0 &&
              commonRoomPrefabsPivotRight.Count > 0 &&
              commonRoomPrefabsPivotLeft.Count > 0 &&
              exitRoomPrefabs.Count > 0))
        {
            return;
        }

        Clear();

        Vector3 spawnRoomPosition = roomsParentTransform.position;
#if UNITY_EDITOR
        var spawnRoom = (Room)PrefabUtility.InstantiatePrefab(spawnRoomPrefabs[0], roomsParentTransform);
        spawnRoom.transform.position = spawnRoomPosition;
#else
        var spawnRoom = Instantiate(spawnRoomPrefabs[0], spawnRoomPosition, Quaternion.identity, roomsParentTransform);
#endif
        int exitRoomGridX = 0, exitRoomGridY = 0;
        GetRoomGridCoordinatesAtFreeDistance(criticalPathFreeDistance, roomGraphType, out exitRoomGridX, out exitRoomGridY);
        Vector3 exitRoomPosition = spawnRoomPosition + new Vector3(exitRoomGridX * 10.0f, 0, exitRoomGridY * 10.0f);
        Quaternion exitRoomRotation = Quaternion.Euler(0, 90.0f * Random.Range(0, 4), 0);
#if UNITY_EDITOR
        var exitRoom = (Room)PrefabUtility.InstantiatePrefab(exitRoomPrefabs[0], roomsParentTransform);
        exitRoom.transform.position = exitRoomPosition;
#else
        var exitRoom = Instantiate(exitRoomPrefabs[0], exitRoomPosition, Quaternion.identity, roomsParentTransform);
#endif
        _generatedRoomList.Add(spawnRoom);
        int criticalPathLength = GenerateInitialPath(spawnRoom.ExitDirection, spawnRoomPosition, spawnRoom.transform.rotation, exitRoomGridX, exitRoomGridY);
        _generatedRoomList.Add(exitRoom);
        int numIterations = 0;
        while ((criticalPathLength < criticalPathLengthBounds.Min || criticalPathLength > criticalPathLengthBounds.Max) && numIterations < maxGeneratorIterations)
        {
            ++numIterations;
        }
    }

    /// <summary>
    /// Light-weight representation of the generated Rooms structure in a Level.
    /// Pre-Processed and used as an input to actually instantiate Room Prefabs.
    /// </summary>

    public interface IQuadTreeDataType
    {
        public int CheckOverlap(Bounds bounds);
    }

    public class QuadTree<T> where T : IQuadTreeDataType
    {
        public struct Bounds
        {
            public int width;
            public int height;

            public Bounds(int width, int height)
            {
                this.width = width;
                this.height = height;
            }
        }

        public class Node { protected Bounds _bounds; }
        public class BranchNode : Node
        {
            Node topLeft, topRight, bottomLeft, bottomRight;

            public BranchNode(int width, int height)
            {
                _bounds = new Bounds(width, height);
            }

            public void AddChild(T data)
            {
            }
        }
        public class LeafNode : Node
        {
            T _data;
            public LeafNode(T data)
            {
                _data = data;
            }
        }

        BranchNode _root;

        public QuadTree(int width, int height)
        {
            _root = new BranchNode(width, height);
        }

        public void Insert(T data) { _root.AddChild(data); }
    }
}
