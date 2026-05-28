using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateDefeatScreen : EditorWindow
{
    [MenuItem("Tools/BloodArena/Build Defeat Screen")]
    public static void BuildDefeatScreen()
    {
        UnityEngine.SceneManagement.Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (currentScene.name != "DefeatScreen")
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), "Assets/Scenes/DefeatScreen.unity");
        }

        GameObject canvasGO = new GameObject("DefeatScreen_Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.AddComponent<EndScreenController>();

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        ColorUtility.TryParseHtmlString("#1A0A0A", out Color bgc);
        ColorUtility.TryParseHtmlString("#8B0000", out Color redc);
        ColorUtility.TryParseHtmlString("#333333", out Color greyc);

        GameObject bgObj = CreateRect("Background", canvasGO.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        bgObj.AddComponent<Image>().color = bgc;

        GameObject line1Obj = CreateRect("Line1Text", canvasGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 140), new Vector2(800, 60));
        TextMeshProUGUI line1Text = line1Obj.AddComponent<TextMeshProUGUI>();
        line1Text.text = "{Loser Name} has fallen!";
        line1Text.fontSize = 48;
        line1Text.fontStyle = FontStyles.Bold;
        line1Text.color = redc;
        line1Text.alignment = TextAlignmentOptions.Center;

        GameObject titleObj = CreateRect("DefeatTitle", canvasGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40), new Vector2(800, 100));
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "DEFEAT";
        titleText.fontSize = 72;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = redc;
        titleText.alignment = TextAlignmentOptions.Center;

        GameObject roundsObj = CreateRect("RoundsFought", canvasGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -50), new Vector2(800, 50));
        TextMeshProUGUI roundsText = roundsObj.AddComponent<TextMeshProUGUI>();
        roundsText.text = "Rounds Fought: {n}";
        roundsText.fontSize = 28;
        roundsText.color = Color.white;
        roundsText.alignment = TextAlignmentOptions.Center;

        CreateButton("PlayAgainButton", canvasGO.transform, new Vector2(0, -140), "PLAY AGAIN", redc);
        CreateButton("MainMenuButton", canvasGO.transform, new Vector2(0, -230), "MAIN MENU", greyc);

        GameObject psObj = new GameObject("EmberSystem");
        ParticleSystem ps = psObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = true;
        main.playOnAwake = true;
        ColorUtility.TryParseHtmlString("#FF4500", out Color emberColor);
        main.startColor = emberColor;
        var emission = ps.emission;
        emission.rateOverTime = 15;
        
        AudioSource audio = canvasGO.AddComponent<AudioSource>();
        audio.playOnAwake = true;

        Debug.Log("Defeat Screen Built.");
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

    private static void CreateButton(string name, Transform parent, Vector2 pos, string text, Color bgColor)
    {
        GameObject btnObj = CreateRect(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, new Vector2(200, 70));
        Image img = btnObj.AddComponent<Image>();
        img.color = bgColor;
        btnObj.AddComponent<Button>();

        GameObject txtObj = CreateRect("Text", btnObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.fontSize = 28;
        txt.color = Color.white;
        txt.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;
    }
}
