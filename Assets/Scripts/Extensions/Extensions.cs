using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using Microsoft.Xna.Framework;
public interface ICloneable<T>
{
    T Clone();
}

public static class Extensions
{
    public static T[] Clone<T>(this T[] array) where T : ICloneable<T>
    {
        var newArray = new T[array.Length];
        for (var i = 0; i < array.Length; i++)
            newArray[i] = array[i].Clone();
        return newArray;
    }
    public static IEnumerable<T> Clone<T>(this IEnumerable<T> items) where T : ICloneable<T>
    {
        foreach (var item in items)
            yield return item.Clone();
    }

    public static Vector3 Multiply(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
}