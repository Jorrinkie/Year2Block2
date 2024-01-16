using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPCount : MonoBehaviour
{
    public int count;
    private int Checkpointcount = 3;
    private bool RunOnce = true;
    private void Update()
    {
        if (count >= Checkpointcount && RunOnce)
        {
            RunOnce= false;
            Debug.Log("All checkpoints passed!"); 
            // You can trigger an event, end the level, or perform any other action here
        }
    }



       
}
