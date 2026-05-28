using UnityEngine;

public class FeedbackCoordinator : MonoBehaviour
{
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private ScreenOverlay screenOverlay;
    [SerializeField] private SlowMotionController slowMo;

    private void OnEnable()
    {
        CombatEvents.OnHitLanded += HandleHitLanded;
        CombatEvents.OnBlock += HandleBlock;
        CombatEvents.OnParry += HandleParry;
        CombatEvents.OnFighterDead += HandleFighterDead;
    }

    private void OnDisable()
    {
        CombatEvents.OnHitLanded -= HandleHitLanded;
        CombatEvents.OnBlock -= HandleBlock;
        CombatEvents.OnParry -= HandleParry;
        CombatEvents.OnFighterDead -= HandleFighterDead;
    }

    private void HandleHitLanded(MoveData move, bool isHeavy)
    {
        if (isHeavy)
        {
            if (cameraShake != null) cameraShake.ShakeHeavyHit();
            if (screenOverlay != null) screenOverlay.HeavyHit();
        }
        else
        {
            if (cameraShake != null) cameraShake.ShakeNormalHit();
            if (screenOverlay != null) screenOverlay.NormalHit();
        }
    }

    private void HandleBlock(FighterController fighter)
    {
        if (screenOverlay != null) screenOverlay.Block();
    }

    private void HandleParry(FighterController fighter)
    {
        if (cameraShake != null) cameraShake.ShakeParry();
        if (screenOverlay != null) screenOverlay.Parry();
    }

    private void HandleFighterDead(FighterController fighter)
    {
        if (slowMo != null)
        {
            slowMo.StartCoroutine(slowMo.DoSlowMo());
        }
    }
}
