using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StationProximityTrigger : MonoBehaviour
{
    public StationFocus stationFocus;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
        if (!stationFocus) stationFocus = GetComponentInParent<StationFocus>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) stationFocus?.SetInRange(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) stationFocus?.SetInRange(false);
    }
}
