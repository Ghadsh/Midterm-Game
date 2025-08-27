// ResultPanelController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultPanelController : MonoBehaviour
{
    [Header("UI (assign in Inspector)")]
    public GameObject resultGroup;     // the container you created (keep inactive by default)
    public Image icon;                 // ResultGroup/Icon  (Raycast Target OFF)
    public TMP_Text countText;         // ResultGroup/Count
    public Button collectButton;       // ResultGroup/CollectButton

    [Header("Station refs (bound at runtime)")]
    public StationInventory stationInventory;
    public StationController stationController;

    // Player bridge (interface) – auto-found if Bind doesn't pass one
    IPlayerInventoryBridge playerBridge;

    void Awake()
    {
        if (collectButton) collectButton.onClick.AddListener(OnClickCollect);
        if (resultGroup) resultGroup.SetActive(false);
        if (playerBridge != null) playerBridge = FindObjectOfType<PlayerInventoryBridgeImpl>();
    }

    void OnEnable()
    {
        if (stationInventory) stationInventory.Changed += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        if (stationInventory) stationInventory.Changed -= Refresh;
    }

    // Call this from StationFocus when you enter a station
    public void Bind(StationInventory inv, StationController ctrl, IPlayerInventoryBridge bridge = null)
    {
        if (stationInventory) stationInventory.Changed -= Refresh;

        stationInventory = inv;
        stationController = ctrl;
        if (bridge != null) playerBridge = bridge;

        if (stationInventory) stationInventory.Changed += Refresh;
        Refresh();
    }

    void Refresh()
    {
        bool completed = stationController && stationController.CurrentStage == StationStage.Completed;
        var s = stationInventory != null ? stationInventory.resultSlot : null;
        bool has = completed && s != null && s.item != null && s.count > 0;

        if (resultGroup) resultGroup.SetActive(has);

        if (!has)
            return;

        if (icon)
        {
            icon.sprite = s.item.icon;
            var c = icon.color; c.a = icon.sprite ? 1f : 0f; icon.color = c;
            // ensure it doesn't block clicks: set Raycast Target OFF in Inspector
        }
        if (countText) countText.text = (s.count > 1) ? s.count.ToString() : "";

        if (collectButton) collectButton.interactable = true;
    }

    void OnClickCollect()
    {
        if (stationInventory == null || playerBridge == null) return;

        var s = stationInventory.resultSlot;
        if (s == null || s.item == null || s.count <= 0) return;

        // Move to player, clear result
        playerBridge.GiveToPlayer(s.item, s.count);
        stationInventory.TakeResultAll();

        // Reset station to Idle (no changes to Base/Mod UI)
        stationController?.ResetAfterCollect();

        Refresh(); // hides the panel
    }
}
