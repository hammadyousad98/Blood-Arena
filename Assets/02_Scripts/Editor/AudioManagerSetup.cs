using UnityEditor;
using UnityEngine;

public class AudioManagerSetup : EditorWindow
{
    [MenuItem("Tools/BloodArena/Setup Audio Manager")]
    public static void SetupAudioManager()
    {
        // Check if an AudioManager already exists
        if (FindObjectOfType<AudioManager>() != null)
        {
            Debug.LogWarning("AudioManager already exists in the scene!");
            return;
        }

        GameObject audioManagerObj = new GameObject("AudioManager");
        AudioManager manager = audioManagerObj.AddComponent<AudioManager>();

        // Setup SFX Source
        GameObject sfxObj = new GameObject("SFX_Source");
        sfxObj.transform.SetParent(audioManagerObj.transform);
        AudioSource sfxSource = sfxObj.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = 1.0f;

        // Setup Music Source
        GameObject musicObj = new GameObject("Music_Source");
        musicObj.transform.SetParent(audioManagerObj.transform);
        AudioSource musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = 0.7f;

        // Assign serialized properties cleanly
        SerializedObject so = new SerializedObject(manager);
        so.FindProperty("_sfxSource").objectReferenceValue = sfxSource;
        so.FindProperty("_musicSource").objectReferenceValue = musicSource;
        so.ApplyModifiedProperties();

        Selection.activeGameObject = audioManagerObj;
        Debug.Log("AudioManager properly created and configured.");
    }
}
