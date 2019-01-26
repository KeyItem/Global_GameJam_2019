using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI Elements")] public UIElement healthUIElement;

    [Space(10)] public UIElement cardUIElement;

    private void Start()
    {
        UIControllerInitialize();
    }

    public virtual void UIControllerInitialize()
    {
        
    }
    
    public virtual void FetchPlayerStatus()
    {
        
    }
}
