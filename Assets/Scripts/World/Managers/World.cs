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
