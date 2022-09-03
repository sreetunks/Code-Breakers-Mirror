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

            GridSystem.RegisterLevelGrid(_levelGrid);
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
            EditorGUILayout.PropertyField(_gridWidth);
            EditorGUILayout.PropertyField(_gridHeight);
            EditorGUILayout.PropertyField(_gridCellSize);
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
                    var newGridCellState = (GridCellState)EditorGUILayout.EnumFlagsField(selectedGridCellState);
                    if (newGridCellState != selectedGridCellState)
                    {
                        GridSystem.SetGridCellState(_selectedGridTilePos, newGridCellState);
                        selectedGridCellState = newGridCellState;
                        _levelGrid.UpdateGrid();
                        var gridCellIndex = (_selectedGridTilePos.Z * _levelGrid.GridWidth) + _selectedGridTilePos.X;
                        var gridCellState = _gridCellStates.GetArrayElementAtIndex(gridCellIndex);
                        gridCellState.enumValueIndex = (int)newGridCellState;
                        serializedObject.ApplyModifiedProperties();
                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (GUI.Button(EditorGUILayout.GetControlRect(
                GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 1.5f)),
                "Reset"))
            {
                _levelGrid.ResetGrid();
                _selectedGridTilePos = GridPosition.Invalid;
                _gridOffset.vector3Value = _levelGrid.GridOffset;
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }

    }
}
