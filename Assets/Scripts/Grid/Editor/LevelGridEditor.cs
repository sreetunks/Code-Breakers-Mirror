using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Grid.Editor
{
    [CustomEditor(typeof(LevelGrid))]
    public class LevelGridEditor : UnityEditor.Editor
    {
        private SerializedProperty _gridWidth;
        private SerializedProperty _gridHeight;
        private SerializedProperty _gridCellSize;
        private SerializedProperty _gridOffset;
        private SerializedProperty _gridCellStates;

        private SerializedProperty _adjacentGridNorth;
        private SerializedProperty _adjacentGridEast;
        private SerializedProperty _adjacentGridSouth;
        private SerializedProperty _adjacentGridWest;

        private SerializedProperty _northDoorGridPosition;
        private SerializedProperty _eastDoorGridPosition;
        private SerializedProperty _southDoorGridPosition;
        private SerializedProperty _westDoorGridPosition;

        private readonly HashSet<GridPosition> _selectedGridTiles = new();
        private LevelGrid _levelGrid;

        private int _newGridWidth;
        private int _newGridHeight;
        private float _newGridCellSize;

        private void OnEnable()
        {
            _levelGrid = (LevelGrid)serializedObject.targetObject;
            _gridWidth = serializedObject.FindProperty("gridWidth");
            _gridHeight = serializedObject.FindProperty("gridHeight");
            _gridOffset = serializedObject.FindProperty("gridOffset");
            _gridCellSize = serializedObject.FindProperty("gridCellSize");
            _gridCellStates = serializedObject.FindProperty("gridCellStates");

            _adjacentGridNorth = serializedObject.FindProperty("adjacentGridNorth");
            _adjacentGridEast = serializedObject.FindProperty("adjacentGridEast");
            _adjacentGridSouth = serializedObject.FindProperty("adjacentGridSouth");
            _adjacentGridWest = serializedObject.FindProperty("adjacentGridWest");

            _northDoorGridPosition = serializedObject.FindProperty("northDoorGridPosition");
            _eastDoorGridPosition = serializedObject.FindProperty("eastDoorGridPosition");
            _southDoorGridPosition = serializedObject.FindProperty("southDoorGridPosition");
            _westDoorGridPosition = serializedObject.FindProperty("westDoorGridPosition");

            _newGridWidth = _gridWidth.intValue;
            _newGridHeight = _gridHeight.intValue;
            _newGridCellSize = _gridCellSize.floatValue;

            if(!Application.isPlaying) GridSystem.RegisterLevelGrid(_levelGrid);
        }

        private void OnSceneGUI()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                var mouseDirRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                if (Physics.Raycast(mouseDirRay, out var hit, Mathf.Infinity, LayerMask.GetMask("MousePlane")))
                {
                    if (!Event.current.shift) _selectedGridTiles.Clear();

                    var selectedGridTilePos = GridSystem.GetGridPosition(hit.point);

                    if (_selectedGridTiles.Contains(selectedGridTilePos))
                        _selectedGridTiles.Remove(selectedGridTilePos);
                    else
                        _selectedGridTiles.Add(selectedGridTilePos);
                }
                else
                    _selectedGridTiles.Clear();
                Event.current.Use();
            }
            else if (Event.current.type == EventType.ScrollWheel)
            {
                if (_selectedGridTiles.Count <= 0) return;
                var selectedGridCellState = (GridCellState)(-1);
                foreach (var gridTile in _selectedGridTiles)
                {
                    GridSystem.TryGetGridCellState(gridTile, out var tempSelectedGridCellState);
                    if (selectedGridCellState == (GridCellState)(-1))
                    {
                        selectedGridCellState = tempSelectedGridCellState;
                    }
                    else if (tempSelectedGridCellState != selectedGridCellState)
                    {
                        selectedGridCellState = (GridCellState)(-1);
                        break;
                    }
                }
                if (selectedGridCellState == (GridCellState)(-1)) selectedGridCellState = GridCellState.Impassable;
                var newGridCellState = (GridCellState)((((int)selectedGridCellState + (int)Mathf.Sign(Event.current.delta.y)) + ((int)GridCellState.LevelExit + 1)) % ((int)GridCellState.LevelExit + 1));
                foreach (var gridTile in _selectedGridTiles)
                    UpdateGridCellState(gridTile, newGridCellState); // Update Grid Cell State is considered Expensive

                if (serializedObject.ApplyModifiedProperties())
                    EditorUtility.SetDirty(target);

                Event.current.Use();
            }
            else if (Event.current.type == EventType.Repaint)
            {
                if (_selectedGridTiles.Count == 0) return;
                var handlesMatrix = Handles.matrix;
                Handles.matrix = _levelGrid.transform.localToWorldMatrix;
                foreach (var gridTile in _selectedGridTiles)
                {
                    Handles.RectangleHandleCap(
                    0,
                    GridSystem.GetWorldPosition(gridTile) - _levelGrid.transform.position,
                    Quaternion.LookRotation(Vector3.up),
                    GridSystem.ActiveLevelGrid.GridCellSize * 0.5f,
                    EventType.Repaint
                    );
                }
                Handles.matrix = handlesMatrix;
                Repaint();
            }
        }

        private void UpdateGridCellState(GridPosition position, GridCellState state)
        {
            var additionalGridCellModified = GridPosition.Invalid;
            SerializedProperty additionalGridCellPosition = null;

            switch (state)
            {
                case GridCellState.DoorNorth:
                    additionalGridCellModified = _levelGrid.DoorNorth;
                    additionalGridCellPosition = _northDoorGridPosition;
                    break;
                case GridCellState.DoorEast:
                    additionalGridCellModified = _levelGrid.DoorEast;
                    additionalGridCellPosition = _eastDoorGridPosition;
                    break;
                case GridCellState.DoorSouth:
                    additionalGridCellModified = _levelGrid.DoorSouth;
                    additionalGridCellPosition = _southDoorGridPosition;
                    break;
                case GridCellState.DoorWest:
                    additionalGridCellModified = _levelGrid.DoorWest;
                    additionalGridCellPosition = _westDoorGridPosition;
                    break;
                case GridCellState.Impassable:
                    break;
                case GridCellState.Walkable:
                    break;
                case GridCellState.Occupied:
                    break;
                case GridCellState.LevelExit:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            GridSystem.SetGridCellState(position, state);
            _levelGrid.UpdateGrid(); // Update Grid is considered Expensive
            var gridCellIndex = (position.Z * _levelGrid.GridWidth) + position.X;
            var gridCellState = _gridCellStates.GetArrayElementAtIndex(gridCellIndex);
            gridCellState.enumValueIndex = (int)state;

            if (additionalGridCellModified == position)
            {
                if (additionalGridCellPosition == null) return;
                additionalGridCellPosition.FindPropertyRelative("X").intValue = -1;
                additionalGridCellPosition.FindPropertyRelative("Z").intValue = -1;
            }
            else
            {
                if (additionalGridCellModified != GridPosition.Invalid)
                {
                    var additionalGridCellIndex = (additionalGridCellModified.Z * _levelGrid.GridWidth) + additionalGridCellModified.X;
                    var additionalGridCellState = _gridCellStates.GetArrayElementAtIndex(additionalGridCellIndex);
                    additionalGridCellState.enumValueIndex = (int)GridCellState.Walkable;
                }
                
                if (additionalGridCellPosition == null) return;
                additionalGridCellPosition.FindPropertyRelative("X").intValue = position.X;
                additionalGridCellPosition.FindPropertyRelative("Z").intValue = position.Z;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(_adjacentGridNorth);
            EditorGUILayout.PropertyField(_adjacentGridEast);
            EditorGUILayout.PropertyField(_adjacentGridSouth);
            EditorGUILayout.PropertyField(_adjacentGridWest);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            var tempGridWidth = EditorGUILayout.IntField(_newGridWidth);
            if (tempGridWidth != _gridWidth.intValue)
                _newGridWidth = tempGridWidth;
            var tempGridHeight = EditorGUILayout.IntField(_newGridHeight);
            if (tempGridHeight != _gridHeight.intValue)
                _newGridHeight = tempGridHeight;
            var tempGridCellSize = EditorGUILayout.FloatField(_newGridCellSize);
            if (tempGridCellSize != _gridCellSize.floatValue) // Floating Point Comparsion
                _newGridCellSize = tempGridCellSize;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            if (_selectedGridTiles.Count > 0)
            {
                EditorGUILayout.BeginVertical();
                var selectedGridCellState = (GridCellState)(-1);
                foreach (var gridTile in _selectedGridTiles)
                {
                    GridSystem.TryGetGridCellState(gridTile, out var tempSelectedGridCellState);
                    if (selectedGridCellState == (GridCellState)(-1))
                    {
                        selectedGridCellState = tempSelectedGridCellState;
                    }
                    else if (tempSelectedGridCellState != selectedGridCellState)
                    {
                        selectedGridCellState = (GridCellState)(-1);
                        break;
                    }
                }
                if (selectedGridCellState == (GridCellState)(-1)) EditorGUI.showMixedValue = true;
                var newGridCellState = (GridCellState)EditorGUILayout.EnumPopup("Cell State: ", selectedGridCellState);
                EditorGUI.showMixedValue = false;
                if (newGridCellState != selectedGridCellState)
                {
                    foreach (var gridTile in _selectedGridTiles)
                        UpdateGridCellState(gridTile, newGridCellState); // Update Grid Cell State is considered Expensive
                    if (serializedObject.ApplyModifiedProperties())
                        EditorUtility.SetDirty(target);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (GUI.Button(EditorGUILayout.GetControlRect(
                GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 1.5f)),
                "Reset"))
            {
                _gridWidth.intValue = _newGridWidth;
                _gridHeight.intValue = _newGridHeight;
                _gridCellSize.floatValue = _newGridCellSize;

                serializedObject.ApplyModifiedProperties();

                _levelGrid.ResetGrid(); // Reset Grid is considered Expensive

                serializedObject.Update();
                _gridCellStates = serializedObject.FindProperty("gridCellStates");

                _selectedGridTiles.Clear();
                _gridOffset.vector3Value = _levelGrid.GridOffset;

                if (serializedObject.ApplyModifiedProperties())
                    EditorUtility.SetDirty(target);
            }

            if (GUI.Button(EditorGUILayout.GetControlRect(
                GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 1.5f)),
                "Regenerate Mesh"))
            {
                _levelGrid.UpdateGridMeshData(); // Update Grid Mesh Data is considered Expensive

                _selectedGridTiles.Clear();

                if (serializedObject.ApplyModifiedProperties())
                    EditorUtility.SetDirty(target);
            }
        }
    }
}
