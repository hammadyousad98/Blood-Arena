#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateFeedbackSetup
{
    [MenuItem("Tools/BloodArena/Setup Feedback")]
    public static void SetupFeedback()
    {
        // 1. Add CameraShake to Main Camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("SetupFeedback: No Main Camera found in the scene.");
            return;
        }
        CameraShake cameraShake = mainCam.GetComponent<CameraShake>();
        if (cameraShake == null)
        {
            cameraShake = mainCam.gameObject.AddComponent<CameraShake>();
        }

        // 2. Create Overlay Canvas and Image
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("FeedbackCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<StandaloneInputModule>();
            }
        }

        Transform existingOverlay = canvas.transform.Find("ScreenOverlay");
        GameObject overlayObj;
        Image overlayImage;
        ScreenOverlay screenOverlayComponent;

        if (existingOverlay != null)
        {
            overlayObj = existingOverlay.gameObject;
            overlayImage = overlayObj.GetComponent<Image>();
            screenOverlayComponent = overlayObj.GetComponent<ScreenOverlay>();
            if (screenOverlayComponent == null) screenOverlayComponent = overlayObj.AddComponent<ScreenOverlay>();
        }
        else
        {
            overlayObj = new GameObject("ScreenOverlay");
            overlayObj.transform.SetParent(canvas.transform, false);
            
            overlayImage = overlayObj.AddComponent<Image>();
            overlayImage.color = new Color(0, 0, 0, 0);
            overlayImage.raycastTarget = false;
            
            RectTransform rect = overlayObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            screenOverlayComponent = overlayObj.AddComponent<ScreenOverlay>();
        }

        SerializedObject soScreenOverlay = new SerializedObject(screenOverlayComponent);
        soScreenOverlay.FindProperty("overlayImage").objectReferenceValue = overlayImage;
        soScreenOverlay.ApplyModifiedProperties();

        // 3. Create FeedbackManager GO
        GameObject feedbackManager = GameObject.Find("FeedbackManager");
        if (feedbackManager == null)
        {
            feedbackManager = new GameObject("FeedbackManager");
        }

        SlowMotionController slowMo = feedbackManager.GetComponent<SlowMotionController>();
        if (slowMo == null) slowMo = feedbackManager.AddComponent<SlowMotionController>();

        FeedbackCoordinator coordinator = feedbackManager.GetComponent<FeedbackCoordinator>();
        if (coordinator == null) coordinator = feedbackManager.AddComponent<FeedbackCoordinator>();

        // 4. Wire up references
        SerializedObject soCoordinator = new SerializedObject(coordinator);
        soCoordinator.FindProperty("cameraShake").objectReferenceValue = cameraShake;
        soCoordinator.FindProperty("screenOverlay").objectReferenceValue = screenOverlayComponent;
        soCoordinator.FindProperty("slowMo").objectReferenceValue = slowMo;
        soCoordinator.ApplyModifiedProperties();

        Debug.Log("Feedback setup completed successfully. All components wired!");
    }
}
#endif
