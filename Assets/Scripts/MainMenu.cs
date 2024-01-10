using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//Use physics.raycast to check if the player is looking at the object, CHECK
//If the player looks at it for 5 seconds then
//Load new scene

public class MainMenu : MonoBehaviour
{
    // i added this variable so you can store the default look time somewhere since the looktimeleft will change throughout the script
    [SerializeField] float DefaultLookTime = 3f;
    //The time it takes to look at the sphere so you can move on
    [SerializeField] float lookTimeLeft = 3.0f;
    [SerializeField] LayerMask ignoreMe;
    [SerializeField] int sceneNumber;



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
                
                if (hit.transform.tag == "BalloonLevel")
                {
                    SceneManager.LoadScene(3);
                    Debug.Log("balloon level loaded");
                }

                if (hit.transform.tag == "BoatLevel")
                {
                    SceneManager.LoadScene(1);
                    Debug.Log("balloon level loaded");
                }
            }


        }

        else lookTimeLeft = 3f;
    }



}


