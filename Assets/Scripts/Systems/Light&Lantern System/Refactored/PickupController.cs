using UnityEngine;

public class PickupController : MonoBehaviour
{
    [Header("Configs (SO)")]
    [SerializeField] private InteractionConfig interaction;
    [SerializeField] private InputConfig inputCfg;

    [Header("Setup")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform holdPoint;

    private Rigidbody heldRB;
    private LightBlob heldBlob;

    private void Awake()
    {
        if (!cam) cam = Camera.main;

        if (!holdPoint)
        {
            var go = new GameObject("HoldPoint_Auto");
            holdPoint = go.transform;
            holdPoint.SetParent(cam.transform, false);

            if (interaction)
            {
                holdPoint.localPosition = interaction.holdLocalPosition;
                holdPoint.localEulerAngles = interaction.holdLocalEuler;
            }
            else
            {
                holdPoint.localPosition = new Vector3(0f, -0.05f, 0.35f);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(inputCfg ? inputCfg.pickKey : KeyCode.E))
        {
            if (heldRB == null) TryPick();
            else Drop();
        }

        if (heldBlob != null)
        {
            if (Input.GetKey(inputCfg ? inputCfg.pourKeyHold : KeyCode.Mouse0))
            {
                if (TryFindJar(out LightJar jar))
                    jar.PourFrom(heldBlob, Time.deltaTime);
            }

            if (Input.GetKeyDown(inputCfg ? inputCfg.splitKey : KeyCode.F))
            {
                var small = heldBlob.Split(0.5f);
                if (small)
                {
                    var rb = small.GetComponent<Rigidbody>();
                    if (rb) rb.AddForce(cam.transform.forward * 2f, ForceMode.VelocityChange);
                }
            }
        }
    }

    private void TryPick()
    {
        float dist = interaction ? interaction.pickDistance : 4f;
        LayerMask mask = interaction ? interaction.interactMask : ~0;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, dist, mask, QueryTriggerInteraction.Ignore))
        {
            var rb = hit.rigidbody;
            if (rb)
            {
                heldRB = rb;
                heldRB.useGravity = false;
                heldRB.linearVelocity = Vector3.zero;
                heldRB.angularVelocity = Vector3.zero;
                heldRB.transform.SetParent(holdPoint, false);
                heldRB.transform.localPosition = Vector3.zero;

                heldBlob = heldRB.GetComponent<LightBlob>(); // can be null (non-blob object)
            }
        }
    }

    private void Drop()
    {
        if (!heldRB) return;
        heldRB.transform.SetParent(null, true);
        heldRB.useGravity = true;
        heldRB = null;
        heldBlob = null;
    }

    private bool TryFindJar(out LightJar jar)
    {
        jar = null;
        float dist = interaction ? interaction.pickDistance : 4f;
        LayerMask mask = interaction ? interaction.interactMask : ~0;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, dist, mask, QueryTriggerInteraction.Ignore))
            jar = hit.collider.GetComponentInParent<LightJar>();

        return jar != null;
    }
}
