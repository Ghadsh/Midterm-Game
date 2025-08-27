using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class mov : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 5f;
   

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(Horizontal, -0.1f, Vertical);
        rb.MovePosition(rb.position + move * 5 * Time.fixedDeltaTime);
        

    }


}

