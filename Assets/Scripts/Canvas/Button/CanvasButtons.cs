using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasButtons : MonoBehaviour
{
    public void OnExit()
    {
        Application.Quit();
    }

    public void OnRetry()
    {
        GameController.GetGameController().ResetLevel();
    }
}
