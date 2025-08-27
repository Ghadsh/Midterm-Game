using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;         
    public Vector3 offset = new Vector3(0, 2, -4); 
    public float smoothSpeed = 5f;   

    void LateUpdate()
    {
        
        Vector3 desiredPosition = player.position + player.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        
        transform.LookAt(player);
    }
}