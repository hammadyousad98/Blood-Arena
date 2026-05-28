using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateFolderStructure : Editor
{
    [MenuItem("Tools/BloodArena/Create Folder Structure")]
    public static void CreateFolders()
    {
        string[] folders = new string[]
        {
            "Assets/01_Scenes",
            "Assets/02_Scripts/Fighters",
            "Assets/02_Scripts/UI",
            "Assets/02_Scripts/Managers",
            "Assets/02_Scripts/Editor",
            "Assets/03_Assets/Audio",
            "Assets/03_Assets/Prefabs",
            "Assets/03_Assets/Materials",
            "Assets/03_Assets/ScriptableObjects",
            "Assets/04_InputActions"
        };

        foreach (string folder in folders)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Folder structure created successfully.");
    }
}
