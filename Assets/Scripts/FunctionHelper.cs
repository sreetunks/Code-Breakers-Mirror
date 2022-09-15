using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionHelper : MonoBehaviour
{
    public static float PyThag(float a, float b)
    {
        return Mathf.Sqrt( a*a + b*b );
    }

    public static void PrintGridPosition(string var, int x, int z)
    {
        print(var + ": (" + x + "," + z + ")");
    }
}