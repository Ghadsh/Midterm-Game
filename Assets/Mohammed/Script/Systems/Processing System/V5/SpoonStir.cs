using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpoonStir : MonoBehaviour
{
    public CupContainer cup;
    public Transform stirPlaneRef;   // center of cup (use cup transform)
    public float gainPerMeter = 0.5f; // how fast progress accumulates per meter moved inside cup
    public float insideRadius = 0.12f;

    Vector3 _lastPos;
    bool _inside;

    void Reset()
    {
        var col = GetComponent<Collider>(); col.isTrigger = true;
        if (!stirPlaneRef && cup) stirPlaneRef = cup.transform;
    }

    void Update()
    {
        if (!_inside || cup == null) return;

        Vector3 p = transform.position;
        Vector3 center = (stirPlaneRef ? stirPlaneRef.position : Vector3.zero);
        Vector3 d = p - center; d.y = 0f;

        if (d.magnitude <= insideRadius)
        {
            float dist = (p - _lastPos).magnitude; // world-space distance this frame
            cup.AddStir(dist * gainPerMeter);
        }

        _lastPos = p;
    }

    void OnTriggerEnter(Collider other)
    {
        if (cup && other.GetComponentInParent<CupContainer>() == cup)
        {
            _inside = true;
            _lastPos = transform.position;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (cup && other.GetComponentInParent<CupContainer>() == cup)
            _inside = false;
    }
}
