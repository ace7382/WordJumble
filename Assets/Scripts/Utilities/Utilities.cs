using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    private static System.Random random = new System.Random();

    public static void Shuffle<T>(T[] array)
    {
        for (int i = 0; i < array.Length - 1; ++i)
        {
            int r = random.Next(i, array.Length);
            (array[r], array[i]) = (array[i], array[r]);
        }
    }
}
