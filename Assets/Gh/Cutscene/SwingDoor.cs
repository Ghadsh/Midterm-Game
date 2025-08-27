using UnityEngine;

public class SwingDoor : MonoBehaviour
{
    public Transform door;
    public float openAngle = 180f;
    public float openSpeed = 2f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool somethingNear = false;

    void Start()
    {
        closedRotation = door.rotation;
        openRotation = Quaternion.Euler(door.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            somethingNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            somethingNear = false;
        }
    }

    void Update()
    {
        if (somethingNear)
        {
            door.rotation = Quaternion.Lerp(door.rotation, openRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            door.rotation = Quaternion.Lerp(door.rotation, closedRotation, Time.deltaTime * openSpeed);
        }
    }
}
