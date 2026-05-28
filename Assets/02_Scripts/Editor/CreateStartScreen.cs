using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateStartScreen : EditorWindow
{
    [MenuItem("Tools/BloodArena/Build Start Screen")]
    public static void BuildStartScreen()
    {
        UnityEngine.SceneManagement.Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (currentScene.name != "StartScreen")
        {
            // Attempt to create or open StartScreen
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), "Assets/Scenes/StartScreen.unity");
        }

        // 1. Create Canvas
        GameObject canvasGO = new GameObject("StartScreen_Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Attach controller
        canvasGO.AddComponent<StartScreenController>();
        
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // --- BACKGROUND ---
        GameObject bgObj = CreateRect("Background", canvasGO.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        Image bgImg = bgObj.AddComponent<Image>();
        ColorUtility.TryParseHtmlString("#0D0D1A", out Color bgc);
        bgImg.color = bgc;

        // --- TITLE TEXT ---
        GameObject titleObj = CreateRect("TitleText", canvasGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 120), new Vector2(900, 100));
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "BLOOD ARENA";
        titleText.fontSize = 72;
        titleText.fontStyle = FontStyles.Bold;
        ColorUtility.TryParseHtmlString("#8B0000", out Color tc);
        titleText.color = tc;
        titleText.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;

        // --- SUBTITLE ---
        GameObject subObj = CreateRect("SubtitleText", canvasGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 30), new Vector2(600, 70));
        TextMeshProUGUI subText = subObj.AddComponent<TextMeshProUGUI>();
        subText.text = "Battle Combat";
        subText.fontSize = 48;
        subText.fontStyle = FontStyles.Bold;
        ColorUtility.TryParseHtmlString("#F39C12", out Color sc);
        subText.color = sc;
        subText.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;

        // --- TAGLINE ---
        GameObject tagObj = CreateRect("TaglineText", canvasGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -30), new Vector2(600, 44));
        TextMeshProUGUI tagText = tagObj.AddComponent<TextMeshProUGUI>();
        tagText.text = "Dual battle combat arena";
        tagText.fontSize = 28;
        tagText.color = Color.white;
        tagText.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;

        // --- FIGHT BUTTON ---
        GameObject fightObj = CreateRect("FightButton", canvasGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -120), new Vector2(200, 70));
        Image fBtnImg = fightObj.AddComponent<Image>();
        fBtnImg.color = tc; // #8B0000
        fightObj.AddComponent<Button>();

        GameObject fTextObj = CreateRect("Text", fightObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        TextMeshProUGUI fBtnText = fTextObj.AddComponent<TextMeshProUGUI>();
        fBtnText.text = "FIGHT";
        fBtnText.fontSize = 28;
        fBtnText.fontStyle = FontStyles.Bold;
        fBtnText.color = Color.white;
        fBtnText.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;

        // --- EXIT BUTTON ---
        GameObject exitObj = CreateRect("ExitButton", canvasGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -210), new Vector2(200, 70));
        Image eBtnImg = exitObj.AddComponent<Image>();
        eBtnImg.color = tc; // #8B0000
        exitObj.AddComponent<Button>();

        GameObject eTextObj = CreateRect("Text", exitObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        TextMeshProUGUI eBtnText = eTextObj.AddComponent<TextMeshProUGUI>();
        eBtnText.text = "EXIT";
        eBtnText.fontSize = 28;
        eBtnText.fontStyle = FontStyles.Bold;
        eBtnText.color = Color.white;
        eBtnText.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;

        Debug.Log("Start Screen UI Built Successfully.");
        Selection.activeGameObject = canvasGO;
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
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
