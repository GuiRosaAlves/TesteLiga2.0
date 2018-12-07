using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public struct RankingItem
{
    [FormerlySerializedAs("_rankNames")] public Text _name;
    [FormerlySerializedAs("_rankScores")] public Text _score;
}
public class Ranking : MenuPanel
{
    [FormerlySerializedAs("_rankNames")] [SerializeField] private RankingItem[] _rankItems;
    
    private void Start()
    {
        for (int i = 0; i < _rankItems.Length; i++)
        {
            _rankItems[i]._name.gameObject.SetActive(false);
            _rankItems[i]._score.gameObject.SetActive(false);
        }
        
        if (_App.RankingManager)
        {
            var scoreBoard = _App.RankingManager.ScoreBoard;
            
            for (int i = 0; i < _rankItems.Length; i++)
            {
                if (i < scoreBoard.Count)
                {
                    _rankItems[i]._name.gameObject.SetActive(true);
                    _rankItems[i]._name.text = scoreBoard[i].Name;
                    _rankItems[i]._score.gameObject.SetActive(true);
                    _rankItems[i]._score.text = scoreBoard[i].Score+"";
                }
            }
        }
    }
}