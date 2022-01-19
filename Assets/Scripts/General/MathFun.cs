using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFun
{
    public static int Floor(float a)
    {
        int val = (int)a;
        if (a < 0)
            val--;
        return val;
    }

    public static bool Between(float a, float b, float val, bool inclusive = true)
    {
        if (inclusive)
            return a <= val && b >= val;
        else
            return a < val && b > val;
    }

    public static float Lerp(float min, float max, float percent)
    {
        return min + percent * (max - min);
    }

    public static int SnapLerp(int min, int max, float percent)
    {
        return Round(min + percent * (max - min));
    }

    public static int Closest(float[] numbers, float val)
    {
        int index = 0;

        for(int i = 1; i < numbers.Length; i++)
        {
            if (Abs(numbers[i] - val) < Abs(numbers[index] - val)) index = i;
        }

        return index;
    }

    public static float Clamp(float a, float b, float value)
    {
        if (value < a) return a;
        if (value > b) return b;
        return value;
    }

    public static int Clamp(int a, int b, int value)
    {
        if (value < a) return a;
        if (value > b) return b;
        return value;
    }

    public static int Round(float value)
    {
        int t = Floor(value);
        if (Abs(value) - Abs(t) >= 0.5f) t++;
        return t;
    }

    public static float InverseLerp(float a, float b, float value)
    {
        return (a - value) / (b - a);
    }

    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    public static float PointOnCircle(float value)
    {
        return Mathf.Sqrt(1 - value * value);
    }

    public static float Curve(float value)
    {
        return value * value * value * (value * (value * 6f - 15f) + 10f);
    }

    public static float Abs(float value)
    {
        if (value < 0) return -value;
        return value;
    }

    public static int Abs(int value)
    {
        if (value < 0) return -value;
        return value;
    }

    public static float Power(float value, float power)
    {
        if (power == 0) return 1;

        float a = value;

        for (int i = 1; i < Abs(power); ++i)
        {
            a *= value;
        }

        if (power < 0)
        {
            a = 1f / a;
        }

        return a;
    }
}
