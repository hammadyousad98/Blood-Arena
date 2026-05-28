using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class FightingGameAnimatorBuilder
{
    [MenuItem("Tools/Create Fighting Game Animator Controller")]
    public static void CreateAnimatorController()
    {
        string controllerPath = "Assets/FightingGameController.controller";
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // 1. Add Parameters
        // Movement
        controller.AddParameter("MoveX", AnimatorControllerParameterType.Float);
        controller.AddParameter("MoveZ", AnimatorControllerParameterType.Float);
        controller.AddParameter("MoveMagnitude", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Crouch", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsJumping", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsOnGround", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsBlocking", AnimatorControllerParameterType.Bool);
        
        // Actions
        controller.AddParameter("LightAttack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("HeavyAttack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("SpecialAttack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Dodge", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Dead", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Parry", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Taunt", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Win", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Lose", AnimatorControllerParameterType.Trigger);

        // Directional / Enums
        controller.AddParameter("DodgeDirection", AnimatorControllerParameterType.Int); // 0:fwd, 1:back, 2:left, 3:right
        controller.AddParameter("HitDirection", AnimatorControllerParameterType.Int);   // 0-8
        controller.AddParameter("HitIntensity", AnimatorControllerParameterType.Int);   // 1:light, 2:heavy, 3:knockback

        // 2. Create Avatar Masks
        AvatarMask upperBodyMask = new AvatarMask();
        for (int i = 0; i < (int)AvatarMaskBodyPart.LastBodyPart; i++)
        {
            upperBodyMask.SetHumanoidBodyPartActive((AvatarMaskBodyPart)i, false);
        }
        upperBodyMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Body, true);
        upperBodyMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Head, true);
        upperBodyMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftArm, true);
        upperBodyMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightArm, true);
        AssetDatabase.CreateAsset(upperBodyMask, "Assets/UpperBodyMask.mask");

        AvatarMask faceMask = new AvatarMask();
        for (int i = 0; i < (int)AvatarMaskBodyPart.LastBodyPart; i++)
        {
            faceMask.SetHumanoidBodyPartActive((AvatarMaskBodyPart)i, false);
        }
        faceMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Head, true);
        AssetDatabase.CreateAsset(faceMask, "Assets/FaceMask.mask");

        // 3. Setup Layers
        AnimatorControllerLayer[] layers = controller.layers;
        layers[0].name = "Base Layer";
        
        controller.AddLayer("UpperBody");
        controller.AddLayer("Face");
        
        layers = controller.layers;
        layers[1].avatarMask = upperBodyMask;
        layers[1].blendingMode = AnimatorLayerBlendingMode.Override;
        layers[1].defaultWeight = 1f;

        layers[2].avatarMask = faceMask;
        layers[2].blendingMode = AnimatorLayerBlendingMode.Override;
        layers[2].defaultWeight = 1f;
        
        controller.layers = layers;

        // 4. Populate Layer 1: Base Layer
        AnimatorStateMachine rootSM = controller.layers[0].stateMachine;

        AnimatorState idleState = rootSM.AddState("Idle");
        AnimatorState locomotionState = rootSM.AddState("Locomotion");
        AnimatorState fightState = rootSM.AddState("Fight");
        AnimatorState jumpState = rootSM.AddState("Jump");
        AnimatorState crouchState = rootSM.AddState("Crouch");
        AnimatorState dodgeState = rootSM.AddState("Dodge");
        AnimatorState hitState = rootSM.AddState("Hit");
        AnimatorState deadState = rootSM.AddState("Dead");
        AnimatorState blockState = rootSM.AddState("Block");

        rootSM.defaultState = idleState;

        // Locomotion BlendTree
        BlendTree locomotionBlendTree = new BlendTree
        {
            name = "LocomotionBlendTree",
            blendType = BlendTreeType.SimpleDirectional2D,
            blendParameter = "MoveX",
            blendParameterY = "MoveZ"
        };
        AssetDatabase.AddObjectToAsset(locomotionBlendTree, controller);
        locomotionState.motion = locomotionBlendTree;
        
        // Add dummy motions to BT (to define threshold layout)
        locomotionBlendTree.AddChild(null, new Vector2(0, 0));   // Idle
        locomotionBlendTree.AddChild(null, new Vector2(0, 1));   // WalkForward
        locomotionBlendTree.AddChild(null, new Vector2(0, -1));  // WalkBack
        locomotionBlendTree.AddChild(null, new Vector2(-1, 0));  // WalkLeft
        locomotionBlendTree.AddChild(null, new Vector2(1, 0));   // WalkRight
        locomotionBlendTree.AddChild(null, new Vector2(0, 2));   // RunForward
        locomotionBlendTree.AddChild(null, new Vector2(0, -2));  // RunBack
        locomotionBlendTree.AddChild(null, new Vector2(-2, 0));  // RunLeft
        locomotionBlendTree.AddChild(null, new Vector2(2, 0));   // RunRight

        // Base Layer Transitions
        AnimatorStateTransition toLocomotion = rootSM.AddAnyStateTransition(locomotionState);
        toLocomotion.AddCondition(AnimatorConditionMode.Greater, 0.1f, "MoveMagnitude");
        toLocomotion.hasExitTime = false;
        toLocomotion.canTransitionToSelf = false;

        AnimatorStateTransition toIdle = locomotionState.AddTransition(idleState);
        toIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "MoveMagnitude");
        toIdle.hasExitTime = false;
        
        AnimatorStateTransition toHit = rootSM.AddAnyStateTransition(hitState);
        toHit.AddCondition(AnimatorConditionMode.Greater, 0, "HitIntensity");
        toHit.hasExitTime = false;
        toHit.interruptionSource = TransitionInterruptionSource.Destination;
        toHit.canTransitionToSelf = false;

        AnimatorStateTransition toDead = rootSM.AddAnyStateTransition(deadState);
        toDead.AddCondition(AnimatorConditionMode.If, 0, "Dead");
        toDead.hasExitTime = false;
        toDead.canTransitionToSelf = false; // Terminal state

        // 5. Populate Layer 2: UpperBody
        AnimatorStateMachine upperSM = controller.layers[1].stateMachine;
        AnimatorState upperEmpty = upperSM.AddState("Empty");
        upperSM.defaultState = upperEmpty;

        AnimatorState lightAttack = upperSM.AddState("LightAttack");
        AnimatorState heavyAttack = upperSM.AddState("HeavyAttack");
        AnimatorState throwGrab = upperSM.AddState("ThrowGrab");
        AnimatorState specialMove = upperSM.AddState("SpecialMove");
        AnimatorState parry = upperSM.AddState("Parry");
        AnimatorState taunt = upperSM.AddState("Taunt");

        // Attack Transitions (High responsiveness, can interrupt into next attack)
        AnimatorStateTransition toLight = upperSM.AddAnyStateTransition(lightAttack);
        toLight.AddCondition(AnimatorConditionMode.If, 0, "LightAttack");
        toLight.hasExitTime = false;
        toLight.duration = 0.05f;
        toLight.interruptionSource = TransitionInterruptionSource.Destination; // Combo support

        AnimatorStateTransition toHeavy = upperSM.AddAnyStateTransition(heavyAttack);
        toHeavy.AddCondition(AnimatorConditionMode.If, 0, "HeavyAttack");
        toHeavy.hasExitTime = false;
        toHeavy.duration = 0.1f;
        toHeavy.interruptionSource = TransitionInterruptionSource.Destination;

        AnimatorStateTransition toSpecial = upperSM.AddAnyStateTransition(specialMove);
        toSpecial.AddCondition(AnimatorConditionMode.If, 0, "SpecialAttack");
        toSpecial.hasExitTime = false;
        toSpecial.duration = 0.1f;

        // Return to Empty states
        lightAttack.AddTransition(upperEmpty).hasExitTime = true;
        heavyAttack.AddTransition(upperEmpty).hasExitTime = true;
        specialMove.AddTransition(upperEmpty).hasExitTime = true;

        // 6. Populate Layer 3: Face Layer
        AnimatorStateMachine faceSM = controller.layers[2].stateMachine;
        AnimatorState faceEmpty = faceSM.AddState("Empty");
        faceSM.defaultState = faceEmpty;
        
        AnimatorState faceWin = faceSM.AddState("WinExpression");
        AnimatorState faceLose = faceSM.AddState("LoseExpression");

        AssetDatabase.SaveAssets();
        Debug.Log("Fighting Game Animator Controller successfully created at: " + controllerPath);
    }
}
