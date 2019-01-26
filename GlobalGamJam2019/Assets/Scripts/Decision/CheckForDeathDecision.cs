using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decision/CheckForDeathDecision")]
public class CheckForDeathDecision : EntityDecision
{
    public override bool Decide(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        return SeeIfEntityIsDead(controller, stateController, stateInfo);
    }

    private bool SeeIfEntityIsDead(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        return controller.status.IsEntityDead();
    }
}
