using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CreateHUDLayout : EditorWindow
{
    [MenuItem("Tools/BloodArena/Build HUD")]
    public static void BuildHUD()
    {
        // 1. Create Canvas
        GameObject canvasGO = new GameObject("HUD_Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Ensure EventSystem exists
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // --- P1 HP BAR ---
        GameObject p1HPObj = CreateRect("HPBar_P1", canvasGO.transform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20), new Vector2(400, 30));
        Image p1Bg = p1HPObj.AddComponent<Image>();
        ColorUtility.TryParseHtmlString("#333333", out Color p1bgc);
        p1Bg.color = p1bgc;

        GameObject p1FillObj = CreateRect("Fill", p1HPObj.transform, Vector2.zero, Vector2.one, new Vector2(0, 0.5f), Vector2.zero, Vector2.zero);
        Image p1Fill = p1FillObj.AddComponent<Image>();
        ColorUtility.TryParseHtmlString("#C0392B", out Color p1fc);
        p1Fill.color = p1fc;
        
        GameObject p1TextObj = CreateRect("NameText", p1HPObj.transform, Vector2.zero, Vector2.one, new Vector2(0, 0.5f), new Vector2(10, 0), Vector2.zero);
        TextMeshProUGUI p1Text = p1TextObj.AddComponent<TextMeshProUGUI>();
        p1Text.text = "RAGNAR";
        p1Text.fontSize = 18;
        p1Text.color = Color.white;
        p1Text.alignment = TextAlignmentOptions.Left | TextAlignmentOptions.Midline;

        // --- P2 HP BAR ---
        GameObject p2HPObj = CreateRect("HPBar_P2", canvasGO.transform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -20), new Vector2(400, 30));
        Image p2Bg = p2HPObj.AddComponent<Image>();
        ColorUtility.TryParseHtmlString("#333333", out Color p2bgc);
        p2Bg.color = p2bgc;

        GameObject p2FillObj = CreateRect("Fill", p2HPObj.transform, Vector2.zero, Vector2.one, new Vector2(1, 0.5f), Vector2.zero, Vector2.zero);
        Image p2Fill = p2FillObj.AddComponent<Image>();
        ColorUtility.TryParseHtmlString("#8E44AD", out Color p2fc);
        p2Fill.color = p2fc;

        GameObject p2TextObj = CreateRect("NameText", p2HPObj.transform, Vector2.zero, Vector2.one, new Vector2(1, 0.5f), new Vector2(-10, 0), Vector2.zero);
        TextMeshProUGUI p2Text = p2TextObj.AddComponent<TextMeshProUGUI>();
        p2Text.text = "DARIUS";
        p2Text.fontSize = 18;
        p2Text.color = Color.white;
        p2Text.alignment = TextAlignmentOptions.Right | TextAlignmentOptions.Midline;

        // --- P1 WIN ICONS ---
        GameObject p1SkullsObj = CreateRect("RoundWins_P1", canvasGO.transform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -60), new Vector2(220, 40));
        HorizontalLayoutGroup p1Layout = p1SkullsObj.AddComponent<HorizontalLayoutGroup>();
        p1Layout.childAlignment = TextAnchor.MiddleLeft;
        p1Layout.spacing = 10;
        p1Layout.childControlWidth = false; p1Layout.childControlHeight = false;

        GameObject s1p1 = CreateRect("Skull1_P1", p1SkullsObj.transform, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(30, 30));
        s1p1.AddComponent<Image>().color = Color.gray;
        GameObject s2p1 = CreateRect("Skull2_P1", p1SkullsObj.transform, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(30, 30));
        s2p1.AddComponent<Image>().color = Color.gray;

        // --- P2 WIN ICONS ---
        GameObject p2SkullsObj = CreateRect("RoundWins_P2", canvasGO.transform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -60), new Vector2(220, 40));
        HorizontalLayoutGroup p2Layout = p2SkullsObj.AddComponent<HorizontalLayoutGroup>();
        p2Layout.childAlignment = TextAnchor.MiddleRight;
        p2Layout.spacing = 10;
        p2Layout.childControlWidth = false; p2Layout.childControlHeight = false;

        GameObject s1p2 = CreateRect("Skull1_P2", p2SkullsObj.transform, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(30, 30));
        s1p2.AddComponent<Image>().color = Color.gray;
        GameObject s2p2 = CreateRect("Skull2_P2", p2SkullsObj.transform, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(30, 30));
        s2p2.AddComponent<Image>().color = Color.gray;

        // --- TIMER ---
        GameObject timerObj = CreateRect("TimerText", canvasGO.transform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -20), new Vector2(200, 60));
        TextMeshProUGUI timerText = timerObj.AddComponent<TextMeshProUGUI>();
        timerText.text = "2:00";
        timerText.fontSize = 28;
        timerText.fontStyle = FontStyles.Bold;
        timerText.color = Color.white;
        timerText.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;

        // --- ROUND COUNTER ---
        GameObject roundObj = CreateRect("RoundCounterText", canvasGO.transform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -90), new Vector2(220, 44));
        TextMeshProUGUI roundText = roundObj.AddComponent<TextMeshProUGUI>();
        roundText.text = "ROUND 1";
        roundText.fontSize = 24;
        roundText.fontStyle = FontStyles.Bold;
        ColorUtility.TryParseHtmlString("#F39C12", out Color roundColor);
        roundText.color = roundColor;
        roundText.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;

        // --- FEEDBACK POPUP ---
        GameObject feedbackObj = CreateRect("FeedbackPopup", canvasGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(500, 80));
        TextMeshProUGUI fbText = feedbackObj.AddComponent<TextMeshProUGUI>();
        fbText.text = "";
        fbText.fontSize = 36;
        fbText.fontStyle = FontStyles.Bold;
        fbText.color = new Color(1, 1, 1, 0); // Alpha 0
        fbText.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;

        // --- PAUSE BUTTON ---
        GameObject pauseObj = CreateRect("PauseButton", canvasGO.transform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -140), new Vector2(120, 44));
        Image pBtnImg = pauseObj.AddComponent<Image>();
        ColorUtility.TryParseHtmlString("#8B0000", out Color pauseColor);
        pBtnImg.color = pauseColor;
        pauseObj.AddComponent<Button>();

        GameObject pauseTextObj = CreateRect("Text", pauseObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        TextMeshProUGUI pBtnText = pauseTextObj.AddComponent<TextMeshProUGUI>();
        pBtnText.text = "PAUSE";
        pBtnText.fontSize = 16;
        pBtnText.color = Color.white;
        pBtnText.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;

        Debug.Log("HUD Created successfully.");
        Selection.activeGameObject = canvasGO;
    }

    private static GameObject CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
        return go;
    }
}
