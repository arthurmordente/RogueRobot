using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AchievementManager))]
public class AchievementManagerEditor : Editor
{
    private string achievementName = "New Achievement";
    private string achievementDescription = "Description";
    private string achievementIconPath = "path/to/icon";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AchievementManager manager = (AchievementManager)target;

        achievementName = EditorGUILayout.TextField("Name", achievementName);
        achievementDescription = EditorGUILayout.TextField("Description", achievementDescription);
        achievementIconPath = EditorGUILayout.TextField("Icon Path", achievementIconPath);

        if (GUILayout.Button("Add Achievement"))
        {
            manager.AddAchievement(achievementName, achievementDescription, achievementIconPath);
        }
    }
}
