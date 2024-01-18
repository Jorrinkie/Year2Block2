using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


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
    [SerializeField] private GameObject Tree;
    [SerializeField] private Animator CastleAnim;
    public int TimerLook;
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text text2;
    [SerializeField] private float RoundedNumber;

  



    // Update is called once per frame
    void Update()
    {
        if (text != null)
        {
            RoundedNumber = Mathf.Round(lookTimeLeft);
            text.SetText(RoundedNumber.ToString());
            text2.SetText(RoundedNumber.ToString());
        }
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 900, ignoreMe))

        {
            //First, decrease the look time by 1 every second
            lookTimeLeft -= Time.deltaTime;

            //Second, check if look time == 0
            if (lookTimeLeft <= 0)
            {
                
                if (hit.transform.tag == "BalloonLevel")
                {
                    SceneManager.LoadScene(1);
                    Debug.Log("balloon level loaded");
                }

                if (hit.transform.tag == "BoatLevel")
                {
                    SceneManager.LoadScene(3);
                    Debug.Log("boat level loaded");
                }
            }

            if (hit.transform.tag == "Falling Tree")
            {
                Debug.Log("TREE");

                if (Tree != null)
                {
                    EventManager.current.Treelook();
                    
                }
            }

            if (hit.transform.tag == "Castle Gate")
            {
                Debug.Log("Gate");

                if (CastleAnim != null)
                {
                    CastleAnim.SetTrigger("GO");
                }
            }


        }

        else lookTimeLeft = 3f;
    }



}


