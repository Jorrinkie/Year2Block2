using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    // List to store the checkpoints
    [SerializeField] int Checkpointcount = 3;
    private CPCount count;
    // Counter for the number of checkpoints driven through
    private int checkpointsPassed = 0;


    private void Start()
    {
        count = transform.parent.GetComponent<CPCount>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           count.count++;
            Debug.Log("Checkpoint passed!" + " " + count.count);
           

            // Check if all checkpoints have been passed
            if (checkpointsPassed >= Checkpointcount)
            {
                Debug.Log("All checkpoints passed!");
                // You can trigger an event, end the level, or perform any other action here
            }
            gameObject.GetComponent<Renderer>().material.color = Color.green;
            Destroy(gameObject, 0.5f);
        }
    }
}
