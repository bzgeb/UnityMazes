using System.Collections.Generic;
using UnityEngine;

public static class SampleUtil<T>
{
    public static T Sample(IReadOnlyList<T> collection)
    {
        return collection[Random.Range(0, collection.Count)];
    }
    
    public static T Sample(T[] collection)
    {
        return collection[Random.Range(0, collection.Length)];
    }
}