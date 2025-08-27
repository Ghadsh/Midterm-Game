using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CupIntake : MonoBehaviour
{
    public CupContainer cup;

    [Header("Assist")]
    public bool assistMagnet = true;
    public float magnetSnapSpeed = 12f;

    Collider _col;
    bool _warnedMissingCup;

    void Reset()
    {
        // Auto-wire when you add the component in the editor
        if (!cup) cup = GetComponentInParent<CupContainer>();
        _col = GetComponent<Collider>();
        if (_col) _col.isTrigger = true;
    }

    void Awake()
    {
        // Auto-wire at runtime too (covers prefab hierarchy changes)
        if (!cup) cup = GetComponentInParent<CupContainer>();
        _col = GetComponent<Collider>();
        if (_col && !_col.isTrigger) _col.isTrigger = true;
    }

    void OnTriggerStay(Collider other)
    {
        // Bail fast if not assisting
        if (!assistMagnet) return;

        // No cup assigned? Guard and warn once.
        if (!cup)
        {
            if (!_warnedMissingCup)
            {
                Debug.LogError("[CupIntake] Missing 'cup' reference on " + name + ". "
                    + "Assign the CupContainer in the Inspector or make sure this object is a child of the Cup.");
                _warnedMissingCup = true;
            }
            return;
        }

        var ing = other.GetComponentInParent<CupIngestible>();
        if (!ing) return; // ignore non-ingestibles

        // Gentle magnet toward cup center
        Vector3 target = cup.transform.position + Vector3.up * 0.05f;
        var rb = ing.GetComponent<Rigidbody>();
        if (rb && !rb.isKinematic)
            rb.linearVelocity = (target - ing.transform.position) * magnetSnapSpeed;
        else
            ing.transform.position = Vector3.Lerp(ing.transform.position, target, Time.deltaTime * magnetSnapSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        var ing = other.GetComponentInParent<CupIngestible>();
        if (!ing || !cup) return;

        var station = GetComponentInParent<StationController>();
        if (station && station.CurrentStage != StationStage.Minigame) return;

        // Respect spawn grace: don�t consume instantly
        if (!ing.CanBeConsumed()) return;

        if (ing.kind == CupIngestible.Kind.Leaves)
        {
            if (cup.leavesUnits >= 1f) return;
            cup.AddLeaves(1f);
            Destroy(ing.gameObject);
            station?.OnCupContentsChanged();
            return;
        }

        if (ing.kind == CupIngestible.Kind.Mod)
        {
            int req = station ? station.RequiredCountFor(ing.itemDef) : 0;
            if (req <= 0) return;

            cup.mods.TryGetValue(ing.itemDef, out int have);
            int lack = Mathf.Max(0, req - have);
            if (lack <= 0) return;

            int toConsume = Mathf.Min(lack, Mathf.Max(1, ing.count));
            cup.AddMod(ing.itemDef, toConsume);

            int leftover = ing.count - toConsume;
            if (leftover <= 0) Destroy(ing.gameObject);
            else ing.count = leftover;

            station?.OnCupContentsChanged();
        }
    }
}
