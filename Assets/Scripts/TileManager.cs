using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    public GameObject[] TilePrefabs;
    private Transform playerTransform;
    private float spawnZ = 0.0f;
    private float tileLenght = 50f;
    private int TilesOnScreen = 8;
    private float Safezone = 200f;
    private int lastPrefabIndex = 0;

    private List<GameObject> activeTiles;
    // Start is called before the first frame update
    void Start()
    {
        activeTiles = new List<GameObject>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        for (int i = 0; i < TilesOnScreen; i++)
        {
            if ( i < 1 )
            {
                //spawn tile number 1 first for the first 2
                SpawnTile(0);

                
            }
            else
            {
                //random spawn
                SpawnTile();
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform.position.z - Safezone > (spawnZ - TilesOnScreen * tileLenght)) 
        {
            SpawnTile();
            DeleteTile();
        }
    }

    private void SpawnTile(int prefabIndex = -1)
    {
        GameObject go;
        if (prefabIndex == -1)
        {
            go = Instantiate(TilePrefabs[RandomPrefabIndex()]) as GameObject;
        }
        else
        {
            go = Instantiate(TilePrefabs[prefabIndex]) as GameObject;
        }
        go.transform.parent = transform;
        go.transform.position = Vector3.forward * spawnZ;
        spawnZ += tileLenght;
        activeTiles.Add (go);
    }

    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    private int RandomPrefabIndex()
    {
        if (TilePrefabs.Length <= 1)
            return 0;

        int randomIndex = lastPrefabIndex;
        while(randomIndex == lastPrefabIndex)
        {
            randomIndex = Random.Range(0, TilePrefabs.Length);
        }

        lastPrefabIndex = randomIndex;
            return randomIndex;
    }
}
