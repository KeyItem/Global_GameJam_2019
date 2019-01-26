using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/ReturnHome")]
public class ReturnHomeAction : EntityAction
{
    public override void InitializeAction(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        InitializeReturnHome(controller, stateController, stateInfo);
    }

    public override void Act(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        ReturnHome(controller, stateController, stateInfo);
    }

    private void InitializeReturnHome(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        controller.navigation.TogglePathfinding();
        controller.navigation.SetPathingType(PATHING_TYPE.RETURN);
    }

    private void ReturnHome(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {

    }
}
