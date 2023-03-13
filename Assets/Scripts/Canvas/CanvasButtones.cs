using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CanvasButtones : MonoBehaviour
{
    public void Restart()
    {
        GameController.GetGameController().ResetLevel();
    }
    public void LoadFirsLevel()
    {
        AsyncOperation  LoadLevel = SceneManager.LoadSceneAsync(0);
        LoadLevel.completed += (asyncOperation) =>
        {
            GameController.GetGameController().SetGameStates(GameStates.PLAY);
        };
        
    }
    public void OnExit()
    {
        Application.Quit();
    }
}
