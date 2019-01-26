using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{
    public override void Update()
    {
        if (isEntityAwake)
        {
            ManageInput();
            ManageCollision();
            ManageDetection();
            ManageStatus();
            ManageNavigation();
            ManageAction(inputValues, collisionInfo, detectionInfo);
            ManageMovement(inputValues, collisionInfo);
        }      
    }

    public override void ManageNavigation()
    {
        
    }
}
