using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decision/CheckForHomePointDecision")]
public class CheckForHomePointDecision : EntityDecision
{
    [Header("Minimum Distance Attributes")]
    public float minDistanceToTarget = 0.5f;

    public override bool Decide(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        return CheckIfArrivedAtHomepoint(controller, stateController, stateInfo);
    }

    private bool CheckIfArrivedAtHomepoint(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        return controller.navigation.CheckIfReachedTargetPoint(controller.navigation.homePoint, minDistanceToTarget);
    }
}
