using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("Score Attributes")] public List<ScoreInfo> highScores;

    public void CheckNewScore(float newScore)
    {
        float convertedScore = Mathf.Abs(newScore);
        
        if (CheckIfNewHighScore(convertedScore))
        {            
            AddNewHighScore(convertedScore);
        }
    }

    private void AddNewHighScore(float newScore)
    {
        highScores.Add(new ScoreInfo(newScore));
    }

    public float ReturnHighScore()
    {
        float highestScore = 0f;

        for (int i = 0; i < highScores.Count; i++)
        {
            if (highestScore < highScores[i].score)
            {
                highestScore = highScores[i].score;
            }
        }

        return highestScore;
    }

    private bool CheckIfNewHighScore(float newScore)
    {
        if (highScores.Count == 0)
        {
            return true;
        }
        
        for (int i = 0; i < highScores.Count; i++)
        {
            if (newScore > highScores[i].score)
            {
                return true;
            }
        }
        
        return false;
    }
}

[System.Serializable]
public struct ScoreInfo
{
    [Header("Score Info Attributes")] public float score;

    [Space(10)] public DateTime scoreTime;

    public ScoreInfo(float newScore)
    {
        this.score = newScore;
        this.scoreTime = DateTime.Now;
    }
}
