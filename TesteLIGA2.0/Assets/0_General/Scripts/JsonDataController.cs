using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class JsonDataController<T>
{
    private string _fileName = "data.json";
    private string _filePath;
    private string _jsonStr;

    public JsonDataController()
    {
        _filePath = Application.persistentDataPath + "/" + _fileName + ".json";
        _jsonStr = "";

        CreateFile();
    }

    public JsonDataController(string fileName)
    {
        if (!String.IsNullOrEmpty(fileName))
            _fileName = fileName;

        _filePath = Application.persistentDataPath + "/" + _fileName + ".json";
        _jsonStr = "";

        CreateFile();
    }

    public JsonDataController(string fileName, string filePath)
    {
        if (!String.IsNullOrEmpty(fileName))
            _fileName = fileName;

        _filePath = filePath + fileName + ".json";
        _jsonStr = "";
    }

    public void SaveData(T[] data)
    {
        using (StreamWriter stream = new StreamWriter(_filePath))
        {
            _jsonStr = JsonHelper.ToJson(data, true);
            Debug.Log(_jsonStr);
            stream.Write(_jsonStr);
            Debug.Log("Saved");
        }
    }

    public void SaveData(List<T> data)
    {
        using (StreamWriter stream = new StreamWriter(_filePath))
        {
            _jsonStr = JsonHelper.ToJson(data, true);
            Debug.Log(_jsonStr);
            stream.Write(_jsonStr);
            Debug.Log("Saved");
        }
    }

    public T[] LoadDataArray()
    {
        using (StreamReader stream = new StreamReader(_filePath))
        {
            _jsonStr = stream.ReadToEnd();
            return JsonHelper.JsonToArray<T>(_jsonStr);
        }
    }

    public List<T> LoadDataList()
    {
        using (StreamReader stream = new StreamReader(_filePath))
        {
            _jsonStr = stream.ReadToEnd();
            return JsonHelper.JsonToList<T>(_jsonStr);
        }
    }


    private void CreateFile()
    {
        if (!File.Exists(_filePath))
            File.CreateText(_filePath).Dispose();
    }
}