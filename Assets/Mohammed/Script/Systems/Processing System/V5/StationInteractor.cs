using UnityEngine;

public class StationInteractor : MonoBehaviour
{
    public StationFocusCamera focusCam;
    public Transform cameraAnchor;
    public Vector3 cameraLocalOffset = new Vector3(0f, 0.9f, -0.6f);
    public Vector3 cameraLocalEuler = new Vector3(5f, 0f, 0f);

    void Update()
    {
        // Example keys; wire these to your interaction flow
        if (Input.GetKeyDown(KeyCode.F)) // Enter focus
            focusCam?.EnterFocus(cameraAnchor, cameraLocalOffset, Quaternion.Euler(cameraLocalEuler));

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) // Exit focus
            focusCam?.ExitFocus();
    }
}
