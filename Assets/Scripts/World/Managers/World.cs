using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World WorldMap { private set; get; }

    [Header("Materials")]
    public Material[] worldMats;
    public TileData[] tiles;
    public Shapes[] shapes;
    public Biome[] biomes;

    [Header("World Data")]
    protected Dictionary<Vector2Int, Chunk> chunkMap;
    public int viewDistance;
    public int worldSeed;
    public GenRules worldGen;

    // Start is called before the first frame update
    void Awake()
    {
        if (WorldMap != null && WorldMap != this)
        {
            Destroy(gameObject);
        }
        else
        {
            WorldMap = this;
        }
        chunkMap = new Dictionary<Vector2Int, Chunk>();
    }

    // Update is called once per frame
    void Start()
    {
        worldMats[0].mainTexture = TextureAtlas.GameTextures.BuildAtlas();
        BuildMap(new Vector2Int(0, 0));

        if (PMovement.Player != null)
        {
            int pHeight = GetHeight(new Vector2(0, 0));
            PMovement.Player.SetSpawn(new Vector3(0.5f, pHeight, 0.5f));
        }
    }

    void BuildMap(Vector2Int center)
    {
        for(int x = -viewDistance; x <= viewDistance; ++x)
        {
            for(int y = -viewDistance; y <= viewDistance; ++y)
            {
                AddChunk(new Vector2Int(x, y) + center);
            }
        }
    }

    public int GetHeight(Vector2 location)
    {
        int chunkX = MathFun.Floor(location.x / Chunk.ChunkSize);
        int chunkY = MathFun.Floor(location.y / Chunk.ChunkSize);
        Vector2Int chunkIndex = new Vector2Int(chunkY, chunkX);

        if(chunkMap.ContainsKey(chunkIndex))
        {
            int posX = MathFun.Floor(location.x - chunkX * Chunk.ChunkSize);
            int posY = MathFun.Floor(location.y - chunkY * Chunk.ChunkSize);
            return chunkMap[chunkIndex].GetHeight(new Vector2Int(posX, posY));
        }
        else
        {
            return Chunk.ChunkHeight;
        }
    }

    public void AddChunk(Vector2Int coord)
    {
        if (chunkMap.ContainsKey(coord)) return;

        chunkMap.Add(coord, new Chunk(coord));

        if(chunkMap.ContainsKey(coord + CompassRose[0]))
        {
            chunkMap[coord + CompassRose[0]].AttachChunk(Chunk.ChunkDirection.North, chunkMap[coord]);
        }
        if (chunkMap.ContainsKey(coord + CompassRose[1]))
        {
            chunkMap[coord + CompassRose[1]].AttachChunk(Chunk.ChunkDirection.East, chunkMap[coord]);
        }
        if (chunkMap.ContainsKey(coord + CompassRose[2]))
        {
            chunkMap[coord + CompassRose[2]].AttachChunk(Chunk.ChunkDirection.South, chunkMap[coord]);
        }
        if (chunkMap.ContainsKey(coord + CompassRose[3]))
        {
            chunkMap[coord + CompassRose[3]].AttachChunk(Chunk.ChunkDirection.West, chunkMap[coord]);
        }
    }

    public static readonly Vector2Int[] CompassRose = new Vector2Int[]
    {
        new Vector2Int( 0, -1),
        new Vector2Int(-1,  0),
        new Vector2Int( 0,  1),
        new Vector2Int( 1,  0)
    };
}
