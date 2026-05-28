using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains static session data across rounds and matches.
/// </summary>
public static class SessionData
{
    public static int currentRound = 1;
    public static int p1RoundWins = 0;
    public static int p2RoundWins = 0;
    public static float p1CurrentHP = 100f;
    public static float p2CurrentHP = 100f;
    public static float roundElapsedSeconds = 0f;
    public static bool sessionActive = false;
    public static bool isPaused = false;
    public static string winnerName = "";
    public static bool isSuddenDeath = false;
    public static float suddenDeathElapsed = 0f;

    public static event Action OnSessionReset;

    public static void ResetSession()
    {
        currentRound = 1;
        p1RoundWins = 0;
        p2RoundWins = 0;
        p1CurrentHP = 100f;
        p2CurrentHP = 100f;
        roundElapsedSeconds = 0f;
        isSuddenDeath = false;
        suddenDeathElapsed = 0f;
        sessionActive = true;
        isPaused = false;
        winnerName = "";
        Time.timeScale = 1.0f;

        // Reset fighters via tag 'Fighter'
        GameObject[] fighterObjs = GameObject.FindGameObjectsWithTag("Fighter");
        foreach (GameObject fObj in fighterObjs)
        {
            FighterController fighter = fObj.GetComponent<FighterController>();
            if (fighter != null)
            {
                fighter.ResetFighter(fObj.transform.position, fObj.transform.rotation);
            }
        }

        // Stop all ParticleSystem components in the scene
        ParticleSystem[] particles = UnityEngine.Object.FindObjectsOfType<ParticleSystem>();
        foreach (ParticleSystem ps in particles)
        {
            ps.Stop();
        }

        // Stop all looping AudioSource components
        AudioSource[] audios = UnityEngine.Object.FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audios)
        {
            if (audio.loop)
            {
                audio.Stop();
            }
        }

        // Set all active screen overlay images to alpha 0
        UnityEngine.UI.Image[] images = UnityEngine.Object.FindObjectsOfType<UnityEngine.UI.Image>();
        foreach (UnityEngine.UI.Image img in images)
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }

        OnSessionReset?.Invoke();
    }
}

/// <summary>
/// Manages the full round and match lifecycle.
/// </summary>
public class RoundManager : MonoBehaviour
{
    [Header("Round Settings")]
    [Tooltip("Default duration for a round in seconds.")]
    public float defaultRoundDuration = 99f;

    // --- EVENTS ---
    public static event Action<int> OnTimerTick;
    public static event Action OnSuddenDeathStart;
    public static event Action<int> OnSuddenDeathTick;
    public static event Action OnRoundDraw;
    public static event Action<string> OnRoundWin; // winner name
    public static event Action<string> OnMatchOver; // winner name

    // Cache the coroutines so they can be stopped if needed
    private Coroutine roundTimerCoroutine;
    private Coroutine suddenDeathCoroutine;

    /// <summary>
    /// Resets HP, starts the timer, and marks the session as active.
    /// </summary>
    public void StartRound()
    {
        SessionData.p1CurrentHP = 100f;
        SessionData.p2CurrentHP = 100f;
        SessionData.roundElapsedSeconds = 0f;
        SessionData.sessionActive = true;
        SessionData.isSuddenDeath = false;

        if (roundTimerCoroutine != null)
            StopCoroutine(roundTimerCoroutine);

        roundTimerCoroutine = StartCoroutine(RunRoundTimer(defaultRoundDuration));
    }

    /// <summary>
    /// Evaluates who wins when the timer expires.
    /// </summary>
    private void EvaluateTimerExpiry()
    {
        if (SessionData.p1CurrentHP > SessionData.p2CurrentHP)
        {
            AwardRoundWin("Player 1");
        }
        else if (SessionData.p2CurrentHP > SessionData.p1CurrentHP)
        {
            AwardRoundWin("Player 2");
        }
        else
        {
            StartSuddenDeath();
        }
    }

    /// <summary>
    /// Starts the Sudden Death sequence.
    /// </summary>
    private void StartSuddenDeath()
    {
        SessionData.isSuddenDeath = true;
        SessionData.suddenDeathElapsed = 0f;
        OnSuddenDeathStart?.Invoke();

        if (suddenDeathCoroutine != null)
            StopCoroutine(suddenDeathCoroutine);

        suddenDeathCoroutine = StartCoroutine(SuddenDeathTimer());
    }

