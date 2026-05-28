using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSource;

    [Header("P1 SFX")]
    [SerializeField] private AudioClip sfxLightP1;
    [SerializeField] private AudioClip sfxHeavyP1;
    [SerializeField] private AudioClip sfxDodgeP1;
    [SerializeField] private AudioClip sfxBlockP1;

    [Header("P2 SFX")]
    [SerializeField] private AudioClip sfxLightP2;
    [SerializeField] private AudioClip sfxHeavyP2;
    [SerializeField] private AudioClip sfxDodgeP2;
    [SerializeField] private AudioClip sfxBlockP2;

    [Header("Combat SFX")]
    [SerializeField] private AudioClip sfxHitLand;
    [SerializeField] private AudioClip sfxHitHeavy;
    [SerializeField] private AudioClip sfxParry;
    [SerializeField] private AudioClip sfxRoundEnd;

    [Header("Match SFX")]
    [SerializeField] private AudioClip sfxVictory;
    [SerializeField] private AudioClip sfxDefeat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // NOTE: Make sure CombatEvents handles its delegates properly!
        CombatEvents.OnHitLanded += HandleHitLanded;
        CombatEvents.OnBlock += HandleBlock;
        CombatEvents.OnParry += HandleParry;
        
        RoundManager.OnMatchOver += HandleMatchOver;

        // Note: FighterController static events added directly in FighterController.cs
        FighterController.OnLightAttack += HandleLightAttack;
        FighterController.OnHeavyAttack += HandleHeavyAttack;
        FighterController.OnDodge += HandleDodge;
    }

    private void OnDisable()
    {
        CombatEvents.OnHitLanded -= HandleHitLanded;
        CombatEvents.OnBlock -= HandleBlock;
        CombatEvents.OnParry -= HandleParry;

        RoundManager.OnMatchOver -= HandleMatchOver;

        FighterController.OnLightAttack -= HandleLightAttack;
        FighterController.OnHeavyAttack -= HandleHeavyAttack;
        FighterController.OnDodge -= HandleDodge;
    }

    /// <summary>
    /// Plays an audio clip via PlayOneShot on the SFX source.
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Finds all AudioSources with loop = true in the scene and stops them.
    /// </summary>
    public void StopAllLoopingAudio()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (source.loop)
            {
                source.Stop();
            }
        }
    }

    // --- EVENT HANDLERS ---

    private void HandleHitLanded(MoveData move, bool isHeavy)
    {
        PlaySFX(isHeavy ? sfxHitHeavy : sfxHitLand);
    }

    private void HandleBlock(FighterController blocker)
    {
        bool isP1 = IsPlayer1(blocker);
        PlaySFX(isP1 ? sfxBlockP1 : sfxBlockP2);
    }

    private void HandleParry(FighterController parrier)
    {
        PlaySFX(sfxParry);
    }

    private void HandleMatchOver(string winnerName)
    {
        PlaySFX(sfxRoundEnd);
        StartCoroutine(PlayVictoryRoutine(winnerName));
    }

    private IEnumerator PlayVictoryRoutine(string winnerName)
    {
        // wait for 1.5s then play sfxVictory or Defeat (based on context)
        yield return new WaitForSecondsRealtime(1.5f);
        
        // Example check: Assuming "Player 1" represents the local primary user
        if (winnerName.Contains("1"))
            PlaySFX(sfxVictory);
        else
            PlaySFX(sfxDefeat);
    }

    private void HandleLightAttack(FighterController attacker)
    {
        bool isP1 = IsPlayer1(attacker);
        PlaySFX(isP1 ? sfxLightP1 : sfxLightP2);
    }

    private void HandleHeavyAttack(FighterController attacker)
    {
        bool isP1 = IsPlayer1(attacker);
        PlaySFX(isP1 ? sfxHeavyP1 : sfxHeavyP2);
    }

    private void HandleDodge(FighterController dodger)
    {
        bool isP1 = IsPlayer1(dodger);
        PlaySFX(isP1 ? sfxDodgeP1 : sfxDodgeP2);
    }

    /// <summary>
    /// Helper to identify player based on tag/naming context.
    /// </summary>
    private bool IsPlayer1(FighterController fighter)
    {
        if (fighter == null) return false;
        return fighter.gameObject.name.Contains("1") || fighter.CompareTag("Player1");
    }
}
