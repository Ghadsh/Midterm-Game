using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class OrientationFollow : MonoBehaviour
{
    public Transform cam;

    void Update()
    {
        Vector3 dir = cam.forward;
        dir.y = 0f; 
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
