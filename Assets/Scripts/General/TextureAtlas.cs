using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAtlas : MonoBehaviour
{
    public static TextureAtlas GameTextures { private set; get; }

    public Texture2D[] tiles;
    int textureRows;

    void Awake()
    {
        if (GameTextures != null && GameTextures != this)
        {
            Destroy(GameTextures);
        }
        else
        {
            GameTextures = this;
        }
        textureRows = Mathf.CeilToInt(Mathf.Sqrt(tiles.Length));
        if (textureRows < 1) textureRows = 1;
    }

    public Texture2D BuildAtlas()
    {
        int imgSize = 1;

        for (int i = 0; i < tiles.Length; ++i)
        {
            int w = tiles[i].width;
            if (w > imgSize)
            {
                imgSize = w;
            }
        }

        Texture2D atlas = new Texture2D(imgSize * tRows, imgSize * tRows);
        Color[] pixelMap = new Color[imgSize * tRows * imgSize * tRows];
        
        for (int t = 0; t < tiles.Length; ++t)
        {
            int x = t % tRows * imgSize;
            int y = t / tRows * imgSize;
            int scale = imgSize / tiles[t].width;
            if (scale < 1) scale = 1;

            for(int h = 0; h < imgSize; ++h)
            {
                for (int w = 0; w < imgSize; ++w)
                {
                    int pIndex = (y + h) * imgSize * tRows + x + w;
                    pixelMap[pIndex] = tiles[t].GetPixel(w / scale, h / scale);
                }
            }
        }

        atlas.SetPixels(pixelMap);
        atlas.filterMode = FilterMode.Point;
        atlas.wrapMode = TextureWrapMode.Clamp;
        atlas.Apply();

        return atlas;
    }

    public int tRows
    {
        get
        {
            return textureRows;
        }
    }

    public float tSize
    {
        get
        {
            return 1f / tRows;
        }
    }
}
