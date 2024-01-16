using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{

    [SerializeField] private GameObject boat;
    [SerializeField] private GameObject DotProduct;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 playerForward = transform.right;
        Vector3 PlayerToBoat = DotProduct.transform.position - playerForward;

        float dotpro = Vector3.Dot(playerForward, PlayerToBoat) + 1;


        if (dotpro > 0.03 && dotpro < 0.07)
        {
            
            Debug.Log("rotate right");
            boat.transform.Rotate(0, dotpro, 0);
            transform.Rotate(0, dotpro * -1, 0);
        }
        
        if (dotpro < -0.03 && dotpro > -0.07)
        {
            
            Debug.Log("rotate left");
            boat.transform.Rotate(0, dotpro, 0);
            transform.Rotate(0, dotpro * -1, 0);
        }

    }
}
