using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Rotates the main camera to face a specific fighter when a key is held.
/// Attach to the same GameObject as the main Camera.
/// V = face Player 1, L = face Player 2.
/// </summary>
public class FaceCameraController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign the FighterSpawner to access P1/P2 transforms.")]
    [SerializeField] private FighterSpawner fighterSpawner;

    [Header("Camera Settings")]
    [Tooltip("How fast the camera rotates to face the target.")]
    [SerializeField] private float rotationSpeed = 5f;
    [Tooltip("Height offset above the fighter's pivot for the look-at target.")]
    [SerializeField] private float lookAtHeightOffset = 1.5f;

    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private bool hasStoredDefaults = false;

    private Transform currentTarget = null;
    private bool isLookingAtFace = false;

    private void LateUpdate()
    {
        // Store default camera pose on first frame
        if (!hasStoredDefaults)
        {
            defaultPosition = transform.position;
            defaultRotation = transform.rotation;
            hasStoredDefaults = true;
        }

        // Check for face-cam key input (new Input System)
        Keyboard kb = Keyboard.current;
        bool p1FacePressed = kb != null && kb.vKey.isPressed;
        bool p2FacePressed = kb != null && kb.lKey.isPressed;

        if (p1FacePressed && fighterSpawner != null && fighterSpawner.P1 != null)
        {
            currentTarget = fighterSpawner.P1.transform;
            isLookingAtFace = true;
        }
        else if (p2FacePressed && fighterSpawner != null && fighterSpawner.P2 != null)
        {
            currentTarget = fighterSpawner.P2.transform;
            isLookingAtFace = true;
        }
        else
        {
            isLookingAtFace = false;
            currentTarget = null;
        }

        if (isLookingAtFace && currentTarget != null)
        {
            // Calculate a position in front of the fighter's face
            Vector3 fighterPos = currentTarget.position + Vector3.up * lookAtHeightOffset;
            Vector3 faceDirection = currentTarget.forward;

            // Position the camera in front of the fighter, looking back at them
            Vector3 targetCamPos = fighterPos + faceDirection * 2f + Vector3.up * 0.3f;
            Quaternion targetCamRot = Quaternion.LookRotation(fighterPos - targetCamPos);

            transform.position = Vector3.Lerp(transform.position, targetCamPos, Time.unscaledDeltaTime * rotationSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetCamRot, Time.unscaledDeltaTime * rotationSpeed);
        }
        else
        {
            // Smoothly return to default
            transform.position = Vector3.Lerp(transform.position, defaultPosition, Time.unscaledDeltaTime * rotationSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, Time.unscaledDeltaTime * rotationSpeed);
        }
    }

    /// <summary>
    /// Call this if the default camera pose changes (e.g. after scene reload).
    /// </summary>
    public void ResetDefaults()
    {
        defaultPosition = transform.position;
        defaultRotation = transform.rotation;
        hasStoredDefaults = true;
    }
}
