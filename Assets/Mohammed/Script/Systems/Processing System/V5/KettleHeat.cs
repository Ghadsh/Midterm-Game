using UnityEngine;

public class KettleHeat : MonoBehaviour
{
    [Header("Thermal")]
    public float tempC = 20f;
    public float maxTempC = 100f;
    public float heatPerLevelPerSec = 12f;
    public float coolPerSec = 6f;

    [Header("Water")]
    public float waterMl = 250f;
    public float waterMaxMl = 300f;

    [Header("Detection")]
    public LayerMask heaterMask;
    public float heaterCheckRadius = 0.2f;
    public Transform heaterProbe; // point near the kettle bottom

    HeaterController _heater; // optional reference if you want to cache

    void Reset()
    {
        if (!heaterProbe)
        {
            var t = new GameObject("HeaterProbe").transform;
            t.SetParent(transform, false);
            t.localPosition = new Vector3(0f, -0.15f, 0f);
            heaterProbe = t;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;
        bool heating = TryFindHeater(out int level);

        if (heating && level > 0)
            tempC = Mathf.Min(maxTempC, tempC + heatPerLevelPerSec * level * dt);
        else
            tempC = Mathf.Max(20f, tempC - coolPerSec * dt); // ambient = 20C
    }

    bool TryFindHeater(out int level)
    {
        level = 0;
        var pos = heaterProbe ? heaterProbe.position : transform.position;
        var cols = Physics.OverlapSphere(pos, heaterCheckRadius, heaterMask, QueryTriggerInteraction.Collide);
        for (int i = 0; i < cols.Length; i++)
        {
            var hc = cols[i].GetComponentInParent<HeaterController>();
            if (hc != null) { level = hc.Level; return true; }
        }
        return false;
    }
}
