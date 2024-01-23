using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;


public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh, FallOffMap}
    public DrawMode drawMode;

    public TerrainData terrainData;
    public NoiseData noiseData;

    [Range(0,6)]
    public int editorPreviewLoD;

    public bool autoUpdate;

    public TerrainType[] regions;
    static MapGenerator instance;

    public List<Vector3> spawnPoints = new List<Vector3>();
    public int maxSpawnPointsPerChunk = 20; 

    float[,] fallOffMap;

    Queue<MapThreatInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreatInfo<MapData>>();
    Queue<MapThreatInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreatInfo<MeshData>>();

    private void Awake()
    {
        fallOffMap = FallOffGenerator.GenerateFallOffMap(mapChunkSize);
    }

    void OnValuesUpdated()
    {
        if(!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }
    public static int mapChunkSize
    {
        //Dont forget the actual size is +1 so the actual dimensions are 240x240 
        //Even though over here it is 239 we add two when generating and subtract 1 somewhere else
        //Because flatshading creates a ton of extra vertices and there is a max amount of vertices for a single mesh in unity it is reduced from 239 to 95
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MapGenerator>();
            }

            if (instance.terrainData.useFlatShading)
            {
                return 95;
            }
            else
            {
                return 239;
            }
        }

    }
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        else if (drawMode == DrawMode.ColorMap)
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        else if (drawMode == DrawMode.Mesh)
            display.DrawMesh(MeshGenerator1.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorPreviewLoD, terrainData.useFlatShading), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        else if (drawMode == DrawMode.FallOffMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOffGenerator.GenerateFallOffMap(mapChunkSize)));
    }

    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(centre, callback);
        };

        //Threadstart is a type which defines what the Thread is going to do. This is required when a new thread is declared 
        //Delegate means to execute 
        //We create this function so when the game is being played and new mapdata needs to be created it can happen so in the background without affecting the players Update
        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        //We now execute this method inside of the thread
        MapData mapData = GenerateMapData(centre);

        //This "lock" prevents other code from accesing this thread while it is active becuase it wouldn't like that 
        //So when this is being executed but other code also wants to do something with the thread the other code will have to wait upon completion of this code below
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreatInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };

        new Thread (threadStart).Start();
    }

    void MeshDataThread(MapData mapData,int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator1.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, lod, terrainData.useFlatShading);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreatInfo<MeshData>(callback, meshData));
        }
    }
    private void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreatInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreatInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }
    MapData GenerateMapData(Vector2 centre)
    {
        float [,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize +2, mapChunkSize +2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, centre + noiseData.offset, noiseData.normalizeMode);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y=0; y<mapChunkSize; y++)
        {
            for (int x=0; x<mapChunkSize; x++)
            {
                if (terrainData.useFallOff)
                {
                    noiseMap[x,y] = Mathf.Clamp01(noiseMap[x,y] - fallOffMap[x,y]);
                }
                float currentHeight = noiseMap[x, y];
                for (int z=0; z<maxSpawnPointsPerChunk; z++)
                {
                    Vector3 spawnPoint = new Vector3(x, currentHeight, y);
                   
                }
                
                for (int i =0; i<regions.Length; i++)
                {
                    if (currentHeight >= regions[i].height)
                    {
                        //If this is true we found the region we need to affect
                        //Now we want to save the color for this region
                        colorMap[y* mapChunkSize + x] = regions[i].color;

                       
                    }
                    else
                    {
                        //Then we break because we don't need to check the other regions for the one we are looking for
                        break;
                    }
                }
            }
        }

       
        return new MapData(noiseMap, colorMap);
        

    }

    //Used to clamp these variables to 1 and 0 respectivly otherwise things break
    //Gets activated only when they get changed 
    private void OnValidate()
    {
        if(terrainData != null)
        {
            //To prevent subscribing multiple times to value update event we first unsubscribe with the -= and then subscribe again
            //If we aren't subscribed yet the -= does nothing 
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated; 
        }

        if(noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }

        fallOffMap = FallOffGenerator.GenerateFallOffMap(mapChunkSize);
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;

        //Building Gen
        public GameObject[] prefabs;
    }

    //I think we create this struct so we can use the "map data" outside of the threat it is running in 
    //The T means that the struct is generic which we do so we can also use it both for the mapdata (eg. heightmap, colormap) and the mesh data 
    struct MapThreatInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreatInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

}

//To get the data from the Generate Map function we need this struct 
public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}