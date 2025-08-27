// HotbarSlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class HotbarSlotUI : MonoBehaviour
{
    public int Index;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject selectedFx;

    void Awake()
    {
        // Auto-create selectedFx if missing
        if (!selectedFx)
        {
            var go = new GameObject("SelectedFx", typeof(Image));
            go.transform.SetParent(transform, false);
            var img = go.GetComponent<Image>();
            img.color = new Color(1f, 0.9f, 0.2f, 0.6f); // tea-gold glow
            img.raycastTarget = false;
            var rt = img.rectTransform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            go.SetActive(false);
            selectedFx = go;
            go.transform.SetAsFirstSibling();
        }
    }

    public void Set(InventoryService.Stack stack)
    {
        if (stack != null && stack.item != null && stack.count > 0)
        {
            icon.enabled = true;
            icon.sprite = stack.item.icon;
            countText.enabled = true;
            countText.text = stack.count > 1 ? stack.count.ToString() : "";
        }
        else
        {
            icon.enabled = false;
            countText.enabled = false;
        }
    }

    public void SetSelected(bool on)
    {
        if (selectedFx) selectedFx.SetActive(on);
    }
}
