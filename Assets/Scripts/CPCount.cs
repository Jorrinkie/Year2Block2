using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPCount : MonoBehaviour
{
    DrunkardsWalk drunkardsWalk;
    public int count;
    private int Checkpointcount;
    private bool RunOnce = true;

    private void Start()
    {
        drunkardsWalk = GetComponent<DrunkardsWalk>();
    }
    private void Update()
    {
        if (drunkardsWalk.drunkWalkStarted) 
        {
            Checkpointcount = drunkardsWalk.stepsToTake;
        }
       
        if (count >= Checkpointcount && drunkardsWalk.drunkWalkStarted)
        {
           // RunOnce= false;
            Debug.Log("All checkpoints passed!"); 
            // You can trigger an event, end the level, or perform any other action here

            //Save score or something
            //Reset system
        }
    }



       
}
