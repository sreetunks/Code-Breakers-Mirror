using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGrid))]
public class LevelGridEditor : Editor
{
    SerializedProperty gridWidth;
    SerializedProperty gridHeight;
    SerializedProperty gridOffset;
    SerializedProperty gridCellSize;
    SerializedProperty gridCellStates;
    GridPosition selectedGridTilePos = new GridPosition(-1, -1);
    LevelGrid _levelGrid;

    private void OnEnable()
    {
        _levelGrid = (LevelGrid)serializedObject.targetObject;
        gridWidth = serializedObject.FindProperty("gridWidth");
        gridHeight = serializedObject.FindProperty("gridHeight");
        gridOffset = serializedObject.FindProperty("gridOffset");
        gridCellSize = serializedObject.FindProperty("gridCellSize");
        gridCellStates = serializedObject.FindProperty("gridCellStates");
    }

    private void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            Ray mouseDirRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(mouseDirRay, out RaycastHit hit))
            {
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(gridWidth);
        EditorGUILayout.PropertyField(gridHeight);
        EditorGUILayout.PropertyField(gridOffset);
        EditorGUILayout.PropertyField(gridCellSize);
        EditorGUILayout.EndVertical();

        if (selectedGridTilePos != GridPosition.Invalid) { }

        serializedObject.ApplyModifiedProperties();

        if (GUI.Button(EditorGUILayout.GetControlRect(
            GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 1.5f)),
            "Reset"))
        {
            //_levelGrid.Reset();
        }
    }
}
