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

public class RankingManager : MonoBehaviour
{
    private List<PlayerStat> _scoreBoard;
    private JsonDataController<PlayerStat> _jsonController;

    private void Awake()
    {
        _jsonController = new JsonDataController<PlayerStat>("score_board_data");
//        _scoreBoard = SortScoreBoard(_jsonController.LoadDataList(), SortingType.Descending);
    }

    public void AddScore(PlayerStat newStat)
    {
        _scoreBoard.Add(newStat);
        _jsonController.SaveData(_scoreBoard);
//        _scoreBoard = SortScoreBoard(_scoreBoard, SortingType.Descending);
//        if (_scoreBoard.Count >= 11)
//            _scoreBoard.RemoveAt(10);   //TODO: Maybe need to tweak this later
    }

    public void ResetScoreBoard()
    {
        _scoreBoard = new List<PlayerStat>();
        _jsonController.SaveData(_scoreBoard);
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