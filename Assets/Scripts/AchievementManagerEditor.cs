using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AchievementManager))]
public class AchievementManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AchievementManager manager = (AchievementManager)target;

        if (GUILayout.Button("Add Default Achievement"))
        {
            manager.AddAchievement("New Achievement", "Description", "path/to/icon");
        }
    }
}
