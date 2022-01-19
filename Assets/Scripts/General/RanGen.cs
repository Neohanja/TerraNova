using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanGen
{
    // Individual Instanced Variables
    public int seed { private set; get; }
    public int position { private set; get; }

    public RanGen(int newSeed, int startIndex = 1)
    {
        if (startIndex < 1) startIndex = 1;
        seed = newSeed;
        position = startIndex;
    }

    public int Roll(int min, int max)
    {
        int val = (PullNumber(seed, position) % (max - min + 1)) + min;
        position++;
        return val;
    }

    public float Percent()
    {
        float val = PullNumber(seed, position) % 10001f / 10000;
        position++;
        return val;
    }

    // Static classes for general use

    public static int PullNumber (int seed, int a, int b = 0, int c = 0)
    {
        int mangled = a * PrimeList[0];
        mangled += b * PrimeList[3];
        mangled += c * PrimeList[4];
        mangled += seed * PrimeList[5];
        mangled ^= mangled >> 3;
        mangled *= PrimeList[1];
        mangled ^= mangled << 5;
        mangled *= PrimeList[2];
        mangled ^= mangled >> 7;

        return mangled;
    }

    public static int EpochTime
    {
        get
        {
            System.DateTime epochTime = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

            return (int)(System.DateTime.UtcNow - epochTime).TotalSeconds;
        }
    }

    protected static int[] PrimeList = new int[]
    {
        16769023,
        479001599,
        433494437,
        370248451,
        218198423,
        479001599,
        777767777,
        715827883,
        126122527,
        53471161
    };
}
