using UnityEngine;
using UnityEngine.UI;

public class GameOver : MenuPanel
{
    [SerializeField] private Text _scoreTxt;

    private void Awake()
    {
        if (_App.RankingManager)
            _scoreTxt.text = _App.RankingManager.StatBuffer.Score+"";
    }

    public void PlayAgain()
    {
        if (_App.SceneSwapManager)
            _App.SceneSwapManager.GoFoward();
    }
}