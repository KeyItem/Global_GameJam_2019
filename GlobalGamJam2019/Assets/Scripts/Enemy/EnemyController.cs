using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    public override void Update()
    {
        if (isEntityAwake)
        {
            ManageStates();
            ManageInput();
            ManageCollision();
            ManageDetection();
            ManageStatus();
            ManageNavigation();
            ManageAction(inputValues, collisionInfo, detectionInfo);
            ManageMovement(inputValues, collisionInfo);
        }   
    }
}