    /// <summary>
    /// Handles logic when a fighter is knocked out.
    /// </summary>
    /// <param name="loser">The FighterController of the defeated player.</param>
    public void OnFighterKO(FighterController loser)
    {
        // Stop any running timers
        if (roundTimerCoroutine != null) StopCoroutine(roundTimerCoroutine);
        if (suddenDeathCoroutine != null) StopCoroutine(suddenDeathCoroutine);

        SessionData.sessionActive = false;

        // Determine the winner based on the loser
        // Note: Replace the identification logic below if FighterController handles player IDs differently
        string winner = "";
        bool isLoserP1 = loser.gameObject.name.Contains("1") || loser.CompareTag("Player1");

        if (isLoserP1)
        {
            winner = "Player 2";
            SessionData.p2RoundWins++;
        }
        else
        {
            winner = "Player 1";
            SessionData.p1RoundWins++;
        }

        OnRoundWin?.Invoke(winner);

        if (!CheckMatchOver())
        {
            StartCoroutine(LoadNextArena());
        }
    }

    /// <summary>
    /// Checks if a player has reached 2 wins.
    /// </summary>
    /// <returns>True if the match is over, false otherwise.</returns>
    private bool CheckMatchOver()
    {
        if (SessionData.p1RoundWins >= 2)
        {
            SessionData.winnerName = "Player 1";
            OnMatchOver?.Invoke(SessionData.winnerName);
            return true;
        }
        else if (SessionData.p2RoundWins >= 2)
        {
            SessionData.winnerName = "Player 2";
            OnMatchOver?.Invoke(SessionData.winnerName);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Awards a round win to a specific player and processes end-of-round logic.
    /// </summary>
    /// <param name="winnerName">The name of the winning player.</param>
    private void AwardRoundWin(string winnerName)
    {
        SessionData.sessionActive = false;

        if (winnerName == "Player 1")
            SessionData.p1RoundWins++;
        else if (winnerName == "Player 2")
            SessionData.p2RoundWins++;

        OnRoundWin?.Invoke(winnerName);

        if (!CheckMatchOver())
        {
            StartCoroutine(LoadNextArena());
        }
    }

    /// <summary>
    /// Declares a draw (no round win awarded) and proceeds to the next arena.
    /// </summary>
    private void DeclareRoundDraw()
    {
        SessionData.sessionActive = false;
        OnRoundDraw?.Invoke();
        StartCoroutine(LoadNextArena());
    }

    // --- COROUTINES ---

    /// <summary>
    /// The main round timer using WaitForSecondsRealtime.
    /// </summary>
    private IEnumerator RunRoundTimer(float duration)
    {
        float remaining = duration;

        while (remaining > 0)
        {
            // Pause handling: yield null while paused
            while (SessionData.isPaused)
            {
                yield return null;
            }

            // Wait 1 real second
            yield return new WaitForSecondsRealtime(1f);

            // If it was paused during the wait, do not decrement yet
            if (SessionData.isPaused)
            {
                continue;
            }

            remaining--;
            SessionData.roundElapsedSeconds++;
            OnTimerTick?.Invoke((int)remaining);
        }

        EvaluateTimerExpiry();
    }

    /// <summary>
    /// The sudden death timer, lasting 30 seconds.
    /// </summary>
    private IEnumerator SuddenDeathTimer()
    {
        float remaining = 30f;

        while (remaining > 0)
        {
            while (SessionData.isPaused)
            {
                yield return null;
            }

            yield return new WaitForSecondsRealtime(1f);

            if (SessionData.isPaused)
            {
                continue;
            }

            remaining--;
            SessionData.suddenDeathElapsed++;
            OnSuddenDeathTick?.Invoke((int)remaining);
        }

        DeclareRoundDraw();
    }

    /// <summary>
    /// Waits 2 seconds, increments the round, and loads the correct arena scene.
    /// </summary>
    private IEnumerator LoadNextArena()
    {
        yield return new WaitForSecondsRealtime(2f);

        SessionData.currentRound++;

        // Load the correct arena scene
        string sceneName = "Arena_Round" + SessionData.currentRound;
        SceneManager.LoadScene(sceneName);
    }

    // --- PAUSE LOGIC ---

    /// <summary>
    /// Pauses the game logic and time scale.
    /// </summary>
    public void PauseGame()
    {
        SessionData.isPaused = true;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Resumes the game logic and time scale.
    /// </summary>
    public void ResumeGame()
    {
        SessionData.isPaused = false;
        Time.timeScale = 1f;
    }
}
