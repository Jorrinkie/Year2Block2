using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabGeneration : MonoBehaviour
{
    //need to get chunk reference and location
    //Then pick a random spot on that chunk 
    //Check the height of that spot 
    //Compare height against the height of the regions
    //Pick a prefab from region list at random
    //Instantiate that prefab at designated spot

    //Activate and deactivate prefab based on if it is visible 

    float chunkSize = MapGenerator.mapChunkSize;
    public int numberOfPrefabsPerChunk = 20;
    List<Vector2> prefabPositons;
    EndlessTerrain.TerrainChunk chunkBorders;

     void DetermineSize()
    {
       // chunkBorders = new Vector2(chunkBorders.position.x, chunkBorders.position.y); 
    }
    //List<Vector3> GenerateRandomPositions()
    //{
    //    List<Vector3> positions = new List<Vector3>();

    //    for (int i = 0; i < numberOfPrefabsPerChunk; i++)
    //    {
    //        positions.Add(GetRandomPositions());
    //    }
    //}

    //Vector3 GetRandomPositons()
    //{
    //    for (int y = 0; y < MapGenerator.mapChunkSize; y++) 
    //    {
    //        for (int x = 0; x < MapGenerator.mapChunkSize; x++)
    //        {

    //        }
    //    }
    //    //get chunksize x and y 
    //    //pick random value for x and y 
    //    //compare that to the noise map and find out the height
    //    //return with the point

    //    //Do all that in a for loop for the amount of points we want 
        
    //    //do it everytime a new chunk is generated
    //}

}
