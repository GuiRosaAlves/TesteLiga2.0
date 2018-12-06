using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MenuPanel
{
    public void PlayGame()
    {
        if (_App.SceneSwapManager)
            _App.SceneSwapManager.GoFoward();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}