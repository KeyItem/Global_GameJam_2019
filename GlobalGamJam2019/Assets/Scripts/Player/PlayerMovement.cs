using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : EntityMovement
{
    public override void MoveController(Vector3 moveDirection)
    {
        Debugger.DrawCustomDebugRay(transform.position, moveDirection, Color.yellow);

        base.MoveController(moveDirection);
    }

    public override Vector3 ReturnRawInputMovementVector(InputInfo inputValues)
    {
        float xAxis = inputValues.ReturnCurrentAxis("XAxis").axisValue;
        float yAxis = inputValues.ReturnCurrentAxis("YAxis").axisValue;
        float zAxis = inputValues.ReturnCurrentAxis("ZAxis").axisValue;

        return new Vector3(xAxis, zAxis, yAxis);
    }

    public override Vector3 ReturnRawInputRotationalVector(InputInfo inputValues)
    {
        float xAxis = inputValues.ReturnCurrentAxis("XAxis").axisValue;
        float yAxis = inputValues.ReturnCurrentAxis("YAxis").axisValue;
        float zAxis = inputValues.ReturnCurrentAxis("ZAxis").axisValue;

        return new Vector3(xAxis, zAxis, yAxis);
    }
}
