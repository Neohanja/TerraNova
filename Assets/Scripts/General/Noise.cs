using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    private static int a = 10001;
    private static float b = 10000f;

    // Graident Noise
    public static float Noise1D(float x, int seed)
    {
        int iX = MathFun.Floor(x);

        float cX = MathFun.Curve(x - iX);

        float x1 = RanGen.PullNumber(seed, iX) % a / b;
        float x2 = RanGen.PullNumber(seed, iX + 1) % a / b;

        return MathFun.Lerp(x1, x2, cX);
    }

    public static float Noise2D(float x, float y, int offset, int seed)
    {
        Vector2 point = new Vector2(x, y);

        int iX = MathFun.Floor(x);
        int iY = MathFun.Floor(y);

        float fX = x - iX;
        float fY = y - iY;

        float cX = MathFun.Curve(fX);
        float cY = MathFun.Curve(fY);

        Vector2[] corner = new Vector2[]
        {
            new Vector2(iX, iY),
            new Vector2(iX, iY + 1),
            new Vector2(iX + 1, iY),
            new Vector2(iX + 1, iY + 1),
        };

        float x1 = MathFun.Dot(corner[0] - point, RandomWalk(corner[0], offset, seed));
        float x2 = MathFun.Dot(corner[1] - point, RandomWalk(corner[1], offset, seed));

        float a = MathFun.Lerp(x1, x2, cY);

        x1 = MathFun.Dot(corner[2] - point, RandomWalk(corner[2], offset, seed));
        x2 = MathFun.Dot(corner[3] - point, RandomWalk(corner[3], offset, seed));

        float b = MathFun.Lerp(x1, x2, cY);

        return MathFun.Lerp(a, b, cX);
    }

    public static Vector2 RandomWalk(Vector2 pos, int offset, int seed)
    {
        int x = MathFun.Floor(pos.x);
        int y = MathFun.Floor(pos.y);

        int quad = RanGen.PullNumber(seed, x, y, offset) % 4 + 1;
        float xP = RanGen.PullNumber(seed, x, y, offset) % a / b;
        float yP = MathFun.PointOnCircle(xP);

        if (quad == 2 || quad == 3) yP *= -1;
        if (quad == 3 || quad == 4) xP *= -1;

        return new Vector2(xP, yP);
    }

    public static Vector2 VornoiNoise(Vector2 pos, int indexes, int seed)
    {
        Vector2 distanceIndex = new Vector2(2f, 0);

        int x = MathFun.Floor(pos.x);
        int y = MathFun.Floor(pos.y);

        for(int oX = -1; oX <= 1; oX++)
        {
            for (int oY = -1; oY <= 1; oY++)
            {
                //v is the 22nd letter in the alphabet, useful for a psuedo-random offset
                // prn : psuedo random number
                float prn = RanGen.PullNumber(seed, oX + x, oY + y, 22) % a / b;
                float vX = x + oX + prn;
                prn = RanGen.PullNumber(seed, oX + x, oY + y, 44) % a / b;
                float vY = y + oY + prn;
                float distance = Vector2.Distance(pos, new Vector2(vX, vY));
                if(distanceIndex.x > distance)
                {
                    distanceIndex.x = distance;
                    distanceIndex.y = RanGen.PullNumber(seed, oX + x, oY + y, 2) % indexes;
                }
            }
        }

        return distanceIndex;
    }

    // Cell Automata
    public static bool[,] CellAuto(int size, int seed, CellRules cellRules)
    {
        bool[,] grid = new bool[size, size];
        RanGen cRNG = new RanGen(seed);

        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                grid[x, y] = cRNG.Percent() <= cellRules.aliveStart;
            }
        }

        for (int smooth = 0; smooth < cellRules.smoothing; ++smooth)
        {
            grid = SmoothCell(grid, cellRules);
        }

        return grid;
    }

    public static bool[,] SmoothCell(bool[,] trans, CellRules cellRules)
    {
        int size = trans.GetLength(0);
        bool[,] grid = new bool[size, size];

        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                int points = 0;
                bool alive = trans[x, y];

                for (int cX = -1; cX <= 1; ++cX)
                {
                    for (int cY = -1; cY <= 1; ++cY)
                    {
                        int pX = x + cX;
                        int pY = y + cY;
                        if (cY == 0 && cX == 0)
                        {
                            continue;
                        }
                        else if (pX < 0 || pX >= size || pY < 0 || pY >= size)
                        {
                            points++;
                        }
                        else if (trans[pX, pY])
                        {
                            points++;
                        }
                    }
                }

                if (points > cellRules.cellBirth) alive = true;
                else if (points < cellRules.cellDeath) alive = false;
                grid[x, y] = alive;
            }
        }

        return grid;
    }
}

[System.Serializable]
public class GenRules
{
    public CellRules cellRules;
    public float vScale;
    public int minHeight;
    public int growth;
    public int seaLevel;

    [Header("Grad Noise")]
    public float gScale;
    [Range(1, 8)]
    public int octaves;
    [Range(0.01f, 2f)]
    public float amplify;
    [Range(0.01f, 4f)]
    public float frequency;
}

[System.Serializable]
public class CellRules
{
    public int cellGridSize;
    public int cellBirth;
    public int cellDeath;
    public int smoothing;
    public float aliveStart;
}