using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class timercount : MonoBehaviour
{
    [SerializeField] TMP_Text CountingText;
    [SerializeField] Animator animator;
    [SerializeField] float RoundedNumber;
    [SerializeField] PlayerGazeTracker tracker;

    // Update is called once per frame
    void Update()
    {
        RoundedNumber = Mathf.Round(tracker.lookTimeLeft * 10.0f) * 0.1f;
        if (tracker.lookTimeLeft < 3)
        {

            //animation trigger
            animator.SetTrigger("Start");
            //showcount
            CountingText.SetText(RoundedNumber.ToString());
        }
        else
        {
            //stop animation if the time is reset
            animator.SetTrigger("Stop");
            CountingText.SetText(tracker.DefaultLookTime.ToString());
            
            
        }
    }
}
