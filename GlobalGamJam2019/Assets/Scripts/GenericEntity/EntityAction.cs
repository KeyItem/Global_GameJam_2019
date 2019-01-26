using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityAction : ScriptableObject
{
    public abstract void InitializeAction(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo);

    public abstract void Act(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo);
}
