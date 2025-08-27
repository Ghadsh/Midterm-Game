using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoanPanelUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject loanInfoGroup;   // visible while loan > 0
    [SerializeField] private GameObject congratsGroup;   // visible when loan == 0
    [SerializeField] private TMP_Text loanRemainingText;
    [SerializeField] private Image loanProgressFill; // Image (Filled, Horizontal)
    [SerializeField] private Button buyoutButton;     // optional "Pay Off Now"

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip paidOffClip;     // plays when loan hits 0
    [SerializeField] private AudioClip cantAffordClip;  // plays on failed buyout

    [Header("Economy (assign ONE)")]
    [SerializeField] private EconomyService economyService; // legacy/adapter
    [SerializeField] private EconomySystem economySystem;  // your new system

    [Header("Loan")]
    [SerializeField] private int loanRemaining = 100_000;
    [SerializeField] private int loanRepaymentPerDay = 10_000;
    [Tooltip("If 0, will init from loanRemaining at runtime.")]
    [SerializeField] private int loanInitial = 0;

    [Header("Behavior")]
    [SerializeField] private bool autoRepayOnNewDay = true;

    private bool paidOffTriggered = false;

    private void Awake()
    {
        if (loanInitial <= 0) loanInitial = Mathf.Max(1, loanRemaining);
        SetCongratsVisible(false);
        UpdateUI();
    }

    private void OnEnable()
    {
        if (autoRepayOnNewDay)
            DayNightCycle.OnNewDay += HandleNewDay;

        if (buyoutButton) buyoutButton.onClick.AddListener(OnBuyoutClicked);
        UpdateUI();
    }

    private void OnDisable()
    {
        if (autoRepayOnNewDay)
            DayNightCycle.OnNewDay -= HandleNewDay;

        if (buyoutButton) buyoutButton.onClick.RemoveListener(OnBuyoutClicked);
    }

    private void HandleNewDay()
    {
        TryDailyRepayment();
    }

    /// <summary>Pays the daily installment if possible.</summary>
    public void TryDailyRepayment()
    {
        if (paidOffTriggered || loanRemaining <= 0) return;

        int toPay = Mathf.Min(loanRepaymentPerDay, loanRemaining);
        if (toPay <= 0) { CheckPaidOff(); return; }

        if (TrySpend(toPay))
        {
            loanRemaining -= toPay;
        }

        UpdateUI();
        CheckPaidOff();
    }

    // --- BUYOUT: pay everything now ---
    private void OnBuyoutClicked() => BuyOutLoan();

    public bool BuyOutLoan()
    {
        if (paidOffTriggered || loanRemaining <= 0) return false;

        int toPay = loanRemaining;
        if (!TrySpend(toPay))
        {
            if (cantAffordClip && sfxSource) sfxSource.PlayOneShot(cantAffordClip);
            StartCoroutine(FlashCannotAfford());
            return false;
        }

        loanRemaining = 0;
        UpdateUI();
        CheckPaidOff();
        return true;
    }

    // --- UI updates ---
    private void UpdateUI()
    {
        if (loanRemainingText)
            loanRemainingText.text = $"Loan Remaining: ${loanRemaining:N0}";

        if (loanProgressFill)
        {
            float pct = 1f - ((float)loanRemaining / Mathf.Max(1, loanInitial));
            loanProgressFill.fillAmount = Mathf.Clamp01(pct);
        }

        if (buyoutButton)
        {
            bool canInteract = loanRemaining > 0 && CanAfford(loanRemaining);
            buyoutButton.interactable = canInteract;

            var label = buyoutButton.GetComponentInChildren<TMP_Text>(true);
            if (label) label.text = $"Pay Off Now (${loanRemaining:N0})";
        }
    }

    private void CheckPaidOff()
    {
        if (!paidOffTriggered && loanRemaining <= 0)
        {
            paidOffTriggered = true;
            SetCongratsVisible(true);
            if (sfxSource)
            {
                if (paidOffClip) sfxSource.PlayOneShot(paidOffClip);
                else sfxSource.Play();
            }
        }
    }

    private void SetCongratsVisible(bool showCongrats)
    {
        if (loanInfoGroup) loanInfoGroup.SetActive(!showCongrats);
        if (congratsGroup) congratsGroup.SetActive(showCongrats);
    }

    // --- Economy bridge ---
    private bool CanAfford(int amount)
    {
        if (economyService) return economyService.Coins >= amount;
        if (economySystem) return economySystem.currentCash + 0.0001f >= amount;
        return false;
    }

    private bool TrySpend(int amount)
    {
        if (amount <= 0) return true;

        if (economyService)
            return economyService.TrySpend(amount);

        if (economySystem)
        {
            if (economySystem.currentCash + 0.0001f < amount) return false;
            economySystem.AddExpense(amount); // keeps its UI/daily totals in sync
            return true;
        }

        Debug.LogError("[LoanPanelUI] No economy assigned.", this);
        return false;
    }

    // --- Utilities ---
    public void SetEconomy(EconomyService service) { economyService = service; economySystem = null; UpdateUI(); }
    public void SetEconomy(EconomySystem system) { economySystem = system; economyService = null; UpdateUI(); }

    public void ResetLoan(int newTotal, int perDay)
    {
        loanInitial = Mathf.Max(1, newTotal);
        loanRemaining = Mathf.Max(0, newTotal);
        loanRepaymentPerDay = Mathf.Max(0, perDay);
        paidOffTriggered = false;
        SetCongratsVisible(false);
        UpdateUI();
    }

    private IEnumerator FlashCannotAfford()
    {
        if (!loanRemainingText) yield break;
        Color orig = loanRemainingText.color;
        loanRemainingText.color = new Color(1f, 0.35f, 0.35f);
        yield return new WaitForSeconds(0.15f);
        loanRemainingText.color = orig;
    }
}
