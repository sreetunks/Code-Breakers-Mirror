using System;
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

        private GridPosition _selectedGridTilePos = GridPosition.Invalid;
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
                    _selectedGridTilePos = GridSystem.GetGridPosition(hit.point);
                }
            }
            else if (Event.current.type == EventType.Repaint)
            {
                if (_selectedGridTilePos == GridPosition.Invalid) return;
                var handlesMatrix = Handles.matrix;
                Handles.matrix = _levelGrid.transform.localToWorldMatrix;
                Handles.RectangleHandleCap(
                    0,
                    GridSystem.GetWorldPosition(_selectedGridTilePos) - _levelGrid.transform.position,
                    Quaternion.LookRotation(Vector3.up),
                    GridSystem.ActiveLevelGrid.GridCellSize * 0.5f,
                    EventType.Repaint
                );
                Handles.matrix = handlesMatrix;
                Repaint();
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

            if (_selectedGridTilePos != GridPosition.Invalid)
            {
                EditorGUILayout.BeginVertical();
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.Vector2IntField(
                        "Selected Tile",
                        new Vector2Int(_selectedGridTilePos.X, _selectedGridTilePos.Z));
                }
                if (GridSystem.TryGetGridCellState(_selectedGridTilePos, out var selectedGridCellState))
                {
                    var newGridCellState = (GridCellState)EditorGUILayout.EnumPopup(selectedGridCellState);
                    if (newGridCellState != selectedGridCellState)
                    {
                        var additionalGridCellModified = GridPosition.Invalid;
                        SerializedProperty additionalGridCellPosition = null;

                        if (newGridCellState == GridCellState.DoorNorth)
                        {
                            additionalGridCellModified = _levelGrid.DoorNorth;
                            additionalGridCellPosition = _northDoorGridPosition;
                        }
                        else if (newGridCellState == GridCellState.DoorEast)
                        {
                            additionalGridCellModified = _levelGrid.DoorEast;
                            additionalGridCellPosition = _eastDoorGridPosition;
                        }
                        else if (newGridCellState == GridCellState.DoorSouth)
                        {
                            additionalGridCellModified = _levelGrid.DoorSouth;
                            additionalGridCellPosition = _southDoorGridPosition;
                        }
                        else if (newGridCellState == GridCellState.DoorWest)
                        {
                            additionalGridCellModified = _levelGrid.DoorWest;
                            additionalGridCellPosition = _westDoorGridPosition;
                        }

                        GridSystem.SetGridCellState(_selectedGridTilePos, newGridCellState);
                        _levelGrid.UpdateGrid();
                        var gridCellIndex = (_selectedGridTilePos.Z * _levelGrid.GridWidth) + _selectedGridTilePos.X;
                        var gridCellState = _gridCellStates.GetArrayElementAtIndex(gridCellIndex);
                        gridCellState.enumValueIndex = (int)newGridCellState;

                        if (additionalGridCellModified != GridPosition.Invalid)
                        {
                            var additionalGridCellIndex = (additionalGridCellModified.Z * _levelGrid.GridWidth) + additionalGridCellModified.X;
                            var additionalGridCellState = _gridCellStates.GetArrayElementAtIndex(additionalGridCellIndex);
                            additionalGridCellState.enumValueIndex = (int)GridCellState.Walkable;
                        }

                        if (additionalGridCellPosition != null)
                        {
                            additionalGridCellPosition.FindPropertyRelative("X").intValue = _selectedGridTilePos.X;
                            additionalGridCellPosition.FindPropertyRelative("Z").intValue = _selectedGridTilePos.Z;
                        }
                    }
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
                _selectedGridTilePos = GridPosition.Invalid;
                _gridOffset.vector3Value = _levelGrid.GridOffset;
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
