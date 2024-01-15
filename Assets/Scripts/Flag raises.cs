using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flagraises : MonoBehaviour
{
    [SerializeField] private bool play;
    [SerializeField] private Animator Flag;

    // Update is called once per frame
    void Update()
    {
        if (play == true)
        {
            Flag.SetTrigger("Flag_raise");
            play = false;
        }

    }
}
