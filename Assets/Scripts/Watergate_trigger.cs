using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watergate_trigger : MonoBehaviour
{
    [SerializeField] private Animator Watergate;
    [SerializeField] private bool play;
 
    // Update is called once per frame
    void Update()
    {
        if (play == true) {
            Watergate.SetTrigger("gate_raises");
            play = false;
        }
    }
}
