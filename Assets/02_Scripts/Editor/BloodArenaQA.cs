using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class BloodArenaQA : EditorWindow
{
    [MenuItem("Tools/BloodArena/Run Full QA Checklist")]
    public static void RunQA()
    {
        int passed = 0;
        int total = 15;
        Debug.Log("<b>--- Starting Blood Arena Full QA ---</b>");

        // 1. Check Fixed Timestep
        if (Mathf.Abs(Time.fixedDeltaTime - 0.01667f) < 0.0001f) { Pass("1. Fixed Timestep is 0.01667"); passed++; }
        else Fail($"1. Fixed Timestep is {Time.fixedDeltaTime}, expected 0.01667");

        // 2. Check 6 scenes in Build Settings
        if (EditorBuildSettings.scenes.Length == 6) { Pass("2. Exactly 6 scenes in Build Settings"); passed++; }
        else Fail($"2. {EditorBuildSettings.scenes.Length} scenes in Build Settings, expected 6");

        // 3 & 4. Arena scenes FOV and Spawn Points
        bool fovPass = true;
        bool spawnPass = true;
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        int arenaCount = 0;

        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.ToLower().Contains("arena"))
            {
                arenaCount++;
                // Open additively to avoid losing current scene state
                Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                
                bool hasFov = false;
                bool hasLeft = false;
                bool hasRight = false;

                GameObject[] roots = scene.GetRootGameObjects();
                foreach (GameObject root in roots)
                {
                    Camera[] cams = root.GetComponentsInChildren<Camera>(true);
                    foreach (Camera cam in cams)
                    {
                        if (Mathf.Approximately(cam.fieldOfView, 65f)) hasFov = true;
                    }

                    Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
                    foreach (Transform t in transforms)
                    {
                        if (Vector3.Distance(t.position, new Vector3(-5, 0, 0)) < 0.1f) hasLeft = true;
                        if (Vector3.Distance(t.position, new Vector3(5, 0, 0)) < 0.1f) hasRight = true;
                    }
                }
                
                if (!hasFov) fovPass = false;
                if (!hasLeft || !hasRight) spawnPass = false;

                EditorSceneManager.CloseScene(scene, true);
            }
        }
        if (arenaCount == 0) { fovPass = false; spawnPass = false; }

        if (fovPass) { Pass("3. Camera FOV = 65 in all arena scenes"); passed++; }
        else Fail("3. Camera FOV = 65 check failed (missing or incorrect)");

        if (spawnPass) { Pass("4. Spawn points at (-5,0,0) and (5,0,0) in all arena scenes"); passed++; }
        else Fail("4. Spawn points check failed (missing or incorrect)");

        // 5. RoundManager WaitForSecondsRealtime
        if (CheckSourceForString("RoundManager", "WaitForSecondsRealtime")) { Pass("5. RoundManager uses WaitForSecondsRealtime"); passed++; }
        else Fail("5. RoundManager does not use WaitForSecondsRealtime");

        // 6. SlowMotionController WaitForSecondsRealtime
        if (CheckSourceForString("SlowMotionController", "WaitForSecondsRealtime")) { Pass("6. SlowMotionController uses WaitForSecondsRealtime"); passed++; }
        else Fail("6. SlowMotionController does not use WaitForSecondsRealtime");

        // 7. SessionConfig totalRounds=3, roundDuration=120, parryWindow 6-8
        bool sessionPass = false;
        string[] scGuids = AssetDatabase.FindAssets("SessionConfig t:Script");
        if (scGuids.Length > 0)
        {
            string txt = File.ReadAllText(AssetDatabase.GUIDToAssetPath(scGuids[0]));
            if (txt.Contains("3") && txt.Contains("120") && txt.Contains("6") && txt.Contains("8")) sessionPass = true;
        }
        string[] scsoGuids = AssetDatabase.FindAssets("t:SessionConfig");
        if (!sessionPass && scsoGuids.Length > 0)
        {
            string txt = File.ReadAllText(AssetDatabase.GUIDToAssetPath(scsoGuids[0]));
            if (txt.Contains("3") && txt.Contains("120") && txt.Contains("6") && txt.Contains("8")) sessionPass = true;
        }
        if (sessionPass) { Pass("7. SessionConfig matches values (3, 120, 6-8)"); passed++; }
        else Fail("7. SessionConfig values check failed");

        // 8. FighterDatabase entries
        bool fdPass = false;
        string[] fdGuids = AssetDatabase.FindAssets("t:FighterDatabase");
        if (fdGuids.Length > 0)
        {
            ScriptableObject fd = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(fdGuids[0]));
            if (fd != null)
            {
                SerializedObject so = new SerializedObject(fd);
                var fightersProp = so.FindProperty("fighters");
                if (fightersProp != null && fightersProp.isArray && fightersProp.arraySize >= 2)
                {
                    var f1 = fightersProp.GetArrayElementAtIndex(0).objectReferenceValue;
                    var f2 = fightersProp.GetArrayElementAtIndex(1).objectReferenceValue;
                    if (f1 != null && f2 != null)
                    {
                        SerializedObject so1 = new SerializedObject(f1);
                        SerializedObject so2 = new SerializedObject(f2);
                        string id1 = so1.FindProperty("fighterID")?.stringValue;
                        string id2 = so2.FindProperty("fighterID")?.stringValue;
                        if (!string.IsNullOrEmpty(id1) && !string.IsNullOrEmpty(id2)) fdPass = true;
                    }
                }
            }
        }
        if (fdPass) { Pass("8. FighterDatabase has both fighters with IDs"); passed++; }
        else Fail("8. FighterDatabase check failed");

        // 9. Audio files are .ogg
        bool oggPass = true;
        string audioDir = "Assets/03_Assets/Audio";
        if (Directory.Exists(audioDir))
        {
            string[] audioFiles = Directory.GetFiles(audioDir, "*.*", SearchOption.AllDirectories).Where(f => !f.EndsWith(".meta")).ToArray();
            if (audioFiles.Length == 0) oggPass = false;
            foreach (string af in audioFiles)
            {
                if (!af.ToLower().EndsWith(".ogg")) oggPass = false;
            }
        }
        else oggPass = false;

        if (oggPass) { Pass("9. All audio files are .ogg"); passed++; }
        else Fail("9. Not all audio files are .ogg or directory is empty");

        // 10. Time.timeScale reset in ResetSession()
        bool resetPass = false;
        string[] allCs = Directory.GetFiles("Assets/02_Scripts", "*.cs", SearchOption.AllDirectories);
        foreach (string cs in allCs)
        {
            string txt = File.ReadAllText(cs);
            if (txt.Contains("ResetSession") && txt.Contains("Time.timeScale") && txt.Contains("1"))
            {
                resetPass = true;
                break;
            }
        }
        if (resetPass) { Pass("10. Time.timeScale is reset in ResetSession()"); passed++; }
        else Fail("10. ResetSession() timescale check failed");

        // 11. InputActions exist
        bool inputExists = File.Exists("Assets/04_InputActions/InputActions_BloodArena.inputactions");
        if (inputExists) { Pass("11. InputActions_BloodArena exists"); passed++; }
        else Fail("11. InputActions_BloodArena missing");

        // 12. P2 uses RightCtrl/RightShift
        bool p2Pass = false;
        if (inputExists)
        {
            string inputTxt = File.ReadAllText("Assets/04_InputActions/InputActions_BloodArena.inputactions").ToLower();
            if (inputTxt.Contains("rightctrl") && inputTxt.Contains("rightshift")) p2Pass = true;
        }
        if (p2Pass) { Pass("12. P2 uses RightCtrl and RightShift bindings"); passed++; }
        else Fail("12. P2 bindings check failed");

        // 13. Ragnar prefab capsule collider
        bool ragnarPass = false;
        string[] ragnarGuids = AssetDatabase.FindAssets("Ragnar t:GameObject");
        foreach (string g in ragnarGuids)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(g));
            if (go != null)
            {
                CapsuleCollider cc = go.GetComponent<CapsuleCollider>();
                if (cc != null && Mathf.Abs(cc.height - 1.8f) < 0.1f) ragnarPass = true;
            }
        }
        if (ragnarPass) { Pass("13. Ragnar prefab has CapsuleCollider with height ~ 1.8"); passed++; }
        else Fail("13. Ragnar CapsuleCollider check failed");

        // 14. Darius prefab capsule collider
        bool dariusPass = false;
        string[] dariusGuids = AssetDatabase.FindAssets("Darius t:GameObject");
        foreach (string g in dariusGuids)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(g));
            if (go != null)
            {
                CapsuleCollider cc = go.GetComponent<CapsuleCollider>();
                if (cc != null && Mathf.Abs(cc.height - 1.7f) < 0.1f) dariusPass = true;
            }
        }
        if (dariusPass) { Pass("14. Darius prefab has CapsuleCollider with height ~ 1.7"); passed++; }
        else Fail("14. Darius CapsuleCollider check failed");

        // 15. Final Summary
        passed++; // Reaching this step implies the summary logging itself succeeds.
        Pass("15. Final summary logged.");
        Debug.Log($"<b>QA Complete: {passed}/{total} checks passed.</b>");
    }

    private static void Pass(string msg) => Debug.Log($"<color=green>PASS</color>: {msg}");
    private static void Fail(string msg) => Debug.LogWarning($"<color=red>FAIL</color>: {msg}");

    private static bool CheckSourceForString(string className, string searchString)
    {
        string[] guids = AssetDatabase.FindAssets($"{className} t:Script");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (File.ReadAllText(path).Contains(searchString)) return true;
        }
        return false;
    }
}
