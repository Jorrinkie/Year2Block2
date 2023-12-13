using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//Use physics.raycast to check if the player is looking at the object, CHECK
//If the player looks at it for 5 seconds then
//Load new scene

public class PlayerGazeTracker : MonoBehaviour
{
    // i added this variable so you can store the default look time somewhere since the looktimeleft will change throughout the script
    public float DefaultLookTime = 3f;
    //The time it takes to look at the sphere so you can move on
    public float lookTimeLeft = 3.0f; 
    public LayerMask ignoreMe;
    public int sceneNumber; 

    

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 150, ignoreMe))

        {
            //First, decrease the look time by 1 every second
            lookTimeLeft -= Time.deltaTime;

            //Second, check if look time == 0
            if (lookTimeLeft <= 0)
            {

                //If it is 0 we will load a new scene

                switch (sceneNumber)

                {
                    //Start with scene number 0
                    case 0:
                        SceneManager.LoadScene(1);
                        break;
                    case 1:
                        SceneManager.LoadScene(2);
                        break;
                    case 2:
                        SceneManager.LoadScene(0);
                        break;
                }

                //Increases the scene number by 1 each time you switch
                sceneNumber++;

                //override for the starterscene
                if (hit.transform.tag == "Balloon")
                {
                    SceneManager.LoadScene(1);
                    Debug.Log("balloon");
                }
            }


        }

        else lookTimeLeft = 3f;
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


