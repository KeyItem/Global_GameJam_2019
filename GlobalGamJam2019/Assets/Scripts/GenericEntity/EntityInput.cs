using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class EntityInput : MonoBehaviour
{
    [Header("Base Input Attributes")]
    public Player input;

    [Space(10)]
    public AxisInfo[] axis;
    public ButtonInfo[] buttons;

    [Space(10)]
    public InputInfo inputValues;

    private void Start()
    {
        InitializeInput();
    }

    public virtual void InitializeInput()
    {
        inputValues.InitializeInputQueues();
    }

    public virtual void GetInput()
    {
        inputValues.Reset();
    }

    public virtual void GetAxisInputs()
    {

    }

    public virtual void GetButtonInputs()
    {

    }

    public virtual InputInfo ReturnInput()
    {
        return inputValues;
    }
}

[System.Serializable]
public struct InputInfo
{
    [Header("Axis Movement")]
    public AxisInputs currentAxisInputs;

    public Queue<AxisInputs> cachedAxisInputs;

    [Space(10)]
    public int axisInputCacheCount;

    [Header("Button Inputs")]
    public ButtonInputs currentButtonInputs;
    public ButtonInputs releasedButtonInputs;
    public ButtonInputs heldButtonInputs;

    public Queue<ButtonInputs> cachedButtonInputs;

    [Space(10)]
    public int buttonInputCacheCount;

    public void InitializeInputQueues()
    {
        cachedAxisInputs = new Queue<AxisInputs>();
        cachedButtonInputs = new Queue<ButtonInputs>();
    }

    public AxisInput ReturnCurrentAxis(string targetAxisName)
    {
        for (int i = 0; i < currentAxisInputs.axisInputs.Count; i++)
        {
            if (currentAxisInputs.axisInputs[i].axisName == targetAxisName)
            {
                return currentAxisInputs.axisInputs[i];
            }
        }

        return new AxisInput();
    }

    public bool ReturnCurrentButtonState(string targetButtonName)
    {
        for (int i = 0; i < currentButtonInputs.buttonInputs.Count; i++)
        {
            if (currentButtonInputs.buttonInputs[i].buttonName == targetButtonName)
            {
                return currentButtonInputs.buttonInputs[i].buttonState;
            }
        }

        return false;
    }

    public bool ReturnButtonHeldState(string targetButtonName)
    {
        for (int i = 0; i < heldButtonInputs.buttonInputs.Count; i++)
        {
            if (heldButtonInputs.buttonInputs[i].buttonName == targetButtonName)
            {
                return heldButtonInputs.buttonInputs[i].buttonState;
            }
        }

        return false;
    }

    public bool ReturnButtonReleaseState(string targetButtonName)
    {
        for (int i = 0; i < currentButtonInputs.buttonInputs.Count; i++)
        {
            if (releasedButtonInputs.buttonInputs[i].buttonName == targetButtonName)
            {
                return heldButtonInputs.buttonInputs[i].buttonState;
            }
        }

        return false;
    }

    public void Reset()
    {
        if (cachedAxisInputs.Count > axisInputCacheCount)
        {
            cachedAxisInputs.Dequeue();
        }

        if (cachedButtonInputs.Count > buttonInputCacheCount)
        {
            cachedAxisInputs.Dequeue();
        }

        cachedAxisInputs.Enqueue(currentAxisInputs);
        cachedButtonInputs.Enqueue(currentButtonInputs);

        currentAxisInputs.axisInputs.Clear();
        currentButtonInputs.buttonInputs.Clear();
        heldButtonInputs.buttonInputs.Clear();
        releasedButtonInputs.buttonInputs.Clear();
    }
}

[System.Serializable]
public struct AxisInputs
{
    public List<AxisInput> axisInputs;
}

[System.Serializable]
public struct AxisInput
{
    public string axisName;
    public float axisValue;

    public AxisInput(string newAxisName, float newAxisValue)
    {
        this.axisName = newAxisName;
        this.axisValue = newAxisValue;
    }
}

[System.Serializable]
public struct AxisInfo
{
    public string axisName;

    public AxisInfo(string newAxisName)
    {
        this.axisName = newAxisName;
    }
}

[System.Serializable]
public struct ButtonInputs
{
    public List<ButtonInput> buttonInputs;
}

[System.Serializable]
public struct ButtonInput
{
    public string buttonName;
    public bool buttonState;

    public ButtonInput(string newButtonName, bool newButtonState)
    {
        buttonName = newButtonName;
        buttonState = newButtonState;
    }
}

[System.Serializable]
public struct ButtonInfo
{
    public string buttonName;

    public ButtonInfo(string newButtonName)
    {
        this.buttonName = newButtonName;
    }
}

