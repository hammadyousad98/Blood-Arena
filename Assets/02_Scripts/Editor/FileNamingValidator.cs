using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class FileNamingValidator : EditorWindow
{
    [MenuItem("Tools/BloodArena/Validate File Naming")]
    public static void ValidateNaming()
    {
        int passed = 0;
        int failed = 0;

        Debug.Log("<b>--- Starting Blood Arena Validation ---</b>");

        // 1. Validate Files
        string[] foldersToScan = new string[]
        {
            "Assets/03_Assets/ScriptableObjects",
            "Assets/03_Assets/Audio"
        };

        foreach (string folder in foldersToScan)
        {
            if (!Directory.Exists(folder))
            {
                Debug.LogWarning($"Directory does not exist (skipping): {folder}");
                continue;
            }

            string[] files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (file.EndsWith(".meta")) continue;

                string fileName = Path.GetFileNameWithoutExtension(file);
                bool isValid = true;
                List<string> errorReasons = new List<string>();

                // Check 1: Starts with 'BA_'
                if (!fileName.StartsWith("BA_"))
                {
                    isValid = false;
                    errorReasons.Add("Does not start with 'BA_'");
                }

                // Check 2: No spaces
                if (fileName.Contains(" "))
                {
                    isValid = false;
                    errorReasons.Add("Contains spaces");
                }

                // Check 3: No uppercase after the first prefix
                if (fileName.StartsWith("BA_"))
                {
                    string afterPrefix = fileName.Substring(3);
                    if (Regex.IsMatch(afterPrefix, "[A-Z]"))
                    {
                        isValid = false;
                        errorReasons.Add("Contains uppercase letters after 'BA_'");
                    }
                }

                // Check 4: Has a version string
                if (!Regex.IsMatch(fileName, @"_v\d+"))
                {
                    isValid = false;
                    errorReasons.Add("Missing version string (e.g., _v1)");
                }
                
                // Check 5 (Implicit from pattern): Has date string [YYYY-MM-DD]
                if (!Regex.IsMatch(fileName, @"_\d{4}-\d{2}-\d{2}$"))
                {
                    isValid = false;
                    errorReasons.Add("Missing or invalid date string suffix (e.g., _YYYY-MM-DD)");
                }

                if (isValid)
                {
                    passed++;
                }
                else
                {
                    failed++;
                    Debug.LogWarning($"FAIL: {file}\nReasons: {string.Join(", ", errorReasons)}");
                }
            }
        }

        // 2. Validate Build Settings (Scenes)
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        if (scenes.Length == 6)
        {
            Debug.Log("PASS: Build Settings contains exactly 6 scenes.");
            passed++;
        }
        else
        {
            Debug.LogWarning($"FAIL: Build Settings contains {scenes.Length} scenes. Expected exactly 6.");
            failed++;
        }

        // Summary
        Debug.Log($"<b>--- Validation Complete | Passed: {passed} | Failed: {failed} ---</b>");
    }
}
