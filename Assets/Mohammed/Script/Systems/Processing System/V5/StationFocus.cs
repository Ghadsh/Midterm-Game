// StationFocus.cs
using UnityEngine;

public class StationFocus : MonoBehaviour
{
    [Header("Refs")]
    public StationFocusCamera focusCam;          // assign your Main Camera with StationFocusCamera
    public Transform cameraAnchor;               // empty child at the ideal eye point
    public Vector3 cameraLocalOffset = new Vector3(0f, 0.85f, -0.55f);
    public Vector3 cameraLocalEuler = new Vector3(5f, 0f, 0f);

    [Header("Integration")]
    public StationController stationController;  // optional; used for saving yaw/pitch

    public GameObject stationPanelUI;
    public StationUIController stationUI;

    [SerializeField] StationFocusUIHider uiHider;

    bool _inRange;
    bool _focused;




    void Reset()
    {
        // try auto-find
        if (!focusCam) focusCam = Camera.main ? Camera.main.GetComponent<StationFocusCamera>() : null;
        if (!cameraAnchor)
        {
            var go = new GameObject("CameraAnchor");
            go.transform.SetParent(transform, false);
            cameraAnchor = go.transform;
        }
        if (!stationController) stationController = GetComponent<StationController>();
    }

    void Update()
    {
        // Simple proximity/interaction: you can replace this with your own interactor
        // Here we assume player presses F while "in range" (set externally), or you can remove _inRange and always allow.
        if (!_focused && _inRange && Input.GetKeyDown(KeyCode.F)) EnterFocus();
        if (_focused && (Input.GetKeyDown(KeyCode.Escape))) ExitFocus();
    }

    public void SetInRange(bool on) => _inRange = on; // call from your player trigger detector

    public void EnterFocus()
    {
        if (_focused || focusCam == null || cameraAnchor == null) return;
        uiHider?.HideExtras();
        // Pull saved yaw/pitch if available
        if (stationController && stationController.TryGetSavedCamera(out float yaw, out float pitch))
            focusCam.SetAngles(yaw, pitch);

        focusCam.EnterFocus(cameraAnchor, cameraLocalOffset, Quaternion.Euler(cameraLocalEuler));

        if (!stationController.RequireStir && stationController.spoon)
            stationController.spoon.enabled = false;

        UIState.PushBlock(); // block world inputs while focused
        _focused = true;

        if (stationUI)
        {
            var bridge = FindObjectOfType<PlayerInventoryBridgeImpl>();
            stationUI.Bind(stationController.inventory, stationController, bridge);
        }
        if (stationPanelUI) stationPanelUI.SetActive(true);

        var resultPanel = FindObjectOfType<ResultPanelController>(true);
        if (resultPanel) resultPanel.Bind(stationController.inventory, stationController, FindObjectOfType<PlayerInventoryBridgeImpl>());

        // Checklist panel (new)
        var checklist = FindObjectOfType<MiniGameChecklistPanel>(true);
        if (checklist) checklist.Bind(stationController);  // no SetActive(true) here

    }

    public void ExitFocus()
    {
        if (stationPanelUI) stationPanelUI.SetActive(false);
        if (!_focused || focusCam == null) return;

        var checklist = FindObjectOfType<MiniGameChecklistPanel>(true);
        if (checklist) checklist.gameObject.SetActive(false);


        // Save yaw/pitch back to station
        if (stationController)
        {
            var (y, p) = focusCam.GetAngles();
            stationController.SetSavedCamera(y, p);
        }
        uiHider?.HideExtras();
        focusCam.ExitFocus();
        UIState.PopBlock(); // re-enable world inputs
        _focused = false;


        // optional: make sure station controller persists immediately
        stationController?.SaveState();
    }
}
