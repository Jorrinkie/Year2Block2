using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject[] buildingPrefabs;
    public int numberOfBuildings;

    public float spawnRadius = 50f; // Define the radius around the player
    public LayerMask terrainLayer;
    Transform playerTransform; // Reference to player's transform

    Vector2 previousChunk;

    bool isTerrainReady = false;

    List<Vector3> randomPosList = new List<Vector3>();
    void Start()
    {
        MeshGenerator1.OnTerrainMeshGenerated += HandleTerrainGenerated;

        // Assuming the player's GameObject has a "Player" tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
           
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }
    }
    void Update()
    {
        Vector2 currentChunk = GetCurrentChunk();

        if (currentChunk != previousChunk)
        {
            StartCoroutine(SpawnBuildings());
            previousChunk = currentChunk;
        }
    }
    void HandleTerrainGenerated()
    {
        isTerrainReady = true;
    }
    private void GenerateBuildingsAfterTerrainMesh()
    {
        GenerateBuildings();
    }

    void GenerateBuildings()
    {
        StartCoroutine(SpawnBuildings());
    }

    IEnumerator SpawnBuildings()
    {
        for (int i = 0; i < numberOfBuildings; i++)
        {
            Vector3 randomPosition = GetRandomPositionAroundPlayer();
            randomPosList.Add(randomPosition);


            for (int j = 0; j < numberOfBuildings; j++)
            {
                
                RaycastHit hit;
                if (Physics.Raycast(randomPosition, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
                {
                    Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    GameObject selectedBuilding = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                    //GameObject newBuilding = Instantiate(selectedBuilding, hit.point, randomRotation);





                    // Adjust the newBuilding's scale, orientation, alignment with terrain, etc.

                    //Add positions to a list 
                    //When position is used get rid of position on viable pos list to prevent multiple buiildings on spawning on each other 

                }
            }

            yield return null; // Yield to the main thread
        }
    }

    Vector2 GetCurrentChunk()
    {
        Vector2 chunk = new Vector2(Mathf.Floor(playerTransform.position.x / MapGenerator.mapChunkSize),
        Mathf.Floor(playerTransform.transform.position.z / MapGenerator.mapChunkSize));

        Debug.Log(chunk);

        return chunk;
    }

    Vector3 GetRandomPositionAroundPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 randomPosition = new Vector3(randomCircle.x, 0, randomCircle.y);
        
        

        return playerTransform.position + randomPosition;
    }
}
