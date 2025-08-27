// StationUIController.cs  (add/confirm these usings)
using UnityEngine;
using UnityEngine.Events;

public class StationUIController : MonoBehaviour
{
    [Header("Refs (bound at runtime or via Inspector)")]
    public StationInventory stationInventory;
    public StationController stationController;

    // Bridge is an interface, Unity won't serialize it; we'll auto-find if Bind() doesn't pass it.
    private IPlayerInventoryBridge playerBridge;

    [Header("Preview UI (assign in Inspector)")]
    [SerializeField] private UnityEngine.UI.Image previewIconTarget;
    [SerializeField] private TMPro.TMP_Text previewNameTarget;

    [Header("Slot UI (assign in Inspector)")]
    [SerializeField] private UnityEngine.UI.Image baseIcon;       // StationPanel/Left/BaseRow/BaseButton/Icon
    [SerializeField] private TMPro.TMP_Text baseCount;            // StationPanel/Left/BaseRow/BaseButton/Count
    [SerializeField] private UnityEngine.UI.Image[] modIcons;     // StationPanel/Left/ModsRow/ModsGrid/ModButton_X/Icon
    [SerializeField] private TMPro.TMP_Text[] modCounts;          // .../ModButton_X/Count

    // Result UI
    [SerializeField] private UnityEngine.UI.Image resultIcon;
    [SerializeField] private TMPro.TMP_Text resultCount;
    [SerializeField] private UnityEngine.UI.Button collectButton;

    [SerializeField] private GameObject stationPanelRoot;   // drag the StationPanel root (the panel you want hidden)
    [SerializeField] private bool hidePanelOnBegin = true;

    [Header("Optional events (you can ignore)")]
    public UnityEvent<Sprite> onPreviewIcon;
    public UnityEvent<string> onPreviewName;
    [SerializeField] private UnityEngine.UI.Button beginButton;
    [SerializeField] private UnityEngine.UI.Button retrieveButton;



    bool canBegin = false;
    void Awake()
    {
        // Auto-find bridge if not provided via Bind()
        if (playerBridge == null)
            playerBridge = FindObjectOfType<PlayerInventoryBridgeImpl>();

        if (stationInventory)
            stationInventory.Changed += OnInventoryChanged;
    }

    bool IsLocked()
    {
        return stationController &&
               (stationController.CurrentStage == StationStage.Minigame ||
                stationController.CurrentStage == StationStage.Completed);
    }

    void OnDestroy()
    {
        if (stationInventory)
            stationInventory.Changed -= OnInventoryChanged;
    }

    public void Bind(StationInventory inv, StationController ctrl, IPlayerInventoryBridge bridge)
    {
        if (stationInventory) stationInventory.Changed -= OnInventoryChanged;

        stationInventory = inv;
        stationController = ctrl;
        if (bridge != null) playerBridge = bridge;

        if (stationInventory) stationInventory.Changed += OnInventoryChanged;

        var resultPanel = FindObjectOfType<ResultPanelController>(true);
        if (resultPanel)
        {
             bridge = FindObjectOfType<PlayerInventoryBridgeImpl>();
            resultPanel.Bind(stationController.inventory, stationController, bridge);
            // No need to force show; ResultPanelController shows itself only when stage=Completed & result exists
        }

        RefreshAll();
    }

    void OnInventoryChanged()
    {
        RefreshAll();
    }

    // --- Called by StationSlotButton drops ---
    public bool OnDropFromPlayer(ItemDefinition def, int count, bool dropToBase)
    {
        if (def == null || count <= 0 || stationInventory == null || playerBridge == null) return false;

        bool accepted = dropToBase
            ? stationInventory.TryAddBase(def, count)
            : stationInventory.TryAddMod(def, count);

        if (!accepted) return false;

        // Move ownership from player to station (not consume yet)
        if (!playerBridge.TakeFromPlayer(def, count))
        {
            // Roll back if player didn't have enough
            if (dropToBase) stationInventory.ClearBase();
            else stationInventory.RemoveLastMod();
            return false;
        }

        RefreshAll();
        return true;
    }

    // --- Called by Base/Mod button clicks (return to player) ---
    public void OnReturnBase()
    {
        if (IsLocked()) return;
        if (playerBridge == null || stationInventory == null) return;
        var s = stationInventory.baseSlot;
        if (s != null && s.item != null && s.count > 0)
        {
            playerBridge.GiveToPlayer(s.item, s.count);
            stationInventory.ClearBase();
        }


        RefreshAll();
    }

    public void OnReturnMod(int index)
    {
       if (IsLocked()) return;
        if (playerBridge == null || stationInventory == null) return;
        if (index < 0 || index >= stationInventory.modSlots.Count) return;

        var s = stationInventory.modSlots[index];
        if (s != null && s.item != null && s.count > 0)
        {
            playerBridge.GiveToPlayer(s.item, s.count);
            stationInventory.RemoveModAt(index);
        }



        RefreshAll();
    }

    public void OnRetrieveAll()
    {
        if (stationInventory == null || playerBridge == null) return;
        stationInventory.ReturnAllToPlayer(playerBridge);
        RefreshAll();
    }

    public void OnBegin()
    {
        if (!stationController) return;
        bool ok = stationController.TryBeginProcess();
        Debug.Log("[StationUI] Begin pressed → " + (ok ? "Minigame started" : "blocked"));

        if (ok)
        {
            RefreshButtons();
            if (hidePanelOnBegin && stationPanelRoot) stationPanelRoot.SetActive(false);
        }
    }


    // -------- UI refreshers --------
    void RefreshAll()
    {
        RefreshSlots();
        RefreshPreview();
        RefreshResult();
        RefreshButtons();
    }

