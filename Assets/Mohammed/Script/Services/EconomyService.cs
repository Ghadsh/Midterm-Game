using UnityEngine;

// Keep this class name so existing code compiles.
// If your project uses IEconomy, this implements it for full compatibility.
public class EconomyService : MonoBehaviour, IEconomy
{
    [Header("Link to the new EconomySystem")]
    [SerializeField] private EconomySystem economySystem;
    [Tooltip("If true, will auto-find an EconomySystem in the scene on Awake/Reset.")]
    [SerializeField] private bool autoFindEconomySystem = true;

    // Expose Coins as int for legacy callers; maps to EconomySystem.currentCash (float).
    public int Coins => economySystem ? Mathf.RoundToInt(economySystem.currentCash) : 0;

    private void Awake()
    {
        if (economySystem == null && autoFindEconomySystem)
        {
            economySystem = FindAnyObjectByType<EconomySystem>();
            if (economySystem == null)
            {
                Debug.LogError("[EconomyService] No EconomySystem found in scene. Assign one in the Inspector.", this);
            }
        }
    }

    private void Reset()
    {
        // Helps you auto-wire in the Inspector when adding the component
        if (economySystem == null)
        {
            economySystem = FindAnyObjectByType<EconomySystem>();
        }
    }

    /// <summary>
    /// Legacy method: tries to spend 'amount' coins.
    /// Maps to EconomySystem.AddExpense if there is enough cash.
    /// </summary>
    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (economySystem == null)
        {
            Debug.LogError("[EconomyService] EconomySystem reference is missing.", this);
            return false;
        }

        // Check funds against float cash
        if (economySystem.currentCash + 0.0001f < amount) return false;

        // Record as an expense so UI/daily totals stay correct
        economySystem.AddExpense(amount);
        return true;
    }

    /// <summary>
    /// Legacy method: adds 'amount' coins.
    /// Maps to EconomySystem.AddIncome so UI/daily totals update.
    /// </summary>
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        if (economySystem == null)
        {
            Debug.LogError("[EconomyService] EconomySystem reference is missing.", this);
            return;
        }
        economySystem.AddIncome(amount);
    }
}
