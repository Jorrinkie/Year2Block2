using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isPlaceable;
    public Vector3 position;
    public Transform obj;

    public Node(bool isPlaceable, Vector3 position)
    {
        this.isPlaceable = isPlaceable;
        this.position = position;
        
    }
}
