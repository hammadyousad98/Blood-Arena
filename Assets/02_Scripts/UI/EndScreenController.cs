using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreenController : MonoBehaviour
{
    public Image background;
    public TMP_Text winnerLine1;
    public TMP_Text winnerLine2;

    [Header("Player 1")]
    public Image p1ResultPanel;
    public TMP_Text p1RoundWins;
    public TMP_Text p1ResultTag;
    public Image p1SkullIcon1;
    public Image p1SkullIcon2;

    [Header("Player 2")]
    public Image p2ResultPanel;
    public TMP_Text p2RoundWins;
    public TMP_Text p2ResultTag;
    public Image p2SkullIcon1;
    public Image p2SkullIcon2;

    public TMP_Text roundsFought;
    public ParticleSystem winnerParticles;

    public Button playAgainButton;
    public Button mainMenuButton;

    private void Start()
    {
        string winner = SessionData.winnerName; // "Player 1" or "Player 2"
        int p1Wins = SessionData.p1RoundWins;
        int p2Wins = SessionData.p2RoundWins;
        int rounds = SessionData.currentRound;

        // Populate winner banner
        if (winnerLine1 != null)
            winnerLine1.text = $"{winner} wins the duel!".ToUpper();

        ColorUtility.TryParseHtmlString("#F39C12", out Color gold);
        ColorUtility.TryParseHtmlString("#C0392B", out Color darkRed);
        ColorUtility.TryParseHtmlString("#2A1F00", out Color darkGoldPanel);
        ColorUtility.TryParseHtmlString("#1A0000", out Color darkRedPanel);

        // Populate P1 panel
        if (p1RoundWins != null) p1RoundWins.text = $"{p1Wins} ROUND{(p1Wins != 1 ? "S" : "")} WON";
        if (p1ResultTag != null)
        {
            p1ResultTag.text = winner == "Player 1" ? "WINNER" : "DEFEATED";
            p1ResultTag.color = winner == "Player 1" ? gold : darkRed;
        }
        if (p1ResultPanel != null)
        {
            p1ResultPanel.color = winner == "Player 1" ? darkGoldPanel : darkRedPanel;
        }

        // Populate P2 panel  
        if (p2RoundWins != null) p2RoundWins.text = $"{p2Wins} ROUND{(p2Wins != 1 ? "S" : "")} WON";
        if (p2ResultTag != null)
        {
            p2ResultTag.text = winner == "Player 2" ? "WINNER" : "DEFEATED";
            p2ResultTag.color = winner == "Player 2" ? gold : darkRed;
        }
        if (p2ResultPanel != null)
        {
            p2ResultPanel.color = winner == "Player 2" ? darkGoldPanel : darkRedPanel;
        }

        // Skull icons — fill based on round wins
        Color darkGrey = new Color(0.3f, 0.3f, 0.3f);
        if (p1SkullIcon1 != null) p1SkullIcon1.color = p1Wins >= 1 ? Color.white : darkGrey;
        if (p1SkullIcon2 != null) p1SkullIcon2.color = p1Wins >= 2 ? Color.white : darkGrey;
        if (p2SkullIcon1 != null) p2SkullIcon1.color = p2Wins >= 1 ? Color.white : darkGrey;
        if (p2SkullIcon2 != null) p2SkullIcon2.color = p2Wins >= 2 ? Color.white : darkGrey;

        // Background color shift
        if (background != null)
        {
            background.color = winner == "Player 1" 
                ? new Color(0.05f, 0.08f, 0.02f)   // dark green tint for P1 win
                : new Color(0.08f, 0.02f, 0.02f);  // dark red tint for P2 win
        }

        // Rounds fought
        if (roundsFought != null) roundsFought.text = $"ROUNDS FOUGHT: {rounds}";

        // Particles
        if (winnerParticles != null)
        {
            winnerParticles.Play();
        }

        if (playAgainButton != null) playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnPlayAgainClicked()
    {
        SessionData.ResetSession();
        SceneManager.LoadScene("Arena_Round1_DragonCourtyard");
    }

    private void OnMainMenuClicked()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
