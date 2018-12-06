using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerStat
{
    public string Name;
    public int Score;
}

public enum SortingType
{
    Ascending = 2,
    Descending = 0
}

public class _RankingManager : MonoBehaviour
{
    public List<PlayerStat> ScoreBoard { get; private set; }
    private JsonDataController<PlayerStat> _jsonController;
    public PlayerStat StatBuffer { get; set; } 

    private void Awake()
    {
        _jsonController = new JsonDataController<PlayerStat>("score_board_data");
        ScoreBoard = SortScoreBoard(_jsonController.LoadDataList(), SortingType.Descending);
        if (ScoreBoard == null)
            ScoreBoard = new List<PlayerStat>();
    }

    public bool CheckHighScore(PlayerStat stat)
    {
        if (ScoreBoard == null)
            return false;
        if (ScoreBoard.Count == 0)
        {
            ScoreBoard.Add(stat);
            return true;
        }

        for (int i = 0; i < 10; i++)
        {
            if (i < ScoreBoard.Count)
            {
                if (stat.Score > ScoreBoard[i].Score)
                {
                    ScoreBoard.Add(stat);
                    return true;
                }
            }
            else
            {
                ScoreBoard.Add(stat);
                return true;
            }
        }

        return false;
    }
    
    public void AddScore(PlayerStat newStat)
    {
        ScoreBoard.Add(newStat);
        _jsonController.SaveData(ScoreBoard);
//        _scoreBoard = SortScoreBoard(_scoreBoard, SortingType.Descending);
//        if (_scoreBoard.Count >= 11)
//            _scoreBoard.RemoveAt(10);   //TODO: Maybe need to tweak this later
    }

    public void ResetScoreBoard()
    {
        ScoreBoard = new List<PlayerStat>();
        _jsonController.SaveData(ScoreBoard);
    }

    public static List<PlayerStat> SortScoreBoard(List<PlayerStat> scoreBoard, SortingType filter)
    {
        if (scoreBoard == null)
            return null;

        var sortedBoard = scoreBoard.ToArray();
        Array.Sort(sortedBoard, (a, b) => (-1 + (int) filter) * (a.Score.CompareTo(b.Score)));
        return sortedBoard.ToList();
    }

    private void LogArray(List<PlayerStat> stats)
    {
        if (stats != null)
        {
            Debug.Log("--------------------------");
            for (int i = 0; i < stats.Count; i++)
            {
                Debug.Log("Name: " + stats[i].Name + " Score: " + stats[i].Score);
            }
            Debug.Log("--------------------------");
        }
    }
}