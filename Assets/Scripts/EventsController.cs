using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsController : MonoBehaviour
{
    [SerializeField] GameObject Tree;

    // Start is called before the first frame update
    private void Start()
    {
        EventManager.current.Ontreelook += TreeEvent;
    }

    private void TreeEvent()
    {
       //make tree fall
       if (Tree != null)
        {
            Tree.gameObject.GetComponent<Rigidbody>().drag = 0;
            
        }
    }
}
