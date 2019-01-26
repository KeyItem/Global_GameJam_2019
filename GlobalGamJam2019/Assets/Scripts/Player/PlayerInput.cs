using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInput : EntityInput
{
    [Header("Player Input Attributes")]
    public InputAttributes playerInputAttributes;

    public override void InitializeInput()
    {
        input = ReInput.players.GetPlayer(0);

        base.InitializeInput();
    }

    public override void GetInput()
    {
        base.GetInput();

        GetAxisInputs();
        GetButtonInputs();
    }

    public override void GetAxisInputs()
    {
        for (int i = 0; i < axis.Length; i++)
        {
            inputValues.currentAxisInputs.axisInputs.Add(new AxisInput(axis[i].axisName, input.GetAxis(axis[i].axisName)));
        }
    }

    public override void GetButtonInputs()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (input.GetButton(buttons[i].buttonName))
            {
                inputValues.currentButtonInputs.buttonInputs.Add(new ButtonInput(buttons[i].buttonName, true));
            }
            else
            {
                inputValues.currentButtonInputs.buttonInputs.Add(new ButtonInput(buttons[i].buttonName, false));
            }

            if (input.GetButtonDown(buttons[i].buttonName))
            {
                inputValues.heldButtonInputs.buttonInputs.Add(new ButtonInput(buttons[i].buttonName, true));
            }
            else
            {
                inputValues.heldButtonInputs.buttonInputs.Add(new ButtonInput(buttons[i].buttonName, false));
            }

            if (input.GetButtonUp(buttons[i].buttonName))
            {
                inputValues.releasedButtonInputs.buttonInputs.Add(new ButtonInput(buttons[i].buttonName, true));
            }
            else
            {
                inputValues.releasedButtonInputs.buttonInputs.Add(new ButtonInput(buttons[i].buttonName, false));
            }
        }
    }
}

[System.Serializable]
public struct InputAttributes
{
    public float xInputSensitity;
    public float yInputSensitity;
}