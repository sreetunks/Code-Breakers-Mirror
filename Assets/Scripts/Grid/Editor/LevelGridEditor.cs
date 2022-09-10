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

        private HashSet<GridPosition> _selectedGridTiles = new HashSet<GridPosition>();
        private LevelGrid _levelGrid;

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
#if UNITY_EDITOR
            if(!Application.isPlaying) GridSystem.RegisterLevelGrid(_levelGrid);
#endif
        }

        private void OnSceneGUI()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                var mouseDirRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                if (Physics.Raycast(mouseDirRay, out var hit, LayerMask.GetMask("MousePlane")))
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

            if (state == GridCellState.DoorNorth)
            {
                additionalGridCellModified = _levelGrid.DoorNorth;
                additionalGridCellPosition = _northDoorGridPosition;
            }
            else if (state == GridCellState.DoorEast)
            {
                additionalGridCellModified = _levelGrid.DoorEast;
                additionalGridCellPosition = _eastDoorGridPosition;
            }
            else if (state == GridCellState.DoorSouth)
            {
                additionalGridCellModified = _levelGrid.DoorSouth;
                additionalGridCellPosition = _southDoorGridPosition;
            }
            else if (state == GridCellState.DoorWest)
            {
                additionalGridCellModified = _levelGrid.DoorWest;
                additionalGridCellPosition = _westDoorGridPosition;
            }

            GridSystem.SetGridCellState(position, state);
            _levelGrid.UpdateGrid();
            var gridCellIndex = (position.Z * _levelGrid.GridWidth) + position.X;
            var gridCellState = _gridCellStates.GetArrayElementAtIndex(gridCellIndex);
            gridCellState.enumValueIndex = (int)state;

            if (additionalGridCellModified != GridPosition.Invalid)
            {
                var additionalGridCellIndex = (additionalGridCellModified.Z * _levelGrid.GridWidth) + additionalGridCellModified.X;
                var additionalGridCellState = _gridCellStates.GetArrayElementAtIndex(additionalGridCellIndex);
                additionalGridCellState.enumValueIndex = (int)GridCellState.Walkable;
            }

            if (additionalGridCellPosition != null)
            {
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

            EditorGUILayout.BeginVertical();
            var tempGridWidth = EditorGUILayout.IntField(_gridWidth.intValue);
            var tempGridHeight = EditorGUILayout.IntField(_gridHeight.intValue);
            var tempGridCellSize = EditorGUILayout.FloatField(_gridCellSize.floatValue);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            if (_selectedGridTiles.Count > 0)
            {
                EditorGUILayout.BeginVertical();
                using (new EditorGUI.DisabledScope(true))
                {
                    //EditorGUILayout.Vector2IntField(
                    //    "Selected Tile",
                    //    new Vector2Int(_selectedGridTilePos.X, _selectedGridTilePos.Z));
                }
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
                        UpdateGridCellState(gridTile, newGridCellState);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (GUI.Button(EditorGUILayout.GetControlRect(
                GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 1.5f)),
                "Reset"))
            {
                _gridWidth.intValue = tempGridWidth;
                _gridHeight.intValue = tempGridHeight;
                _gridCellSize.floatValue = tempGridCellSize;

                _levelGrid.ResetGrid();
                _selectedGridTiles.Clear();
                _gridOffset.vector3Value = _levelGrid.GridOffset;
            }

            if(serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }

    }
}
