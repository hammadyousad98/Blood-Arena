using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreenController : MonoBehaviour
{
    private void Start()
    {
        TextMeshProUGUI line1 = transform.Find("Line1Text")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI roundsText = transform.Find("RoundsFought")?.GetComponent<TextMeshProUGUI>();

        if (line1 != null)
        {
            string name = string.IsNullOrEmpty(SessionData.winnerName) ? "Player" : SessionData.winnerName;
            
            // Check if this script is sitting on the Defeat Screen via the text component
            if (line1.text.Contains("fallen"))
            {
                // In Defeat, find the loser based on who the winner was
                string loser = name.Contains("1") ? "Player 2" : "Player 1";
                line1.text = $"{loser} has fallen!";
            }
            else
            {
                // Victory Screen behavior
                line1.text = $"{name} wins the duel!";
            }
        }

        if (roundsText != null)
        {
            roundsText.text = $"Rounds Fought: {SessionData.currentRound}";
        }

        // Attach buttons programmatically
        Transform playBtn = transform.Find("PlayAgainButton");
        if (playBtn != null)
            playBtn.GetComponent<Button>()?.onClick.AddListener(OnPlayAgainClicked);

        Transform menuBtn = transform.Find("MainMenuButton");
        if (menuBtn != null)
            menuBtn.GetComponent<Button>()?.onClick.AddListener(OnMainMenuClicked);
    }

    public void OnPlayAgainClicked()
    {
        // Reset the slate and load standard Arena_Round1 directly as specified
        SessionData.ResetSession();
        SceneManager.LoadScene("Arena_Round1");
    }

    public void OnMainMenuClicked()
    {
        // Head back to the Start Screen
        SceneManager.LoadScene("StartScreen");
    }
}
