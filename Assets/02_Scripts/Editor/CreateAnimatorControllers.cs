using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public class CreateAnimatorControllers : EditorWindow
{
    [MenuItem("Tools/BloodArena/Create Animator Controllers")]
    public static void CreateControllers()
    {
        string dir = "Assets/03_Assets/Animators";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
            AssetDatabase.Refresh();
        }

        CreateController(dir + "/Ragnar_AnimatorController.controller");
        CreateController(dir + "/Darius_AnimatorController.controller");

        Debug.Log("Animator Controllers generated successfully for Ragnar and Darius!");
    }

    private static void CreateController(string path)
    {
        // Create the AnimatorController asset
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);

        // Parameters
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsBlocking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("LightAttack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("HeavyAttack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Dodge", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Dead", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        // States
        AnimatorState idleState = rootStateMachine.AddState("Idle");
        AnimatorState walkState = rootStateMachine.AddState("Walk");
        AnimatorState lightAttackState = rootStateMachine.AddState("LightAttack");
        AnimatorState heavyAttackState = rootStateMachine.AddState("HeavyAttack");
        AnimatorState blockState = rootStateMachine.AddState("Block");
        AnimatorState dodgeState = rootStateMachine.AddState("Dodge");
        AnimatorState hitState = rootStateMachine.AddState("Hit");
        AnimatorState deadState = rootStateMachine.AddState("Dead");

        // Set default state
        rootStateMachine.defaultState = idleState;

        // Transitions: Idle <-> Walk
        AnimatorStateTransition idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
        idleToWalk.hasExitTime = false;
        idleToWalk.duration = 0.1f;

        AnimatorStateTransition walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
        walkToIdle.hasExitTime = false;
        walkToIdle.duration = 0.1f;

        // Transitions: Any State -> Triggers
        AddAnyStateTriggerTransition(rootStateMachine, lightAttackState, "LightAttack");
        AddAnyStateTriggerTransition(rootStateMachine, heavyAttackState, "HeavyAttack");
        AddAnyStateTriggerTransition(rootStateMachine, dodgeState, "Dodge");
        AddAnyStateTriggerTransition(rootStateMachine, hitState, "Hit");
        AddAnyStateTriggerTransition(rootStateMachine, deadState, "Dead");

        // Transition: Any State -> Block
        AnimatorStateTransition anyToBlock = rootStateMachine.AddAnyStateTransition(blockState);
        anyToBlock.AddCondition(AnimatorConditionMode.If, 0, "IsBlocking");
        anyToBlock.hasExitTime = false;
        anyToBlock.duration = 0.05f;
        anyToBlock.canTransitionToSelf = false;

        // Transition: Block -> Idle
        AnimatorStateTransition blockToIdle = blockState.AddTransition(idleState);
        blockToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsBlocking");
        blockToIdle.hasExitTime = false;
        blockToIdle.duration = 0.1f;
    }

    private static void AddAnyStateTriggerTransition(AnimatorStateMachine rootStateMachine, AnimatorState state, string triggerName)
    {
        AnimatorStateTransition transition = rootStateMachine.AddAnyStateTransition(state);
        transition.AddCondition(AnimatorConditionMode.If, 0, triggerName);
        transition.hasExitTime = false;
        transition.duration = 0.05f;
        transition.canTransitionToSelf = false; // "All 'Any State' triggers: Can Transition To Self = false"
    }
}
