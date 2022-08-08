using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LevelGenerationSystem.CriticalPathMinMax))]
public class CriticalPathMinMaxDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var minProp = property.FindPropertyRelative("Min");
        var maxProp = property.FindPropertyRelative("Max");

        GUIContent[] multiIntLabels = { new GUIContent("Min"), new GUIContent("Max") };
        int[] multiIntValues = { minProp.intValue, maxProp.intValue };
        EditorGUI.MultiIntField(position, multiIntLabels, multiIntValues);
        minProp.intValue = multiIntValues[0] > 0 ? multiIntValues[0] : 0;
        maxProp.intValue = multiIntValues[1] > multiIntValues[0] ? multiIntValues[1] : multiIntValues[0];

        EditorGUI.EndProperty();
    }
}


[CustomEditor(typeof(LevelGenerationSystem))]
public class LevelGenerationSystemEditor : Editor
{
    GUIContent clearGUIContent = new GUIContent("Clear");
    GUIContent generateGUIContent = new GUIContent("Generate");

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var levelGen = (LevelGenerationSystem)target;

        var maxButtonWidth = GUI.skin.button.CalcSize(generateGUIContent).x;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(clearGUIContent, GUILayout.MaxWidth(maxButtonWidth)))
        {
            levelGen.Clear();
        }
        if (GUILayout.Button(generateGUIContent, GUILayout.MaxWidth(maxButtonWidth)))
        {
            levelGen.Generate();
        }
        EditorGUILayout.EndHorizontal();
    }
}
