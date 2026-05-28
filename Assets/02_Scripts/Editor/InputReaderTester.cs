using UnityEngine;
using UnityEditor;

public class InputReaderTester : EditorWindow
{
    [MenuItem("Window/BloodArena/Input Debug")]
    public static void ShowWindow()
    {
        GetWindow<InputReaderTester>("Input Debug");
    }

    private void OnInspectorUpdate()
    {
        Repaint(); // Forces window to update frequently
    }

    private void OnGUI()
    {
        GUILayout.Label("Real-time Input State", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to view real-time input states.", MessageType.Info);
            return;
        }

        InputReader[] readers = Object.FindObjectsByType<InputReader>(FindObjectsSortMode.None);
        
        if (readers.Length == 0)
        {
            EditorGUILayout.HelpBox("No InputReader components found in the scene.", MessageType.Warning);
            return;
        }

        foreach (var reader in readers)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label($"Player: {reader.playerIndex}", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            EditorGUILayout.Toggle("Is Moving", reader.IsMoving);
            EditorGUILayout.Toggle("Is Blocking", reader.IsBlocking);
            EditorGUILayout.Toggle("Light Attack", reader.IsLightAttacking);
            EditorGUILayout.Toggle("Heavy Attack", reader.IsHeavyAttacking);
            EditorGUILayout.Toggle("Dodge", reader.IsDodging);
            EditorGUI.indentLevel--;
            
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }
    }
}
