using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TextureData : UpdatableData
{
    private float minHeight, maxHeight;

    public void ApplyToMaterial(Material material)
    {

    }

    public void UpdateMeshHeight(Material material, float minHeight, float maxHeight)
    {
        //this.minHeight = minHeight;
       // maxHeight = maxHeight;  
        Debug.Log("Heights Updated");
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }

}
