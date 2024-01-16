using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlacementGenerator : MonoBehaviour
{
    [SerializeField] GameObject prefab;


    [SerializeField] int density;

    [SerializeField] float minHeight; 
    [SerializeField] float maxHeight;
    [SerializeField] Vector2 xRange;
    [SerializeField] Vector2 zRange;

    [SerializeField, Range(0, 1)] float rotateTowardsNormal;
    [SerializeField] Vector2 rotationRange;
    [SerializeField] Vector3 minScale;
    [SerializeField] Vector3 maxScale;

    public void GeneratePrefabs()
    {
        for (int i = 0; i < density; i++) 
        {
            float sampleX = UnityEngine.Random.Range(xRange.x, zRange.y);
            float sampleY = UnityEngine.Random.Range(zRange.x, zRange.y);
            Vector3 rayStart = new Vector3(sampleX, maxHeight, sampleY);
            
            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                continue;

            if (hit.point.y < minHeight) 
                continue;

            GameObject instantiatePrefab = (GameObject)PrefabUtility.InstantiatePrefab(this.prefab, transform);
            instantiatePrefab.transform.position = hit.point;
            instantiatePrefab.transform.Rotate(Vector3.up, UnityEngine.Random.Range(rotationRange.x, rotationRange.y), Space.Self);
            instantiatePrefab.transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.FromToRotation(instantiatePrefab.transform.up, hit.normal), rotateTowardsNormal);
            instantiatePrefab.transform.localScale = new Vector3(
                UnityEngine.Random.Range(minScale.x, maxScale.x),
                UnityEngine.Random.Range(minScale.y, maxScale.y),
                UnityEngine.Random.Range(minScale.z, maxScale.z));
            
        }
    }

    public void ClearPrefab()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
