using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class SlotUI : MonoBehaviour
{
    [HideInInspector] public int Index;
    [HideInInspector] public InventoryUI RootUI;

    [SerializeField] private Image iconImage;           // assign child "Icon"
    [SerializeField] private TextMeshProUGUI countText; // assign child "Count"
    [SerializeField] private GameObject selectedFx;     // auto-created if null

    Button _btn;

    void Awake()
    {
        // ensure the root image receives raycasts
        var img = GetComponent<Image>();
        img.raycastTarget = true;

        // click selection routed through Button (reliable)
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(() => {
            if (RootUI != null) RootUI.OnSlotClicked(Index);
        });

        // auto-create a simple highlight if not assigned
        if (!selectedFx)
        {
            var go = new GameObject("Highlight", typeof(Image));
            go.transform.SetParent(transform, false);
            var hImg = go.GetComponent<Image>();
            hImg.color = new Color(1f, 1f, 0f, 0.35f);   // semi-transparent yellow
            hImg.raycastTarget = false;                 // don't block clicks
            var rt = hImg.rectTransform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            go.transform.SetAsFirstSibling();           // draw behind icon/count
            go.SetActive(false);
            selectedFx = go;
        }
    }

    public void Set(InventoryService.Stack stack)
    {
        if (stack != null && stack.item != null && stack.count > 0)
        {
            iconImage.enabled = true;
            iconImage.sprite = stack.item.icon;
            countText.enabled = true;
            countText.text = stack.count > 1 ? stack.count.ToString() : "";
        }
        else
        {
            iconImage.enabled = false;
            countText.enabled = false;
        }
    }

    public void SetSelected(bool on)
    {
        if (selectedFx) selectedFx.SetActive(on);
    }
}
