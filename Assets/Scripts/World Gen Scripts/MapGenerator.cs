using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;


public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, Mesh, FallOffMap}
    public DrawMode drawMode;

    public TerrainData terrainData;
    public NoiseData noiseData;
    public TextureData textureData;

    public Material terrainMaterial;

    [Range(0,6)]
    public int editorPreviewLoD;

    public bool autoUpdate;

    float[,] fallOffMap;

    Queue<MapThreatInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreatInfo<MapData>>();
    Queue<MapThreatInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreatInfo<MeshData>>();

    void OnValuesUpdated()
    {
        if(!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }
    public int mapChunkSize
    {
        //Dont forget the actual size is +1 so the actual dimensions are 240x240 
        //Even though over here it is 239 we add two when generating and subtract 1 somewhere else
        //Because flatshading creates a ton of extra vertices and there is a max amount of vertices for a single mesh in unity it is reduced from 239 to 95
        get
        {
            if (terrainData.useFlatShading)
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
        else if (drawMode == DrawMode.Mesh)
            display.DrawMesh(MeshGenerator1.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorPreviewLoD, terrainData.useFlatShading));
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

        if (terrainData.useFallOff)
        {
            if (fallOffMap == null)
                fallOffMap = FallOffGenerator.GenerateFallOffMap(mapChunkSize + 2);
        }
       
        for (int y=0; y<mapChunkSize+2; y++)
        {
            for (int x=0; x<mapChunkSize+2; x++)
            {
                if (terrainData.useFallOff)
                {
                    noiseMap[x,y] = Mathf.Clamp01(noiseMap[x,y] - fallOffMap[x,y]);
                }

                
            }
        }

        textureData.UpdateMeshHeight(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
        Debug.Log("MinHeight " + terrainData.minHeight);
        Debug.Log("MaxHeight " + terrainData.maxHeight);


        return new MapData(noiseMap);
    }

    
    private void OnValidate()
    {
        if(terrainData != null)
        {
            //To prevent multiple subscribers we first remove all subscribers which only does something if there are already subscribers 
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated; 
        }

        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }

        if(textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
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
   
    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}