using System.Collections;
using UnityEngine;

public static class Helper
{ 
    public static int RoundToNearestMultiple(int value, int multiple)
    {
        int modifier = value % multiple;

        float midpoint = multiple / 2f;

        if (modifier > midpoint)
        {
            return value + (multiple - modifier);
        }
        else
        {
            return value - modifier;
        }
    }

    public static float ReturnTimeRatio (float currentTime, float startTime, float maxTime, bool percentage = false)
    {
        float newRatio = (currentTime - startTime) / (maxTime - startTime);

        if (percentage)
        {
            newRatio *= 100;
        }

        return newRatio;
    }

    public static float ReturnEvalutationOfAnimationCurve(float currentRatio, float maxRatio, AnimationCurve targetCurve)
    {
        return targetCurve.Evaluate(currentRatio / maxRatio);
    }
}
