using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArenaController : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Ambient background loop for the arena.")]
    [SerializeField] private AudioSource ambientLoopSource;

    [Header("Fade UI")]
    [Tooltip("Black full-screen image for transitions.")]
    [SerializeField] private Image fadeImage;

    public FighterSpawner spawner;

    private void Awake()
    {
        // Lock game to 60 FPS for consistent fighting game logic
        Application.targetFrameRate = 60;

        // 1. Play background music
        if (ambientLoopSource != null)
        {
            ambientLoopSource.loop = true;
            ambientLoopSource.Play();
        }

        // 2. Find spawn points and spawn fighters
        // GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        // Transform p1Spawn = null;
        // Transform p2Spawn = null;

        // foreach (var sp in spawnPoints)
        // {
        //     if (sp.name.Contains("P1")) p1Spawn = sp.transform;
        //     if (sp.name.Contains("P2")) p2Spawn = sp.transform;
        // }

        // if (p1Spawn != null && p2Spawn != null)
        // {
        //     // Assuming FighterSpawner exists and handles instantiation/resetting
        //     FighterSpawner.SpawnFighters();
        //     Debug.Log($"ArenaController: FighterSpawner.SpawnFighters called for {p1Spawn.name} and {p2Spawn.name}");
        // }
        // else
        // {
        //     Debug.LogWarning("ArenaController: Could not find SpawnP1 or SpawnP2 tagged as 'SpawnPoint'.");
        // }
        spawner.SpawnFighters();

        // Start Round
        RoundManager rm = Object.FindObjectOfType<RoundManager>();
        if (rm != null)
        {
            rm.StartRound();
        }

        // 3. Fade In
        StartCoroutine(FadeRoutine(1f, 0f));
    }

    private void OnDestroy()
    {
        // Stop background music on unload
        if (ambientLoopSource != null)
        {
            ambientLoopSource.Stop();
        }
    }

    private void OnEnable()
    {
        RoundManager.OnMatchOver += HandleMatchOver;
    }

    private void OnDisable()
    {
        RoundManager.OnMatchOver -= HandleMatchOver;
    }

    private void HandleMatchOver(string winnerName)
    {
        StartCoroutine(FadeOutAndLoadScreen(winnerName));
    }

    /// <summary>
    /// Smooth fade routine utilizing unscaled time (realtime) for smooth alpha lerp.
    /// </summary>
    private IEnumerator FadeRoutine(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);
        Color c = fadeImage.color;
        c.a = startAlpha;
        fadeImage.color = c;

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            fadeImage.color = c;
            yield return null; // Using null + unscaledDeltaTime for smooth real-time fade
        }

        c.a = endAlpha;
        fadeImage.color = c;
        
        if (endAlpha == 0f)
            fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeOutAndLoadScreen(string winnerName)
    {
        // 1. Fade Out
        yield return StartCoroutine(FadeRoutine(0f, 1f));

        // Wait an extra second if needed or just transition
        yield return new WaitForSecondsRealtime(0.5f);

        // 2. Load Victory or Defeat screen based on the winner
        // This assumes Player 1 represents the local user (Victory), otherwise Defeat.
        string nextScene = "EndScreen";
        // if (!string.IsNullOrEmpty(winnerName))
        // {
        //     nextScene = winnerName.Contains("1") ? "VictoryScreen" : "DefeatScreen";
        // }
        // else
        // {
        //     nextScene = SessionData.winnerName.Contains("1") ? "VictoryScreen" : "DefeatScreen";
        // }

        SceneManager.LoadScene(nextScene);
    }
}
