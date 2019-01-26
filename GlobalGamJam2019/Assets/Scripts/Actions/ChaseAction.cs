using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/ChaseAction")]
public class ChaseAction : EntityAction
{
    public override void InitializeAction(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        InitializeChase(controller, stateController, stateInfo);
    }

    public override void Act(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        Chase(controller, stateController, stateInfo);
    }

    private void InitializeChase(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        if (stateInfo.detectionInfo.detectedEntities.Count > 0)
        {
            controller.navigation.TogglePathfinding();
            controller.navigation.SetPathingType(PATHING_TYPE.CHASE);
        }
    }

    private void Chase (EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {

    }
}
