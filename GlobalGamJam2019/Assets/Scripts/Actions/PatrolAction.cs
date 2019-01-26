using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/PatrolAction")]
public class PatrolAction : EntityAction
{
    public override void InitializeAction(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        InitializePatrol(controller, stateController, stateInfo);
    }

    public override void Act(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        Patrol(controller, stateController, stateInfo);
    }

    private void InitializePatrol(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        if (controller.navigation.localPatrolPoints.Length > 0)
        {
            controller.navigation.SetPathingType(PATHING_TYPE.PATROL);
            controller.navigation.TogglePathfinding();
        }
    }

    private void Patrol(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {

    }
}
