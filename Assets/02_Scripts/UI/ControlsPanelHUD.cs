using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Creates and manages control hint panels below each player's HP bar on the HUD.
/// Attach to the same GameObject as HUDController or place anywhere in the arena scene.
/// </summary>
public class ControlsPanelHUD : MonoBehaviour
{
    private GameObject p1Panel;
    private GameObject p2Panel;

    private void Start()
    {
        GameObject canvas = GameObject.Find("HUD_Canvas");
        if (canvas == null)
        {
            Debug.LogWarning("ControlsPanelHUD: HUD_Canvas not found.");
            return;
        }

        // Build P1 controls panel — anchored below HPBar_P1
        p1Panel = CreateControlsPanel(
            canvas.transform,
            "ControlsPanel_P1",
            new Vector2(0f, 1f),   // anchor top-left
            new Vector2(0f, 1f),
            new Vector2(10f, -60f), // position below HP bar
            TextAnchor.UpperLeft,
            BuildP1ControlsText()
        );

        // Build P2 controls panel — anchored below HPBar_P2
        p2Panel = CreateControlsPanel(
            canvas.transform,
            "ControlsPanel_P2",
            new Vector2(1f, 1f),   // anchor top-right
            new Vector2(1f, 1f),
            new Vector2(-10f, -60f), // position below HP bar
            TextAnchor.UpperRight,
            BuildP2ControlsText()
        );
    }

    private string BuildP1ControlsText()
    {
        return "<color=#FFD700>P1 CONTROLS</color>\n" +
               "<color=#AAAAAA>" +
               "WASD  <color=#FFFFFF>Move</color>\n" +
               "Q     <color=#FFFFFF>Light Attack</color>\n" +
               "F     <color=#FFFFFF>Heavy Attack</color>\n" +
               "E     <color=#FFFFFF>Block</color>\n" +
               "C     <color=#FFFFFF>Dodge</color>\n" +
               "V     <color=#FFFFFF>Face Cam</color>" +
               "</color>";
    }

    private string BuildP2ControlsText()
    {
        return "<color=#FFD700>P2 CONTROLS</color>\n" +
               "<color=#AAAAAA>" +
               "Arrows    <color=#FFFFFF>Move</color>\n" +
               "R.Ctrl    <color=#FFFFFF>Light Attack</color>\n" +
               "P         <color=#FFFFFF>Heavy Attack</color>\n" +
               "R.Shift   <color=#FFFFFF>Block</color>\n" +
               "O         <color=#FFFFFF>Dodge</color>\n" +
               "L         <color=#FFFFFF>Face Cam</color>" +
               "</color>";
    }

    private GameObject CreateControlsPanel(
        Transform parent,
        string panelName,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPos,
        TextAnchor alignment,
        string controlsText)
    {
        // Background panel
        GameObject panelGO = new GameObject(panelName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelGO.transform.SetParent(parent, false);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = anchorMin;
        panelRect.anchorMax = anchorMax;
        panelRect.pivot = anchorMin; // pivot matches anchor for clean positioning
        panelRect.anchoredPosition = anchoredPos;
        panelRect.sizeDelta = new Vector2(220f, 145f);

        Image panelImage = panelGO.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.5f); // semi-transparent black

        // Text child
        GameObject textGO = new GameObject("ControlsText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(panelGO.transform, false);

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(8f, 4f);
        textRect.offsetMax = new Vector2(-8f, -4f);

        TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
        tmp.text = controlsText;
        tmp.fontSize = 12f;
        tmp.color = Color.white;
        tmp.richText = true;
        tmp.alignment = alignment == TextAnchor.UpperLeft
            ? TextAlignmentOptions.TopLeft
            : TextAlignmentOptions.TopRight;
        tmp.enableWordWrapping = false;

        return panelGO;
    }
}
