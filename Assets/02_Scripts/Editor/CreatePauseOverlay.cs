using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatePauseOverlay : EditorWindow
{
    [MenuItem("Tools/BloodArena/Build Pause Overlay")]
    public static void BuildPauseOverlay()
    {
        GameObject canvasGO = new GameObject("PauseOverlay_Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.AddComponent<PauseMenuController>();

        GameObject panelObj = CreateRect("PausePanel", canvasGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600, 400));
        Image panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.8f);

        GameObject titleObj = CreateRect("TitleText", panelObj.transform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -60), new Vector2(400, 60));
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "PAUSED";
        titleText.fontSize = 48;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;

        ColorUtility.TryParseHtmlString("#8B0000", out Color redc);

        CreateButton("ResumeButton", panelObj.transform, new Vector2(0, 40), "RESUME", redc);
        CreateButton("RestartButton", panelObj.transform, new Vector2(0, -40), "RESTART", redc);
        CreateButton("MainMenuButton", panelObj.transform, new Vector2(0, -120), "MAIN MENU", redc);

        panelObj.SetActive(false);

        Debug.Log("Pause Overlay Built in active scene.");
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

    private static void CreateButton(string name, Transform parent, Vector2 pos, string text, Color bgColor)
    {
        GameObject btnObj = CreateRect(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, new Vector2(240, 60));
        Image img = btnObj.AddComponent<Image>();
        img.color = bgColor;
        btnObj.AddComponent<Button>();

        GameObject txtObj = CreateRect("Text", btnObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.fontSize = 24;
        txt.fontStyle = FontStyles.Bold;
        txt.color = Color.white;
        txt.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;
    }
}
