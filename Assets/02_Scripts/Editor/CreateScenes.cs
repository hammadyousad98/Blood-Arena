using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class CreateScenes : Editor
{
    [MenuItem("Tools/BloodArena/Create All Scenes")]
    public static void CreateAllScenes()
    {
        string sceneDirectory = "Assets/01_Scenes";
        if (!Directory.Exists(sceneDirectory))
        {
            Directory.CreateDirectory(sceneDirectory);
        }

        string[] sceneNames = new string[]
        {
            "StartScreen",
            "Arena_Round1_DragonCourtyard",
            "Arena_Round2_CursedCrypt",
            "Arena_Round3_InfernalForge",
            "VictoryScreen",
            "DefeatScreen"
        };

        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();
        int createdCount = 0;
        int existingCount = 0;

        foreach (string sceneName in sceneNames)
        {
            string scenePath = $"{sceneDirectory}/{sceneName}.unity";
            
            // Check if scene exists to avoid overwriting
            if (!File.Exists(scenePath))
            {
                // Create new empty scene
                Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                // Save it
                EditorSceneManager.SaveScene(newScene, scenePath);
                createdCount++;
            }
            else
            {
                existingCount++;
            }

            // Add to build settings list
            buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }

        // Apply to build settings
        EditorBuildSettings.scenes = buildScenes.ToArray();
        
        AssetDatabase.Refresh();
        
        Debug.Log($"Scene generation complete. Created: {createdCount}, Already Existed: {existingCount}. All {buildScenes.Count} scenes added to Build Settings successfully.");
    }
}
