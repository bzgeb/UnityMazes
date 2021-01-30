using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static T Sample<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}