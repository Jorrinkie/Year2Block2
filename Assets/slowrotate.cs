using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowrotate : MonoBehaviour
{
    [SerializeField] GameObject boat;
    [SerializeField] float turnspeed = 50f;
    // Start is called before the first frame update
    void Start()
    {

    }

 
    void Update()
    {
        if (Camera.main.transform.rotation.y > transform.rotation.y + 0.2)
        {
            boat.transform.Rotate(0, Time.deltaTime * Camera.main.transform.rotation.y * turnspeed, 0);
        }
        if (Camera.main.transform.rotation.y < transform.rotation.y - 0.2)
        {
            boat.transform.Rotate(0, Time.deltaTime * Camera.main.transform.rotation.y * turnspeed, 0);
        }

        Debug.Log(Camera.main.transform.rotation.y + "  " + transform.rotation.y);
    }




 
}
