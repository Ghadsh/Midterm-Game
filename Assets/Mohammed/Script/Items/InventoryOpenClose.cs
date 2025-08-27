using UnityEngine;

public class InventoryOpenClose : MonoBehaviour
{
    public GameObject inventoryPanel;   // root of your inventory UI

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool show = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(show);
            if (show) UIState.PushBlock(); else UIState.PopBlock();
        }
    }
}
