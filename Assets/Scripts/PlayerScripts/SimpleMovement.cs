using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float flyForce = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Disable gravity for flying effect
        rb.useGravity = false;
    }

    void Update()
    {
        // Movement in the air (horizontal)
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalMovement, 0f, verticalMovement) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        // Flying up and down
        float flyInput = Input.GetAxis("Jump");
        Vector3 flyDirection = Vector3.up * flyInput * flyForce;
        rb.AddForce(flyDirection, ForceMode.Force);
    }
}
