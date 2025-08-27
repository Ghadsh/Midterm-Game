using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EconomySystem : MonoBehaviour
{
    public float currentCash = 100f;
    private float dailyIncome = 0f;
    private float dailyExpenses = 0f;

    public TMP_Text cashText;
    public TMP_Text incomeText;
    public TMP_Text expenseText;
    public TMP_Text profitText;

    void OnEnable()
    {
        DayNightCycle.OnNewDay += EndOfDay;
    }

    void OnDisable()
    {
        DayNightCycle.OnNewDay -= EndOfDay;
    }

    public void AddIncome(float amount)
    {
        dailyIncome += amount;
        currentCash += amount;
        UpdateUI();
    }

    public void AddExpense(float amount)
    {
        dailyExpenses += amount;
        currentCash -= amount;
        UpdateUI();
    }

    public void EndOfDay()
    {
        float profit = dailyIncome - dailyExpenses;
        profitText.text = $"Profit/Loss: {profit:F2}";

        dailyIncome = 0f;
        dailyExpenses = 0f;

        UpdateUI(); //refresh cash
    }

    private void UpdateUI()
    {
        cashText.text = $"Cash: {currentCash:F2}";
        incomeText.text = $"Income Today: {dailyIncome:F2}";
        expenseText.text = $"Expenses Today: {dailyExpenses:F2}";
    }

    void Start()
    {
        UpdateUI();
    }

}