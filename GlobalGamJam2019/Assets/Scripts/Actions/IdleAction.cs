using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/Idle")]
public class IdleAction : EntityAction
{
    public override void InitializeAction(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        controller.navigation.TogglePathfinding(false);
        controller.navigation.SetPathingType(PATHING_TYPE.NONE);
    }

    public override void Act(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        Idle(controller, stateController, stateInfo);
    }

    private void Idle (EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {

    }
}
