using UnityEngine;
using System.Collections.Generic;

public class KettlePour : MonoBehaviour
{
    [Header("Refs")]
    public KettleHeat kettle;
    public CupContainer cup;               // (unused; we detect Cup via cast)
    public Transform pourMouth;            // tip of spout
    [SerializeField] private StationController station;   // auto-filled in Awake if left empty

    [Header("Pour Settings")]
    public float mlPerSecAtFullTilt = 120f;    // pour rate (we're not requiring tilt)
    public bool useRMBToPour = true;
    [SerializeField] private bool requireTiltToPour = false; // keep false
    public float pourAngleDeg = 50f;            // only used if requireTiltToPour = true

    [Header("Aim / Hit Settings")]
    [SerializeField] private bool useSphereAim = true;
    [SerializeField] private float aimSphereRadius = 0.06f; // ~6 cm "laser" thickness
    [SerializeField] private float maxRayDistance = 2.0f;
    [SerializeField] private LayerMask aimMask = ~0;        // "Everything" by default (hits all)
    public LayerMask cupMask;                               // ONLY Cup layer for pour
    [SerializeField] private bool ignoreSelf = true;        // ⬅ ignore kettle colliders to stop flicker

    [Header("Heat Gate")]
    [SerializeField] private bool requireHeatToPour = true;
    [SerializeField] private Color heatBadColor = Color.red;

    [Header("Debug")]
    public bool drawDebugRay = true;
    public Color aimColor = Color.yellow;
    public Color pourColor = Color.cyan;
    public float debugRayLength = 0.5f;

    // cache of our own colliders to ignore
    Collider[] _selfCols;

    void Awake()
    {
        if (!kettle) kettle = GetComponent<KettleHeat>();
        if (!station) station = GetComponentInParent<StationController>();

        if (!pourMouth)
        {
            var t = new GameObject("PourMouth").transform;
            t.SetParent(transform, false);
            t.localPosition = new Vector3(0f, 0.12f, 0.12f);
            pourMouth = t;
        }

        _selfCols = ignoreSelf ? GetComponentsInChildren<Collider>(true) : null;
    }

    void Reset()
    {
        if (!kettle) kettle = GetComponent<KettleHeat>();
        if (!pourMouth)
        {
            var t = new GameObject("PourMouth").transform;
            t.SetParent(transform, false);
            t.localPosition = new Vector3(0f, 0.12f, 0.12f);
            pourMouth = t;
        }
    }

    void Update()
    {
        bool wantPourButton = !useRMBToPour || Input.GetMouseButton(1);

        // Optional tilt (disabled by default)
        float tilt = Vector3.Angle(transform.up, Vector3.up);
        bool enoughTilt = !requireTiltToPour || (tilt >= pourAngleDeg);

        // Ray basis from the spout
        Vector3 rayOrigin = pourMouth ? pourMouth.position : transform.position;
        Vector3 rayDir = pourMouth ? -pourMouth.up : -transform.up;

        // 1) Aim hit (ANYTHING)
        bool gotAim;
        RaycastHit aimHit;
        gotAim = CastFirstNonSelf(rayOrigin, rayDir, maxRayDistance, aimMask, useSphereAim, aimSphereRadius, out aimHit);

        // 2) Cup-only hit (actual pour)
        bool gotCup;
        RaycastHit cupHit;
        gotCup = CastFirstNonSelf(rayOrigin, rayDir, maxRayDistance, cupMask, useSphereAim, aimSphereRadius, out cupHit);

        CupContainer cupTarget = null;
        if (gotCup)
            cupTarget = cupHit.collider.GetComponentInParent<CupContainer>();

        // Heat gate
        bool heatOK = true;
        if (requireHeatToPour && station && station.ActiveRecipe != null && kettle != null)
        {
            float cur = kettle.tempC;
            float min = station.ActiveRecipe.targetTempMinC;
            float max = station.ActiveRecipe.targetTempMaxC;
            heatOK = (cur >= min && cur <= max);
        }

        // Debug draw:
        //  - cyan: actually pouring (RMB + tilt OK + cup hit + heat OK + water)
        //  - red : aiming at cup but heat wrong
        //  - yellow: aiming (any surface), not pouring
        if (drawDebugRay)
        {
            if (gotCup)
            {
                bool pouringNow = wantPourButton && enoughTilt && kettle != null && kettle.waterMl > 0f && heatOK;
                Debug.DrawLine(rayOrigin, cupHit.point, pouringNow ? pourColor : heatBadColor, 0f, false);
                Debug.DrawRay(cupHit.point, Vector3.up * 0.02f, pouringNow ? pourColor : heatBadColor, 0f, false);
            }
            else if (gotAim)
            {
                Debug.DrawLine(rayOrigin, aimHit.point, aimColor, 0f, false);
            }
            else
            {
                Debug.DrawRay(rayOrigin, rayDir * maxRayDistance, aimColor, 0f, false);
            }
        }

        // Pour only if: RMB (if used) + tilt (if required) + Cup hit + heat OK + has water
        if (!(wantPourButton && enoughTilt && gotCup && cupTarget != null && kettle != null && kettle.waterMl > 0f && heatOK))
            return;

        float t = requireTiltToPour ? Mathf.InverseLerp(pourAngleDeg, 90f, tilt) : 1f;
        float rate = Mathf.Lerp(0f, mlPerSecAtFullTilt, t);
        float ml = Mathf.Min(kettle.waterMl, rate * Time.deltaTime);
        if (ml <= 0f) return;

        kettle.waterMl -= ml;
        cupTarget.AddWater(ml, kettle.tempC);
    }

    // ---- helpers ----

    bool CastFirstNonSelf(
        Vector3 origin,
        Vector3 dir,
        float distance,
        LayerMask mask,
        bool sphere,
        float radius,
        out RaycastHit bestHit)
    {
        // Try a single cast first; if it hits self, fall back to All and pick the nearest non-self
        if (!sphere)
        {
            if (Physics.Raycast(origin, dir, out bestHit, distance, mask, QueryTriggerInteraction.Collide))
            {
                if (!IsSelf(bestHit.collider)) return true;
            }

            // Need to search all hits for the first non-self
            var hits = Physics.RaycastAll(origin, dir, distance, mask, QueryTriggerInteraction.Collide);
            return SelectNearestNonSelf(hits, out bestHit);
        }
        else
        {
            if (Physics.SphereCast(origin, radius, dir, out bestHit, distance, mask, QueryTriggerInteraction.Collide))
            {
                if (!IsSelf(bestHit.collider)) return true;
            }

            var hits = Physics.SphereCastAll(origin, radius, dir, distance, mask, QueryTriggerInteraction.Collide);
            return SelectNearestNonSelf(hits, out bestHit);
        }
    }

    bool SelectNearestNonSelf(RaycastHit[] hits, out RaycastHit best)
    {
        best = default;
        float bestDist = float.PositiveInfinity;
        for (int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            if (IsSelf(h.collider)) continue;
            if (h.distance < bestDist) { best = h; bestDist = h.distance; }
        }
        return bestDist < float.PositiveInfinity;
    }

    bool IsSelf(Collider col)
    {
        if (!ignoreSelf || col == null) return false;
        if (_selfCols == null || _selfCols.Length == 0) return false;

        // cheap reference compare against cached colliders
        for (int i = 0; i < _selfCols.Length; i++)
            if (_selfCols[i] == col) return true;

        // also ignore colliders that belong to our transform hierarchy
        return col.transform.IsChildOf(transform);
    }
}
