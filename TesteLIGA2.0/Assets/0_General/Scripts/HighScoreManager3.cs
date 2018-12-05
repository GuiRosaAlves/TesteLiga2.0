using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class HighScoreManager3 : MonoBehaviour
{
    private static int _scoreBoardSize = 10;
    private static List<PlayerStat> _scoreBoard = new List<PlayerStat>();
    public static List<PlayerStat> ScoreBoard { get { return _scoreBoard; } }
    private static PlayerStat _playerBuffer;
    public static PlayerStat PlayerBuffer { get { return _playerBuffer; } set { _playerBuffer = value; } }

    protected void Awake()
    {
        LoadScore();
        JsonUtility.ToJson(_scoreBoard);
    }

    public static void SaveScore()
    {
        BinaryFormatter bF = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/highscore.dat");

        _scoreBoard = _scoreBoard.OrderByDescending(sB => sB.Score).ToList();

        if (_scoreBoard.Count > _scoreBoardSize)
        {
            while (_scoreBoard.Count > _scoreBoardSize)
                _scoreBoard.RemoveAt(_scoreBoardSize + 1);
        }
        if (file != null)
        {
            bF.Serialize(file, _scoreBoard);
            file.Close();
        }
        else
        {
            Debug.Log("Error! Couldn't create a file");
        }
    }

    protected static void LoadScore()
    {
        if (File.Exists(Application.persistentDataPath + "/highscore.dat"))
        {
            BinaryFormatter bF = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/highscore.dat", FileMode.Open);
            _scoreBoard = (List<PlayerStat>) bF.Deserialize(file);
            file.Close();
        }
    }

    public static bool AddScore(PlayerStat player)
    {
        for (int i = 0; i < _scoreBoard.Count; i++)
        {
            if (player.Score > _scoreBoard[i].Score)
            {
                if (player.Name.ToLower() == _scoreBoard[i].Name.ToLower())
                {
                    _scoreBoard[i].Score = player.Score;
                    return true;
                }
            }
        }
        if (_scoreBoard.Count == _scoreBoardSize)
        {
            return false;
        }
        else
        {
            _scoreBoard.Add(player);
            return true;
        }
    }

    protected static void ResetScoreBoard()
    {
        _scoreBoard.Clear();
        SaveScore();
    }
}