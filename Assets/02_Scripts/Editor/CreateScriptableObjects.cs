using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateScriptableObjects : Editor
{
    [MenuItem("Tools/BloodArena/Create ScriptableObjects")]
    public static void CreateSOs()
    {
        string directoryPath = "Assets/03_Assets/ScriptableObjects";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh();
        }

        FighterDatabase fighterDatabase = ScriptableObject.CreateInstance<FighterDatabase>();
        AssetDatabase.CreateAsset(fighterDatabase, $"{directoryPath}/BA_FighterDatabase_v1_2026-05-22.asset");

        SessionConfig sessionConfig = ScriptableObject.CreateInstance<SessionConfig>();
        AssetDatabase.CreateAsset(sessionConfig, $"{directoryPath}/BA_SessionConfig_v1_2026-05-22.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("ScriptableObjects created successfully.");
    }
}
