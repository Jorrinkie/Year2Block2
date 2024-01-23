using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{

    [SerializeField] private GameObject boat;
    [SerializeField] private GameObject DotProduct;
    [SerializeField] private float betterdotpro;
    [SerializeField, Range(0f, 1f)] float rotspeed; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 playerForward = transform.right;
        Vector3 PlayerToBoat = boat.transform.right;

        float dotpro = Vector3.Dot(playerForward, PlayerToBoat);

        //Debug.Log("dotpro gives = " + dotpro);
        dotpro = dotpro / 2;

        if (dotpro > 0.2 && dotpro < 0.8)
        {

            boat.transform.Rotate(0, dotpro * -1, 0);
            transform.Rotate(0, dotpro, 0);
        }

        if (dotpro < -0.2 && dotpro > -0.8)
        {

            boat.transform.Rotate(0, dotpro * -1, 0);
            transform.Rotate(0, dotpro, 0);
        }

    }
}
