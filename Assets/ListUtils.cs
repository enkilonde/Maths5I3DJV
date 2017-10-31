using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListUtils
{



    public static void bubbleSort(this List<float> list)
    {
        int n = list.Count;
        do
        {
            int newn = 0;

            for (int i = 1; i <= n-1; i++)
            {
                if (list[i-1] > list[i])
                {
                    float temp = list[i];
                    list[i] = list[i - 1];
                    list[i - 1] = temp;
                    newn = i;
                }
            }
            n = newn;
        } while (n != 0);
    }


    public static void sortX(this List<Vector2> list)
    {
        list.Sort((a, b) => a.x.CompareTo(b.x));
    }

    public static void sortY(this List<Vector2> list)
    {
        list.Sort((a, b) => a.y.CompareTo(b.y));
    }

}
