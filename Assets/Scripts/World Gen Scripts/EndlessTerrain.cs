using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static EndlessTerrain;

public class EndlessTerrain : MonoBehaviour
{
    const float viewerThresholdForChunkUpdate = 25f;
    //We square it because it is quicker this way to get the distance then using the sqr root operator 
    const float sqrviewerThresholdForChunkUpdate = viewerThresholdForChunkUpdate * viewerThresholdForChunkUpdate;

    public LODInfo[] detailLevels;
    public static float maxViewDist;

    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 viewerPos;
    Vector2 viewerPosOld;
    static MapGenerator mapGenerator;
    public int chunkSize;
    int chunksVisibleInViewDist;

    //Used to keep track of existing chunks
    public Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk> ();
    //This list will be used to disable chunks
    public static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk> ();

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator> ();

        maxViewDist = detailLevels[detailLevels.Length -1].visibleDistThreshold;

        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);

        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPos = new Vector2(viewer.position.x, viewer.position.z) / mapGenerator.terrainData.uniformScale;

        if ((viewerPosOld - viewerPos).sqrMagnitude > sqrviewerThresholdForChunkUpdate)
        {
            viewerPosOld = viewerPos;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        //Get current chunk Coordinate
        int currentChunkCoordX = Mathf.RoundToInt(viewerPos.x/ chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPos.y/ chunkSize);

        for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                //We want to know if a chunk has already been created on said coordinates to prevent chunks spawning on eachother 
                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        public Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;
        LODMesh collisionLoDMesh;

        MapData mapData;
        bool mapDataReceived;
        int previousLoDIndex = -1;

        List<GameObject> spawnedPrefabs = new List<GameObject> ();

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;


            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            int terrainLayer = LayerMask.NameToLayer("TerrainChunk");

            
            meshObject = new GameObject("Terrain Chunk");
            

            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = material;
            meshObject.layer = terrainLayer;
           
            meshObject.transform.position = positionV3 * mapGenerator.terrainData.uniformScale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformScale;

            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
                if (detailLevels[i].useForCollider)
                {
                    collisionLoDMesh = lodMeshes[i];
                }
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunk();
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void UpdateTerrainChunk()
        {
            if (mapDataReceived)
            {
                float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
                bool visible = viewerDistFromNearestEdge <= maxViewDist;

                

                if (visible)
                {
                    int lodIndex = 0;
                    //In this for loop we check if the correct LoD is displayed compared to the distance of the viewer
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDistFromNearestEdge > detailLevels[i].visibleDistThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lodIndex != previousLoDIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLoDIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }

                    if(lodIndex == 0)
                    {
                        if (collisionLoDMesh.hasMesh)
                        {
                            meshCollider.sharedMesh = collisionLoDMesh.mesh;
                            //SpawnPrefabBasedOnColormap();
                        }
                        else if (!collisionLoDMesh.hasRequestedMesh)
                        {
                            collisionLoDMesh.RequestMesh(mapData);
                        }
                    }

                    //Updates the list with all the visible chunks 
                    terrainChunksVisibleLastUpdate.Add(this); 
                }

                SetVisible(visible);
            }
        }
        
      
        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool isVisible()
        {
            return meshObject.activeSelf;
        }

        void SpawnPrefabBasedOnColormap()
        {
            GameObject treePrefab = mapGenerator.treePrefab;
            float[,] heightmap = mapData.heightMap;
            float minTreeSpawn = .4f;
            float maxTreeSpawn = .55f;
            float chunkSize = MapGenerator.mapChunkSize - 1;

            float randomizationFactor = .8f;
            float maxTreesPerChunk = 60;
            int treeSpawned = 0;

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    float heightValue = heightmap[x, y];
                    
                    if (heightValue >= minTreeSpawn && heightValue <= maxTreeSpawn)
                    { 
                        if (UnityEngine.Random.value < randomizationFactor)
                        {
                            float randomX = position.x + x + UnityEngine.Random.Range(-50f, 50f);
                            float randomZ = position.y + y + UnityEngine.Random.Range(-50f, 50f);
                            Vector3 pixelWorldPosition = new Vector3(randomX, heightValue * mapGenerator.terrainData.uniformScale, randomZ);

                            
                                Instantiate(treePrefab, pixelWorldPosition, Quaternion.identity);
                                treeSpawned++;
                            

                            if (treeSpawned >= maxTreesPerChunk)
                                return;
                        }
                    }

                }
            }
        }

        bool isInCorrectRegion(Vector3 position)
        {
            float[,] heightmap = mapData.heightMap;
            float regionThreshold = 0.5f;
            int x = Mathf.RoundToInt(position.x / mapGenerator.terrainData.uniformScale);
            int y = Mathf.RoundToInt(position.z / mapGenerator.terrainData.uniformScale);

            if (x >= 0 && x < heightmap.GetLength(0) && y >= 0 && y < heightmap.GetLength(1))
            {
                float regionHeight = heightmap[x, y];

                return regionHeight > regionThreshold;
            }

            return false;
        }

        public void AddPrefab(GameObject prefab)
        {
            spawnedPrefabs.Add(prefab);
        }

        public void DeactivatePrefabs()
        {
            foreach (var prefab in spawnedPrefabs)
            {
                prefab.SetActive(false);
            }
        }

    }

    //This class will be responsible for fetching the correct mesh from the map generator 
    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;
        TerrainChunk terrainChunk;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;
            updateCallback();
        }
        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
            //terrainChunk.CreateGrid();
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDistThreshold;
        public bool useForCollider;
    }
}

