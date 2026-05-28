using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private GameObject pausePanel;

    private void Awake()
    {
        Transform panel = transform.Find("PausePanel");
        if (panel != null)
        {
            pausePanel = panel.gameObject;

            Transform resumeBtn = panel.Find("ResumeButton");
            if (resumeBtn != null)
                resumeBtn.GetComponent<Button>()?.onClick.AddListener(OnResumeClicked);

            Transform restartBtn = panel.Find("RestartButton");
            if (restartBtn != null)
                restartBtn.GetComponent<Button>()?.onClick.AddListener(OnRestartClicked);

            Transform menuBtn = panel.Find("MainMenuButton");
            if (menuBtn != null)
                menuBtn.GetComponent<Button>()?.onClick.AddListener(OnMainMenuClicked);
        }
    }

    /// <summary>
    /// Intended to be triggered by the existing HUD Pause Button.
    /// Opens the panel if pausing, closes it if resuming.
    /// </summary>
    public void TogglePauseMenu()
    {
        RoundManager rm = Object.FindObjectOfType<RoundManager>();
        if (rm != null)
        {
            if (SessionData.isPaused)
            {
                OnResumeClicked();
            }
            else
            {
                rm.PauseGame();
                if (pausePanel != null) pausePanel.SetActive(true);
            }
        }
    }

    public void OnResumeClicked()
    {
        RoundManager rm = Object.FindObjectOfType<RoundManager>();
        if (rm != null)
        {
            rm.ResumeGame();
        }

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void OnRestartClicked()
    {
        SessionData.ResetSession();
        
        // Ensure unpaused state before reloading the arena
        Time.timeScale = 1f;
        SceneManager.LoadScene("Arena_Round1");
    }

    public void OnMainMenuClicked()
    {
        // Ensure unpaused state before loading the menu
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScreen");
    }
}
