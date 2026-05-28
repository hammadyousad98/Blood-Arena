using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HUDController : MonoBehaviour
{
    private RectTransform p1Fill;
    private RectTransform p2Fill;
    private Image p1Skull1, p1Skull2;
    private Image p2Skull1, p2Skull2;
    private TextMeshProUGUI timerText;
    private TextMeshProUGUI roundCounterText;
    private TextMeshProUGUI feedbackPopup;
    private Button pauseButton;

    private void Awake()
    {
        // Bind elements
        GameObject canvas = GameObject.Find("HUD_Canvas");
        if (canvas == null)
        {
            Debug.LogError("HUD_Canvas not found in the scene.");
            return;
        }

        Transform p1HP = canvas.transform.Find("HPBar_P1/Fill");
        if (p1HP != null) p1Fill = p1HP.GetComponent<RectTransform>();

        Transform p2HP = canvas.transform.Find("HPBar_P2/Fill");
        if (p2HP != null) p2Fill = p2HP.GetComponent<RectTransform>();

        Transform p1Wins = canvas.transform.Find("RoundWins_P1");
        if (p1Wins != null)
        {
            p1Skull1 = p1Wins.Find("Skull1_P1")?.GetComponent<Image>();
            p1Skull2 = p1Wins.Find("Skull2_P1")?.GetComponent<Image>();
        }

        Transform p2Wins = canvas.transform.Find("RoundWins_P2");
        if (p2Wins != null)
        {
            p2Skull1 = p2Wins.Find("Skull1_P2")?.GetComponent<Image>();
            p2Skull2 = p2Wins.Find("Skull2_P2")?.GetComponent<Image>();
        }

        timerText = canvas.transform.Find("TimerText")?.GetComponent<TextMeshProUGUI>();
        roundCounterText = canvas.transform.Find("RoundCounterText")?.GetComponent<TextMeshProUGUI>();
        feedbackPopup = canvas.transform.Find("FeedbackPopup")?.GetComponent<TextMeshProUGUI>();
        
        Transform pauseBtn = canvas.transform.Find("PauseButton");
        if (pauseBtn != null)
        {
            pauseButton = pauseBtn.GetComponent<Button>();
            RoundManager rm = Object.FindObjectOfType<RoundManager>();
            if (rm != null)
            {
                pauseButton.onClick.AddListener(() => {
                    if (SessionData.isPaused) rm.ResumeGame();
                    else rm.PauseGame();
                });
            }
        }
    }

    private void OnEnable()
    {
        RoundManager.OnTimerTick += SetTimer;
        RoundManager.OnRoundWin += HandleRoundWin;
        RoundManager.OnMatchOver += HandleMatchOver;
        RoundManager.OnSuddenDeathStart += ShowSuddenDeath;

        // NOTE: Please adjust FighterController event bindings if your signatures are different!
        // FighterController.OnP1HPChanged += UpdateP1HP;
        // FighterController.OnP2HPChanged += UpdateP2HP;
    }

    private void OnDisable()
    {
        RoundManager.OnTimerTick -= SetTimer;
        RoundManager.OnRoundWin -= HandleRoundWin;
        RoundManager.OnMatchOver -= HandleMatchOver;
        RoundManager.OnSuddenDeathStart -= ShowSuddenDeath;

        // FighterController.OnP1HPChanged -= UpdateP1HP;
        // FighterController.OnP2HPChanged -= UpdateP2HP;
    }

    private void HandleRoundWin(string winnerName)
    {
        StartCoroutine(ShowFeedback(winnerName + " WINS!", Color.white));
    }

    private void HandleMatchOver(string winnerName)
    {
        StartCoroutine(ShowFeedback(winnerName + " WINS THE MATCH!", Color.yellow));
    }

    public void UpdateP1HP(float hp01)
    {
        if (p1Fill != null)
        {
            p1Fill.localScale = new Vector3(Mathf.Clamp01(hp01), 1f, 1f);
        }
    }

    public void UpdateP2HP(float hp01)
    {
        if (p2Fill != null)
        {
            p2Fill.localScale = new Vector3(Mathf.Clamp01(hp01), 1f, 1f);
        }
    }

    public void SetP1Wins(int wins)
    {
        if (p1Skull1 != null) p1Skull1.color = wins >= 1 ? Color.yellow : Color.gray;
        if (p1Skull2 != null) p1Skull2.color = wins >= 2 ? Color.yellow : Color.gray;
    }

    public void SetP2Wins(int wins)
    {
        if (p2Skull1 != null) p2Skull1.color = wins >= 1 ? Color.yellow : Color.gray;
        if (p2Skull2 != null) p2Skull2.color = wins >= 2 ? Color.yellow : Color.gray;
    }

    public void SetTimer(int seconds)
    {
        if (timerText == null) return;
        
        int m = Mathf.Max(0, seconds) / 60;
        int s = Mathf.Max(0, seconds) % 60;
        timerText.text = string.Format("{0}:{1:00}", m, s);
        timerText.color = seconds < 15 ? Color.red : Color.white;
    }

    public void SetRound(int round)
    {
        if (roundCounterText != null)
        {
            roundCounterText.text = "ROUND " + round;
        }
    }

    public IEnumerator ShowFeedback(string text, Color color)
    {
        if (feedbackPopup == null) yield break;

        feedbackPopup.text = text;
        feedbackPopup.color = new Color(color.r, color.g, color.b, 1f);

        // Show for 1 second
        yield return new WaitForSeconds(1f);

        // Fade out
        float fadeDuration = 1f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            feedbackPopup.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }

        feedbackPopup.color = new Color(color.r, color.g, color.b, 0f);
    }

    public void ShowSuddenDeath()
    {
        if (roundCounterText != null)
        {
            roundCounterText.text = "SUDDEN DEATH";
            ColorUtility.TryParseHtmlString("#F39C12", out Color gold);
            roundCounterText.color = gold;
        }
        StartCoroutine(ShowFeedback("SUDDEN DEATH!", Color.red));
    }
}
