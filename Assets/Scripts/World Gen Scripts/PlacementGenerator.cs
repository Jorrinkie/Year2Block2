using System.Collections;
using UnityEngine;

public class PlacementGenerator : MonoBehaviour
{
    EndlessTerrain endlessTerrain;
    EndlessTerrain.TerrainChunk terrainChunk;

    [SerializeField] public GameObject prefab;
    [SerializeField] int density;
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;
    [SerializeField] Vector2 xRange;
    [SerializeField] Vector2 zRange;
    [SerializeField, Range(0, 1)] float rotateTowardsNormal;
    [SerializeField] Vector2 rotationRange;
    [SerializeField] Vector3 minScale;
    [SerializeField] Vector3 maxScale;

    Vector2 previousChunk;

    private void Start()
    {
        endlessTerrain = GetComponent<EndlessTerrain>();
        MeshGenerator1.OnTerrainMeshGenerated += HandleTerrainGenerated;
    }

    private void Update()
    {
        Vector2 currentChunk = GetCurrentChunk();

        if (currentChunk != previousChunk)
        {
            StartCoroutine(GeneratePrefabs(currentChunk));
            previousChunk = currentChunk;
        }
    }

    IEnumerator GeneratePrefabs(Vector2 currentChunk)
    {
        if (endlessTerrain.terrainChunkDictionary.ContainsKey(currentChunk))
        {
            terrainChunk = endlessTerrain.terrainChunkDictionary[currentChunk];

            if (!terrainChunk.isVisible())
                // Deactivate existing prefabs for this chunk
                terrainChunk.DeactivatePrefabs();

            if (terrainChunk.isVisible())
            {
                

                for (int i = 0; i < density; i++)
                {
                    float sampleX = UnityEngine.Random.Range(terrainChunk.position.x - (endlessTerrain.chunkSize / 2), terrainChunk.position.x + (endlessTerrain.chunkSize / 2));
                    float sampleY = UnityEngine.Random.Range(terrainChunk.position.y - (endlessTerrain.chunkSize / 2), terrainChunk.position.y + (endlessTerrain.chunkSize / 2));

                    Vector3 rayStart = new Vector3(sampleX, maxHeight, sampleY);

                    if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                        continue;

                    if (hit.point.y < minHeight)
                        continue;

                    GameObject instantiatePrefab = Instantiate(prefab, hit.point, Quaternion.identity);
                    instantiatePrefab.transform.Rotate(Vector3.up, UnityEngine.Random.Range(rotationRange.x, rotationRange.y), Space.Self);
                    instantiatePrefab.transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.FromToRotation(instantiatePrefab.transform.up, hit.normal), rotateTowardsNormal);
                    instantiatePrefab.transform.localScale = new Vector3(
                        UnityEngine.Random.Range(minScale.x, maxScale.x),
                        UnityEngine.Random.Range(minScale.y, maxScale.y),
                        UnityEngine.Random.Range(minScale.z, maxScale.z));

                    // Store instantiated prefab in the terrain chunk
                    terrainChunk.AddPrefab(instantiatePrefab);
                    instantiatePrefab.SetActive(true);
                }
            }
        }

        yield return null;
    }

    void HandleTerrainGenerated()
    {
        // Handle terrain generation
    }

    Vector2 GetCurrentChunk()
    {
        int currentChunkCoordX = Mathf.RoundToInt(EndlessTerrain.viewerPos.x / endlessTerrain.chunkSize);
        int CurrentChunkCoordY = Mathf.RoundToInt(EndlessTerrain.viewerPos.y / endlessTerrain.chunkSize);

        return new Vector2(currentChunkCoordX, CurrentChunkCoordY);
    }
}
