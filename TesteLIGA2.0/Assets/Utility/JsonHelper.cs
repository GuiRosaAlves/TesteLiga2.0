using UnityEngine;
using System.Collections.Generic;

public static class JsonHelper
{
	#region Json -> Array Methods
	
	public static T[] JsonToArray<T>(string json)
	{
		ArrayWrapper<T> arrayWrapper = JsonUtility.FromJson<ArrayWrapper<T>>(json);
		return (arrayWrapper == null) ? null : arrayWrapper.Objects;
	}
	public static string ToJson<T>(T[] array)
	{
		ArrayWrapper<T> arrayWrapper = new ArrayWrapper<T>();
		arrayWrapper.Objects = array;
		return JsonUtility.ToJson(arrayWrapper);
	}
	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		ArrayWrapper<T> arrayWrapper = new ArrayWrapper<T>();
		arrayWrapper.Objects = array;
		return JsonUtility.ToJson(arrayWrapper, prettyPrint);
	}
	[System.Serializable]
	private class ArrayWrapper<T>
	{
		public T[] Objects;
	}
	
	#endregion
	
	#region Json -> List Methods
	
	public static List<T> JsonToList<T>(string json)
	{
		ListWrapper<T> listWrapper = JsonUtility.FromJson<ListWrapper<T>>(json);

		return (listWrapper == null) ? null : listWrapper.Objects;
	}
	public static string ToJson<T>(List<T> list)
	{
		ListWrapper<T> listWrapper = new ListWrapper<T>();
		listWrapper.Objects = list;
		return JsonUtility.ToJson(listWrapper);
	}
	public static string ToJson<T>(List<T> list, bool prettyPrint)
	{
		ListWrapper<T> listWrapper = new ListWrapper<T>();
		listWrapper.Objects = list;
		return JsonUtility.ToJson(listWrapper, prettyPrint);
	}
	[System.Serializable]
	private class ListWrapper<T>
	{
		public List<T> Objects;
	}
	
	#endregion
}