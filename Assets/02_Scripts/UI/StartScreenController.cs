using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    private void Awake()
    {
        // Programmatically link the Fight and Exit buttons
        GameObject fightObj = GameObject.Find("FightButton");
        if (fightObj != null)
        {
            Button fightBtn = fightObj.GetComponent<Button>();
            if (fightBtn != null)
                fightBtn.onClick.AddListener(OnFightClicked);
        }

        GameObject exitObj = GameObject.Find("ExitButton");
        if (exitObj != null)
        {
            Button exitBtn = exitObj.GetComponent<Button>();
            if (exitBtn != null)
                exitBtn.onClick.AddListener(OnExitClicked);
        }
    }

    public void OnFightClicked()
    {
        // Attempt to clean the session slate upon a brand new match entry
        SessionData.ResetSession();

        // Load the specified target scene
        // NOTE: Please ensure your Arena 1 scene is literally named exactly 'Arena_Round1_DragonCourtyard'
        // in Unity's Build Settings, otherwise SceneManager will fail.
        SceneManager.LoadScene("Arena_Round1_DragonCourtyard");
    }

    public void OnExitClicked()
    {
        Debug.Log("Exiting Blood Arena...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
