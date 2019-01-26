using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/Death")]
public class DeathAction : EntityAction
{
    public override void InitializeAction(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        
    }

    public override void Act(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        Die(controller, stateController, stateInfo);
    }

    private void Die (EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {

    }
}
