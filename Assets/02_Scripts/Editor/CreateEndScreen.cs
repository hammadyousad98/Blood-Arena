using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace BloodArena.Editor
{
    public class CreateEndScreen
    {
        [MenuItem("Tools/BloodArena/Build End Screen")]
        public static void BuildEndScreen()
        {
            // Create or Open scene
            string scenePath = "Assets/01_Scenes/EndScreen.unity";
            if (!System.IO.File.Exists(scenePath))
            {
                var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(newScene, scenePath);
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath);
            }

            // Remove existing Canvas if any
            var oldCanvas = GameObject.Find("EndScreen_Canvas");
            if (oldCanvas != null) GameObject.DestroyImmediate(oldCanvas);
            var oldEventSystem = GameObject.Find("EventSystem");
            if (oldEventSystem != null) GameObject.DestroyImmediate(oldEventSystem);

            // Create Canvas
            GameObject canvasObj = new GameObject("EndScreen_Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
            
            canvasObj.AddComponent<AudioSource>();

            // EndScreenController
            EndScreenController controller = canvasObj.AddComponent<EndScreenController>();

            ColorUtility.TryParseHtmlString("#F39C12", out Color gold);

            // Background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvasObj.transform, false);
            Image bgImg = bgObj.AddComponent<Image>();
            ColorUtility.TryParseHtmlString("#0D0D0D", out Color darkNeutral);
            bgImg.color = darkNeutral;
            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            controller.background = bgImg;

            // Top Section (Winner Banner)
            GameObject topPanelObj = new GameObject("TopPanel");
            topPanelObj.transform.SetParent(canvasObj.transform, false);
            Image topPanelImg = topPanelObj.AddComponent<Image>();
            ColorUtility.TryParseHtmlString("#1A1A1A", out Color darkAlpha);
            darkAlpha.a = 0.85f;
            topPanelImg.color = darkAlpha;
            RectTransform topRt = topPanelObj.GetComponent<RectTransform>();
            topRt.anchorMin = new Vector2(0.5f, 1f);
            topRt.anchorMax = new Vector2(0.5f, 1f);
            topRt.pivot = new Vector2(0.5f, 1f);
            topRt.anchoredPosition = new Vector2(0, -90);
            topRt.sizeDelta = new Vector2(900, 180);

            TMP_Text CreateText(string name, Transform parent, string text, int size, Color color, Vector2 pos, bool bold = false)
            {
                GameObject obj = new GameObject(name);
                obj.transform.SetParent(parent, false);
                TMP_Text t = obj.AddComponent<TextMeshProUGUI>();
                t.text = text;
                t.fontSize = size;
                t.color = color;
                t.alignment = TextAlignmentOptions.Center;
                if (bold) t.fontStyle = FontStyles.Bold;
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = pos;
                rt.sizeDelta = new Vector2(800, 100);
                return t;
            }

            controller.winnerLine1 = CreateText("WinnerLine1", topPanelObj.transform, "{WINNER} WINS THE DUEL!", 48, gold, new Vector2(0, 40), true);
            controller.winnerLine2 = CreateText("WinnerLine2", topPanelObj.transform, "VICTORY", 72, gold, new Vector2(0, -40), true);

            // Left Panel (P1)
            GameObject p1PanelObj = new GameObject("P1ResultPanel");
            p1PanelObj.transform.SetParent(canvasObj.transform, false);
            Image p1Img = p1PanelObj.AddComponent<Image>();
            RectTransform p1Rt = p1PanelObj.GetComponent<RectTransform>();
            p1Rt.anchorMin = new Vector2(0.5f, 0.5f);
            p1Rt.anchorMax = new Vector2(0.5f, 0.5f);
            p1Rt.anchoredPosition = new Vector2(-480, -80);
            p1Rt.sizeDelta = new Vector2(500, 300);
            controller.p1ResultPanel = p1Img;

            CreateText("P1Label", p1PanelObj.transform, "PLAYER 1", 24, Color.white, new Vector2(0, 120), true);
            controller.p1RoundWins = CreateText("P1RoundWins", p1PanelObj.transform, "0 ROUNDS WON", 36, Color.white, new Vector2(0, 0), true);
            controller.p1ResultTag = CreateText("P1ResultTag", p1PanelObj.transform, "WINNER", 28, Color.white, new Vector2(0, -60), true);

            GameObject p1Skulls = new GameObject("P1Skulls");
            p1Skulls.transform.SetParent(p1PanelObj.transform, false);
            RectTransform p1SkullsRt = p1Skulls.AddComponent<RectTransform>();
            p1SkullsRt.anchoredPosition = new Vector2(0, -110);
            
            Image CreateSkull(string name, Transform parent, Vector2 pos)
            {
                GameObject obj = new GameObject(name);
                obj.transform.SetParent(parent, false);
                Image img = obj.AddComponent<Image>();
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.anchoredPosition = pos;
                rt.sizeDelta = new Vector2(40, 40);
                return img;
            }
            controller.p1SkullIcon1 = CreateSkull("P1SkullIcon1", p1Skulls.transform, new Vector2(-25, 0));
            controller.p1SkullIcon2 = CreateSkull("P1SkullIcon2", p1Skulls.transform, new Vector2(25, 0));

            // Right Panel (P2)
            GameObject p2PanelObj = new GameObject("P2ResultPanel");
            p2PanelObj.transform.SetParent(canvasObj.transform, false);
            Image p2Img = p2PanelObj.AddComponent<Image>();
            RectTransform p2Rt = p2PanelObj.GetComponent<RectTransform>();
            p2Rt.anchorMin = new Vector2(0.5f, 0.5f);
            p2Rt.anchorMax = new Vector2(0.5f, 0.5f);
            p2Rt.anchoredPosition = new Vector2(480, -80);
            p2Rt.sizeDelta = new Vector2(500, 300);
            controller.p2ResultPanel = p2Img;

            CreateText("P2Label", p2PanelObj.transform, "PLAYER 2", 24, Color.white, new Vector2(0, 120), true);
            controller.p2RoundWins = CreateText("P2RoundWins", p2PanelObj.transform, "0 ROUNDS WON", 36, Color.white, new Vector2(0, 0), true);
            controller.p2ResultTag = CreateText("P2ResultTag", p2PanelObj.transform, "WINNER", 28, Color.white, new Vector2(0, -60), true);

            GameObject p2Skulls = new GameObject("P2Skulls");
            p2Skulls.transform.SetParent(p2PanelObj.transform, false);
            RectTransform p2SkullsRt = p2Skulls.AddComponent<RectTransform>();
            p2SkullsRt.anchoredPosition = new Vector2(0, -110);
            controller.p2SkullIcon1 = CreateSkull("P2SkullIcon1", p2Skulls.transform, new Vector2(-25, 0));
            controller.p2SkullIcon2 = CreateSkull("P2SkullIcon2", p2Skulls.transform, new Vector2(25, 0));

            // Bottom Section
            controller.roundsFought = CreateText("RoundsFought", canvasObj.transform, "ROUNDS FOUGHT: 2", 28, Color.white, new Vector2(0, -340));
            RectTransform rfRt = controller.roundsFought.GetComponent<RectTransform>();
            rfRt.anchorMin = new Vector2(0.5f, 0f);
            rfRt.anchorMax = new Vector2(0.5f, 0f);
            rfRt.anchoredPosition = new Vector2(0, 200);

            // Buttons
            Button CreateButton(string name, string text, Color color, Vector2 pos)
            {
                GameObject obj = new GameObject(name);
                obj.transform.SetParent(canvasObj.transform, false);
                Image img = obj.AddComponent<Image>();
                img.color = color;
                Button btn = obj.AddComponent<Button>();
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0f);
                rt.anchorMax = new Vector2(0.5f, 0f);
                rt.pivot = new Vector2(0.5f, 0f);
                rt.anchoredPosition = pos;
                rt.sizeDelta = new Vector2(200, 70);

                GameObject tObj = new GameObject("Text");
                tObj.transform.SetParent(obj.transform, false);
                TMP_Text t = tObj.AddComponent<TextMeshProUGUI>();
                t.text = text;
                t.fontSize = 28;
                t.color = Color.white;
                t.alignment = TextAlignmentOptions.Center;
                t.fontStyle = FontStyles.Bold;
                RectTransform tRt = tObj.GetComponent<RectTransform>();
                tRt.anchorMin = Vector2.zero;
                tRt.anchorMax = Vector2.one;
                tRt.sizeDelta = Vector2.zero;

                return btn;
            }

            ColorUtility.TryParseHtmlString("#8B0000", out Color darkRedBtn);
            controller.playAgainButton = CreateButton("PlayAgainButton", "PLAY AGAIN", darkRedBtn, new Vector2(-120, 100));

            ColorUtility.TryParseHtmlString("#444444", out Color greyBtn);
            controller.mainMenuButton = CreateButton("MainMenuButton", "MAIN MENU", greyBtn, new Vector2(120, 100));

            // Particles
            GameObject ptcObj = new GameObject("WinnerParticles");
            ptcObj.transform.SetParent(canvasObj.transform, false);
            ptcObj.transform.position = Vector3.zero;
            ParticleSystem ptc = ptcObj.AddComponent<ParticleSystem>();
            var main = ptc.main;
            main.playOnAwake = false;
            main.duration = 3f;
            main.loop = false;
            main.startColor = gold;

            var shape = ptc.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.rotation = new Vector3(-90, 0, 0); // Pointing up

            var emission = ptc.emission;
            emission.rateOverTime = 25;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0, 60) });

            controller.winnerParticles = ptc;

            // Event System
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

            UpdateBuildSettings();

            Debug.Log("[Blood Arena] EndScreen built successfully.");
        }

        private static void UpdateBuildSettings()
        {
            var originalScenes = EditorBuildSettings.scenes;
            var newScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();
            
            foreach (var s in originalScenes)
            {
                if (!s.path.Contains("VictoryScreen") && !s.path.Contains("DefeatScreen"))
                {
                    newScenes.Add(s);
                }
            }

            string endScreenPath = "Assets/01_Scenes/EndScreen.unity";
            bool hasEndScreen = false;
            foreach (var s in newScenes)
            {
                if (s.path == endScreenPath) hasEndScreen = true;
            }

            if (!hasEndScreen)
            {
                int insertIndex = Mathf.Min(5, newScenes.Count);
                newScenes.Insert(insertIndex, new EditorBuildSettingsScene(endScreenPath, true));
            }

            EditorBuildSettings.scenes = newScenes.ToArray();
        }
    }
}
