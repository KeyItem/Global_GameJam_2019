using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/State")]
public class EntityState : ScriptableObject
{
    public EntityAction[] actions;

    [Space(10)]
    public EntityTransition[] transitions;

    public void UpdateState(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        DoActions(controller, stateController, stateInfo);
        CheckTransitions(controller, stateController, stateInfo);
    }

    public void InitializeActions(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].InitializeAction(controller, stateController, stateInfo);
        }
    }

    private void DoActions(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller, stateController, stateInfo);
        }
    }

    private void CheckTransitions(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i].decision.Decide(controller, stateController, stateInfo))
            {
                if (transitions[i].trueState != null)
                {
                    stateController.TransitionToState(transitions[i].trueState, controller, stateController, stateInfo);
                }
            }
            else
            {
                if (transitions[i].falseState != null)
                {
                    stateController.TransitionToState(transitions[i].falseState, controller, stateController, stateInfo);
                }
            }
        }
    }
}