    void RefreshSlots()
    {
        // Base
        var b = stationInventory != null ? stationInventory.baseSlot : null;
        if (baseIcon)
        {
            baseIcon.sprite = (b != null && b.item) ? b.item.icon : null;
            var c = baseIcon.color; c.a = baseIcon.sprite ? 1f : 0f; baseIcon.color = c;
        }
        if (baseCount)
            baseCount.text = (b != null && b.item && b.count > 1) ? b.count.ToString() : "";

        // Mods (index-aligned to your 3 buttons)
        if (modIcons != null)
        {
            for (int i = 0; i < modIcons.Length; i++)
            {
                Sprite s = null; int ct = 0;
                if (stationInventory != null && i < stationInventory.modSlots.Count)
                {
                    var m = stationInventory.modSlots[i];
                    if (m != null && m.item != null) { s = m.item.icon; ct = m.count; }
                }

                if (modIcons[i])
                {
                    modIcons[i].sprite = s;
                    var col = modIcons[i].color; col.a = s ? 1f : 0f; modIcons[i].color = col;
                }
                if (modCounts != null && i < modCounts.Length && modCounts[i])
                    modCounts[i].text = (ct > 1) ? ct.ToString() : "";
            }
        }
    }

    void RefreshPreview()
    {
        if (!stationController)
        {
            if (previewIconTarget) { var c = previewIconTarget.color; c.a = 0f; previewIconTarget.color = c; }
            if (previewNameTarget) previewNameTarget.text = "Add a Base item";
            return;
        }

        var (icon, name) = stationController.GetPreviewForCurrentContents();
        if (previewIconTarget)
        {
            previewIconTarget.sprite = icon;
            var c = previewIconTarget.color; c.a = icon ? 1f : 0f; previewIconTarget.color = c;
        }
        if (previewNameTarget) previewNameTarget.text = string.IsNullOrEmpty(name) ? "Add a Base item" : name;
    }

    void RefreshResult()
    {
        if (resultIcon == null && resultCount == null && collectButton == null) return;

        var s = stationInventory ? stationInventory.resultSlot : null;
        bool has = s != null && s.item != null && s.count > 0;

        if (resultIcon)
        {
            resultIcon.sprite = has ? s.item.icon : null;
            var c = resultIcon.color; c.a = has ? 1f : 0f; resultIcon.color = c;
        }
        if (resultCount) resultCount.text = has && s.count > 1 ? s.count.ToString() : "";
        if (collectButton) collectButton.interactable = has;
    }

    void RefreshButtons()
    {
        if (!beginButton) return;
        // Enable Begin only when recipe exists

        if (!beginButton && !retrieveButton) return;
        bool inMini = stationController && stationController.CurrentStage == StationStage.Minigame;
        bool done = stationController && stationController.CurrentStage == StationStage.Completed;

        if (beginButton)
        {
            // Begin only when there is a valid recipe AND not in minigame/done
             canBegin = false;
            if (!inMini && !done && stationInventory && stationInventory.baseSlot?.item)
            {
                var resolver = stationController ? stationController.recipeResolver : null;
                canBegin = resolver && resolver.FindMatch(stationInventory.baseSlot.item, stationInventory.modSlots) != null;
            }
            beginButton.interactable = canBegin;
        }

        if (retrieveButton)
        {
            // Retrieve disabled in Minigame/Completed
            bool hasItems = stationInventory &&
                            ((stationInventory.baseSlot != null && stationInventory.baseSlot.item) ||
                             stationInventory.modSlots.Exists(s => s != null && s.item));
            retrieveButton.interactable = hasItems && !inMini && !done;
        }

         canBegin = false;
        if (stationController && stationInventory && stationInventory.baseSlot?.item)
        {
            var resolver = stationController.recipeResolver;
            canBegin = resolver && resolver.FindMatch(stationInventory.baseSlot.item, stationInventory.modSlots) != null;
        }
        beginButton.interactable = canBegin;
    }

    // Optional: expose a "can accept" check for highlights
    public bool CanAcceptDrop(bool isBase, int modIndex, ItemDefinition def, int count)
    {
        if (stationInventory == null || def == null || count <= 0) return false;
        if (isBase)
            return stationInventory.GetAddableToBase(def, count) > 0;
        else
            return stationInventory.GetAddableToModAt(modIndex, def, count) > 0;
    }

    // NEW: slot-targeted drop (used by StationSlotButton)
    public bool OnDropToSlot(bool isBase, int modIndex, ItemDefinition def, int count)
    {
        if (def == null || count <= 0 || stationInventory == null || playerBridge == null) return false;

      // if (IsLocked()) return false; // dont remove the commit it will lock the ui and you will try to find fix it (time waste counter = 4.5 Hours)

        int accepted = isBase
            ? stationInventory.AddToBase(def, count)                      // clamps internally
            : stationInventory.AddToModAt(modIndex, def, count);          // clamps internally

        if (accepted <= 0) return false;

        // Remove exactly what was accepted from the player
        if (!playerBridge.TakeFromPlayer(def, accepted))
        {
            // Roll back if player didn't actually have the items
            if (isBase) stationInventory.SubtractFromBase(accepted);
            else stationInventory.SubtractFromModAt(modIndex, accepted);
            return false;
        }


        RefreshAll();
        return true;
    }
    public void OnCollectResult()
    {
        if (stationInventory == null || playerBridge == null) return;
        var s = stationInventory.resultSlot;
        if (s == null || s.item == null || s.count <= 0) return;

        // Give to player (assumes space; extend bridge if you want partial handling)
        playerBridge.GiveToPlayer(s.item, s.count);
        stationInventory.TakeResultAll();

        RefreshAll();
    }
}
