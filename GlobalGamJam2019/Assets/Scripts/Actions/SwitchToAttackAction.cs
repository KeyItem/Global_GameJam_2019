using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/AttackAction")]
public class SwitchToAttackAction : EntityAction
{
    public override void InitializeAction(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        InitializeAttackAction(controller, stateController, stateInfo);
    }

    public override void Act(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        AttackAction(controller, stateController, stateInfo);
    }

    private void InitializeAttackAction(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {

    }

    private void AttackAction(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {

    }
}
