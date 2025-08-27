using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class ShopSystem : MonoBehaviour
{
    public int CoffeePrice;
    public int TeaPrice;
    public int MintTeaPrice;
    public int mintTeaInventory = 0;
    public TMP_Text mintTeaInventoryText;
    public TMP_Text CoffeePriceTMP_Text;
    public TMP_Text TeaPriceTMP_Text;
    public TMP_Text MintTeaPriceTMP_Text;
    public EconomySystem economy;

    void OnEnable()
    {
        DayNightCycle.OnNewDay += UpdatePrices;
    }

    void OnDisable()
    {
        DayNightCycle.OnNewDay -= UpdatePrices;
    }

    void Start()
    {

        CoffeePrice = PlayerPrefs.GetInt("CoffeePrice", Random.Range(250, 351));
        TeaPrice = PlayerPrefs.GetInt("TeaPrice", Random.Range(150, 251));
        MintTeaPrice = PlayerPrefs.GetInt("MintTeaPrice", Random.Range(155, 256));
        UpdateUI();
    }

    void UpdatePrices()
    {
        CoffeePrice = Random.Range(250, 351);
        TeaPrice = Random.Range(150, 251);
        MintTeaPrice = Random.Range(155, 256);

        PlayerPrefs.SetInt("CoffeePrice", CoffeePrice);
        PlayerPrefs.SetInt("TeaPrice", TeaPrice);
        PlayerPrefs.SetInt("MintTeaPrice", MintTeaPrice);
        PlayerPrefs.Save();

        UpdateUI();
    }


    void UpdateUI()
    {
        CoffeePriceTMP_Text.text = CoffeePrice.ToString();
        TeaPriceTMP_Text.text = TeaPrice.ToString();
        MintTeaPriceTMP_Text.text = MintTeaPrice.ToString();
        if (mintTeaInventoryText != null)
            mintTeaInventoryText.text = "Mint Tea Inventory: " + mintTeaInventory;


    }


    public void BuyCoffee()
    {
        if (economy.currentCash >= CoffeePrice)
        {
            economy.AddExpense(CoffeePrice);
            Debug.Log("Bought Coffee");
        }
    }

    public void SellCoffee()
    {
        economy.AddIncome(CoffeePrice);
        Debug.Log("Sold Coffee");
    }

    public void BuyTea()
    {
        if (economy.currentCash >= TeaPrice)
        {
            economy.AddExpense(TeaPrice);
            Debug.Log("Bought Tea");
        }
    }

    public void SellTea()
    {
        economy.AddIncome(TeaPrice);
        Debug.Log("Sold Tea");
    }

    public void BuyMintTea()
    {
        mintTeaInventory++;
        Debug.Log($"Mint Tea prepared! Inventory: {mintTeaInventory}");
        UpdateUI();
    }

    public void SellMintTea()
    {
        if (mintTeaInventory > 0)
        {
            economy.AddIncome(MintTeaPrice);
            mintTeaInventory--;
            Debug.Log($"Mint Tea sold. Remaining: {mintTeaInventory}");
        }
        else
        {
            Debug.Log("No Mint left to sell!");
        }
    }


    // popup
    public GameObject pricePopupPanel;
    public TMP_Text popupText;

    void ShowPricePopup()
    {
        if (pricePopupPanel != null && popupText != null)
        {
            popupText.text =
                $"☀ New Day Prices:\n" +
                $"- Coffee: {CoffeePrice}\n" +
                $"- Tea: {TeaPrice}\n" +
                $"- Mint Tea: {MintTeaPrice}";

            pricePopupPanel.SetActive(true);
            Invoke(nameof(HidePricePopup), 4f); // Hide after 4 seconds
        }
    }

    void HidePricePopup()
    {
        pricePopupPanel.SetActive(false);
    }
}


