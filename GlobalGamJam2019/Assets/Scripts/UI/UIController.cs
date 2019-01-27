using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Depth Text Attributes")] public string baseDepthString = "Depth : ";
    public string baseDepthUnits = "m";

    [Space(10)] public TextMeshProUGUI depthText;

    private StringBuilder depthSb = new StringBuilder();

    [Header("Score Text Attributes")] public string baseScoreString = "Score : ";

    [Space(10)] public TextMeshProUGUI scoreText;

    private StringBuilder scoreSb = new StringBuilder();
    
    public void UpdateDepthText(float newDepthValue)
    {
        float crushedDepthValue = Mathf.Abs(newDepthValue);
        crushedDepthValue = Mathf.Floor(crushedDepthValue);
        
        depthSb.Clear();
        depthSb.Append(baseDepthString + crushedDepthValue + baseDepthUnits);

        depthText.text = depthSb.ToString();
    }

    public void UpdateScoreText(float newScore)
    {
        float crushedScore = Mathf.Abs(newScore);
        crushedScore = Mathf.Floor(crushedScore);

        scoreSb.Clear();
        scoreSb.Append(baseScoreString + newScore);

        scoreText.text = scoreSb.ToString();
    }
}
