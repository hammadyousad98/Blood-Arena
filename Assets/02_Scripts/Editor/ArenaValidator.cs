using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaValidator : EditorWindow
{
    [MenuItem("Tools/BloodArena/Validate All Arenas")]
    public static void ValidateArenas()
    {
        string[] scenesToValidate = {
            "Assets/Scenes/Arena_Round1.unity",
            "Assets/Scenes/Arena_Round2.unity",
            "Assets/Scenes/Arena_Round3.unity"
        };

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            foreach (var scenePath in scenesToValidate)
            {
                ValidateScene(scenePath);
            }
        }
    }

    private static void ValidateScene(string scenePath)
    {
        Debug.Log($"<b><color=cyan>--- Validating {scenePath} ---</color></b>");
        
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"FAIL: Could not open scene {scenePath}. Make sure it exists.");
            return;
        }

        // 1. Camera FOV = 65
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            if (Mathf.Approximately(mainCam.fieldOfView, 65f))
                Debug.Log("PASS: Camera FOV is exactly 65.");
            else
                Debug.LogError($"FAIL: Camera FOV is {mainCam.fieldOfView}, expected 65.");
        }
        else
        {
            Debug.LogError("FAIL: Main Camera not found in scene.");
        }

        // 2. Spawn points exist at correct positions
        GameObject p1Spawn = GameObject.Find("SpawnP1");
        GameObject p2Spawn = GameObject.Find("SpawnP2");

        if (p1Spawn != null && p1Spawn.CompareTag("SpawnPoint"))
        {
            if (Vector3.Distance(p1Spawn.transform.position, new Vector3(-5, 0, 0)) < 0.01f)
                Debug.Log("PASS: SpawnP1 is correctly placed at (-5, 0, 0).");
            else
                Debug.LogError($"FAIL: SpawnP1 position is {p1Spawn.transform.position}, expected (-5, 0, 0).");
        }
        else
        {
            Debug.LogError("FAIL: SpawnP1 not found or lacks 'SpawnPoint' tag.");
        }

        if (p2Spawn != null && p2Spawn.CompareTag("SpawnPoint"))
        {
            if (Vector3.Distance(p2Spawn.transform.position, new Vector3(5, 0, 0)) < 0.01f)
                Debug.Log("PASS: SpawnP2 is correctly placed at (5, 0, 0).");
            else
                Debug.LogError($"FAIL: SpawnP2 position is {p2Spawn.transform.position}, expected (5, 0, 0).");
        }
        else
        {
            Debug.LogError("FAIL: SpawnP2 not found or lacks 'SpawnPoint' tag.");
        }

        // 3. Fog is enabled
        if (RenderSettings.fog)
            Debug.Log("PASS: Fog is enabled.");
        else
            Debug.LogError("FAIL: Fog is disabled in RenderSettings.");

        // 4. Boundary walls exist
        string[] walls = { "Wall_North", "Wall_South", "Wall_East", "Wall_West" };
        bool allWallsFound = true;
        foreach (var wallName in walls)
        {
            GameObject wall = GameObject.Find(wallName);
            if (wall == null || wall.GetComponent<BoxCollider>() == null)
            {
                allWallsFound = false;
                Debug.LogError($"FAIL: {wallName} not found or missing a BoxCollider.");
            }
        }

        if (allWallsFound)
            Debug.Log("PASS: All 4 boundary walls exist and contain BoxColliders.");
    }
}
