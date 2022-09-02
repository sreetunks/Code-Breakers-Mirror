using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGrid))]
public class LevelGridEditor : Editor
{
    SerializedProperty gridWidth;
    SerializedProperty gridHeight;
    SerializedProperty gridCellSize;
    SerializedProperty gridCellStates;
    GridPosition selectedGridTilePos = GridPosition.Invalid;
    LevelGrid _levelGrid;

    private void OnEnable()
    {
        _levelGrid = (LevelGrid)serializedObject.targetObject;
        gridWidth = serializedObject.FindProperty("gridWidth");
        gridHeight = serializedObject.FindProperty("gridHeight");
        gridCellSize = serializedObject.FindProperty("gridCellSize");
        gridCellStates = serializedObject.FindProperty("gridCellStates");

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        GridSystem.RegisterLevelGrid(_levelGrid);
    }

    private void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Ray mouseDirRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(mouseDirRay, out RaycastHit hit, LayerMask.GetMask("MousePlane")))
            {
                selectedGridTilePos = GridSystem.GetGridPosition(hit.point);
            }
        }
        else if (Event.current.type == EventType.Repaint)
        {
            if (selectedGridTilePos != GridPosition.Invalid)
            {
                Handles.RectangleHandleCap(
                    0,
                    //(new Vector3(1, 0, 1) * GridSystem.ActiveLevelGrid.GridCellSize * 0.5f) +
                    GridSystem.GetWorldPosition(selectedGridTilePos),
                    Quaternion.LookRotation(Vector3.up),
                    GridSystem.ActiveLevelGrid.GridCellSize * 0.5f,
                    EventType.Repaint
                    );
                Repaint();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(gridWidth);
        EditorGUILayout.PropertyField(gridHeight);
        EditorGUILayout.PropertyField(gridCellSize);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        if (selectedGridTilePos != GridPosition.Invalid)
        {
            EditorGUILayout.BeginVertical();
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.Vector2IntField(
                "Selected Tile",
                new Vector2Int(selectedGridTilePos.X, selectedGridTilePos.Z));
            }

            if (GridSystem.TryGetGridCellState(selectedGridTilePos, out GridCellState selectedGridCellState))
            {
                var newGridCellState = (GridCellState)EditorGUILayout.EnumFlagsField(selectedGridCellState);
                if (newGridCellState != selectedGridCellState)
                {
                    GridSystem.SetGridCellState(selectedGridTilePos, newGridCellState);
                    selectedGridCellState = newGridCellState;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.Button(EditorGUILayout.GetControlRect(
            GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 1.5f)),
            "Update"))
        {
            _levelGrid.UpdateGrid();
        }

        if (GUI.Button(EditorGUILayout.GetControlRect(
            GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 1.5f)),
            "Reset"))
        {
            _levelGrid.ResetGrid();
            selectedGridTilePos = GridPosition.Invalid;
        }
    }
}
