using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace BloodArena.Editor
{
    public class CreateAdvancedAnimatorController
    {
        [MenuItem("Tools/BloodArena/Create Advanced Animator Controller")]
        public static void CreateController()
        {
            string animatorsFolder = "Assets/03_Assets/Animators";
            
            // Ensure directories exist
            if (!AssetDatabase.IsValidFolder("Assets/03_Assets"))
            {
                AssetDatabase.CreateFolder("Assets", "03_Assets");
            }
            if (!AssetDatabase.IsValidFolder(animatorsFolder))
            {
                AssetDatabase.CreateFolder("Assets/03_Assets", "Animators");
            }

            // 1. Create Avatar Mask (Upper Body Only)
            string maskPath = animatorsFolder + "/UpperBodyMask.mask";
            if (AssetDatabase.LoadAssetAtPath<AvatarMask>(maskPath) != null)
            {
                AssetDatabase.DeleteAsset(maskPath);
            }

            AvatarMask mask = new AvatarMask();
            
            // Disable all body parts first
            for (AvatarMaskBodyPart i = AvatarMaskBodyPart.Root; i < AvatarMaskBodyPart.LastBodyPart; i++)
            {
                mask.SetHumanoidBodyPartActive(i, false);
            }

            // Enable specific upper body parts
            mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftArm, true);
            mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightArm, true);
            mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftFingers, true);
            mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightFingers, true);
            mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Body, true); // Spine/Chest

            AssetDatabase.CreateAsset(mask, maskPath);

            // 2. Create Animator Controller
            string controllerPath = animatorsFolder + "/Fighter_AnimatorController.controller";
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

            int paramCount = 0;
            int stateCount = 0;

            // --- Add Parameters ---
            controller.AddParameter("MoveX", AnimatorControllerParameterType.Float); paramCount++;
            controller.AddParameter("MoveZ", AnimatorControllerParameterType.Float); paramCount++;
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float); paramCount++;
            
            controller.AddParameter("IsBlocking", AnimatorControllerParameterType.Bool); paramCount++;
            controller.AddParameter("IsInCombatIdle", AnimatorControllerParameterType.Bool); paramCount++;
            controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool); paramCount++;

            string[] triggers = {
                "LightAttack", "HeavyAttack", "DodgeForward", "DodgeBackward",
                "DodgeLeft", "DodgeRight", "HitHigh", "HitLow", "KnockBack",
                "BlockImpact", "Parry", "Death", "ComboLight2", "ComboHeavy2"
            };

            foreach (string t in triggers)
            {
                controller.AddParameter(t, AnimatorControllerParameterType.Trigger);
                paramCount++;
            }

            // --- LAYER 0: BASE LAYER ---
            AnimatorStateMachine baseSM = controller.layers[0].stateMachine;

            // Locomotion Blend Tree
            BlendTree locomotionTree = new BlendTree();
            locomotionTree.name = "Locomotion";
            locomotionTree.blendType = BlendTreeType.FreeformDirectional2D;
            locomotionTree.blendParameter = "MoveX";
            locomotionTree.blendParameterY = "MoveZ";
            AssetDatabase.AddObjectToAsset(locomotionTree, controller);

            // Add Motions (Null clips as requested for developer manual assignment)
            locomotionTree.AddChild(null, new Vector2(0, 0));       // Idle
            locomotionTree.AddChild(null, new Vector2(0, 0));       // CombatIdle
            locomotionTree.AddChild(null, new Vector2(0, 1));       // WalkForward
            locomotionTree.AddChild(null, new Vector2(0, -1));      // WalkBackward
            locomotionTree.AddChild(null, new Vector2(-1, 0));      // StrafeLeft
            locomotionTree.AddChild(null, new Vector2(1, 0));       // StrafeRight
            locomotionTree.AddChild(null, new Vector2(-0.7f, 0.7f)); // WalkForwardLeft
            locomotionTree.AddChild(null, new Vector2(0.7f, 0.7f));  // WalkForwardRight
            locomotionTree.AddChild(null, new Vector2(-0.7f, -0.7f));// WalkBackLeft
            locomotionTree.AddChild(null, new Vector2(0.7f, -0.7f)); // WalkBackRight

            AnimatorState locomotionState = baseSM.AddState("Locomotion");
            locomotionState.motion = locomotionTree;
            locomotionState.writeDefaultValues = false;
            baseSM.defaultState = locomotionState;
            stateCount++;

            // Base Layer States
            AnimatorState blockIdleState = baseSM.AddState("Block Idle");
            blockIdleState.writeDefaultValues = false; stateCount++;

            AnimatorState blockImpactState = baseSM.AddState("Block Impact");
            blockImpactState.writeDefaultValues = false; stateCount++;

            AnimatorState parryState = baseSM.AddState("Parry");
            parryState.writeDefaultValues = false; stateCount++;

            AnimatorState dodgeFwdState = baseSM.AddState("DodgeForward");
            dodgeFwdState.writeDefaultValues = false; stateCount++;

            AnimatorState dodgeBwdState = baseSM.AddState("DodgeBackward");
            dodgeBwdState.writeDefaultValues = false; stateCount++;

            AnimatorState dodgeLState = baseSM.AddState("DodgeLeft");
            dodgeLState.writeDefaultValues = false; stateCount++;

            AnimatorState dodgeRState = baseSM.AddState("DodgeRight");
            dodgeRState.writeDefaultValues = false; stateCount++;

            AnimatorState hitHighState = baseSM.AddState("HitHigh");
            hitHighState.writeDefaultValues = false; stateCount++;

            AnimatorState hitLowState = baseSM.AddState("HitLow");
            hitLowState.writeDefaultValues = false; stateCount++;

            AnimatorState knockBackState = baseSM.AddState("KnockBack");
            knockBackState.writeDefaultValues = false; stateCount++;

            AnimatorState deadState = baseSM.AddState("Dead");
            deadState.writeDefaultValues = false; stateCount++;

            // --- Base Layer Transitions ---

            // Block Idle <-> Locomotion
            var trans = locomotionState.AddTransition(blockIdleState);
            trans.AddCondition(AnimatorConditionMode.If, 0, "IsBlocking");
            trans.hasExitTime = false;
            trans.duration = 0.1f;

            trans = blockIdleState.AddTransition(locomotionState);
            trans.AddCondition(AnimatorConditionMode.IfNot, 0, "IsBlocking");
            trans.hasExitTime = false;
            trans.duration = 0.15f;

            // Block Impact
            trans = baseSM.AddAnyStateTransition(blockImpactState);
            trans.AddCondition(AnimatorConditionMode.If, 0, "BlockImpact");
            trans.hasExitTime = false; trans.duration = 0.05f; trans.canTransitionToSelf = false;
            trans = blockImpactState.AddTransition(blockIdleState);
            trans.hasExitTime = true; trans.exitTime = 0.8f;

            // Parry
            trans = baseSM.AddAnyStateTransition(parryState);
            trans.AddCondition(AnimatorConditionMode.If, 0, "Parry");
            trans.hasExitTime = false; trans.duration = 0.05f; trans.canTransitionToSelf = false;
            trans = parryState.AddTransition(locomotionState);
            trans.hasExitTime = true; trans.exitTime = 0.9f;

            // Dodges & Hit Reactions (Using local helper method)
            void SetupReaction(AnimatorState state, string trigger, float exitT)
            {
                var t = baseSM.AddAnyStateTransition(state);
                t.AddCondition(AnimatorConditionMode.If, 0, trigger);
                t.hasExitTime = false; t.duration = 0.05f; t.canTransitionToSelf = false;

                var t2 = state.AddTransition(locomotionState);
                t2.hasExitTime = true; t2.exitTime = exitT;
            }

            SetupReaction(dodgeFwdState, "DodgeForward", 0.85f);
            SetupReaction(dodgeBwdState, "DodgeBackward", 0.85f);
            SetupReaction(dodgeLState, "DodgeLeft", 0.85f);
            SetupReaction(dodgeRState, "DodgeRight", 0.85f);
            
            SetupReaction(hitHighState, "HitHigh", 0.75f);
            SetupReaction(hitLowState, "HitLow", 0.75f);
            SetupReaction(knockBackState, "KnockBack", 0.9f);

            // Dead (No exit transition)
            trans = baseSM.AddAnyStateTransition(deadState);
            trans.AddCondition(AnimatorConditionMode.If, 0, "Death");
            trans.hasExitTime = false; trans.duration = 0.1f; trans.canTransitionToSelf = false;

            // --- LAYER 1: UPPER BODY ---
            controller.AddLayer("Upper Body");
            AnimatorControllerLayer[] layers = controller.layers;
            layers[1].defaultWeight = 1.0f;
            layers[1].blendingMode = AnimatorLayerBlendingMode.Additive;
            layers[1].avatarMask = mask;
            controller.layers = layers;

            AnimatorStateMachine upperSM = layers[1].stateMachine;

            AnimatorState upperEmpty = upperSM.AddState("UpperBody_Empty");
            upperEmpty.writeDefaultValues = false; stateCount++;
            upperSM.defaultState = upperEmpty;

            AnimatorState light1 = upperSM.AddState("LightAttack1");
            light1.writeDefaultValues = false; stateCount++;

            AnimatorState comboLight2 = upperSM.AddState("ComboLight2");
            comboLight2.writeDefaultValues = false; stateCount++;

            AnimatorState heavy1 = upperSM.AddState("HeavyAttack1");
            heavy1.writeDefaultValues = false; stateCount++;

            AnimatorState comboHeavy2 = upperSM.AddState("ComboHeavy2");
            comboHeavy2.writeDefaultValues = false; stateCount++;

            // --- Upper Body Transitions ---
            
            // Light 1
            trans = upperSM.AddAnyStateTransition(light1);
            trans.AddCondition(AnimatorConditionMode.If, 0, "LightAttack");
            trans.hasExitTime = false; trans.duration = 0.05f; trans.canTransitionToSelf = false;

            trans = light1.AddTransition(comboLight2);
            trans.AddCondition(AnimatorConditionMode.If, 0, "ComboLight2");
            trans.hasExitTime = false; trans.duration = 0.1f; trans.exitTime = 0.4f; // Activatable from 40%

            trans = light1.AddTransition(upperEmpty);
            trans.hasExitTime = true; trans.exitTime = 1.0f;

            // Combo Light 2
            trans = comboLight2.AddTransition(upperEmpty);
            trans.hasExitTime = true; trans.exitTime = 1.0f;

            // Heavy 1
            trans = upperSM.AddAnyStateTransition(heavy1);
            trans.AddCondition(AnimatorConditionMode.If, 0, "HeavyAttack");
            trans.hasExitTime = false; trans.duration = 0.05f; trans.canTransitionToSelf = false;

            trans = heavy1.AddTransition(comboHeavy2);
            trans.AddCondition(AnimatorConditionMode.If, 0, "ComboHeavy2");
            trans.hasExitTime = false; trans.duration = 0.1f; trans.exitTime = 0.5f; // Activatable from 50%

            trans = heavy1.AddTransition(upperEmpty);
            trans.hasExitTime = true; trans.exitTime = 1.0f;

            // Combo Heavy 2
            trans = comboHeavy2.AddTransition(upperEmpty);
            trans.hasExitTime = true; trans.exitTime = 1.0f;

            // Save and Refresh
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[Blood Arena] Advanced Animator Controller created successfully at: {controllerPath}\nCreated {stateCount} states and {paramCount} parameters.");
        }
    }
}
