using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//Use physics.raycast to check if the player is looking at the object, CHECK
//If the player looks at it for 1 seconds then
//Sound effect will play

public class InteractMusic : MonoBehaviour
{
    // i added this variable so you can store the default look time somewhere since the looktimeleft will change throughout the script
    public float DefaultLookTime = 1f;

    public LayerMask ignoreMe;
    public int sceneNumber;

    public AudioSource duckSound;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 150, ignoreMe))

        {
            Debug.Log("quack");
            duckSound.Play(0);
        }
    }








//physical representation of the line
void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 150;
        Gizmos.DrawRay(transform.position, direction);
    }
}
