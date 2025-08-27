using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject shopPanel;

    public void ShowInventory()
    {
        HideAll();
        inventoryPanel.SetActive(true);
    }

    public void ShowShop()
    {
        HideAll();
        shopPanel.SetActive(true);
    }

    public void HideAll()
    {
        inventoryPanel.SetActive(false);
        shopPanel.SetActive(false);
    }
}
