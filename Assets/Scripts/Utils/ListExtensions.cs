using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static T Draw<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            Debug.LogWarning("List is empty. Cannot draw an item.");
            return default;
        }


        int index = Random.Range(0, list.Count);
        T item = list[index];
        list.RemoveAt(index);
        return item;
    }

    public static T DrawFirst<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            Debug.LogWarning("List is empty. Cannot draw an item.");
            return default;
        }

        T item = list[0];
        list.RemoveAt(0);
        return item;
    }
}
