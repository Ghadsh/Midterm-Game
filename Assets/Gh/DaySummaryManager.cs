using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class DaySummaryManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject summaryPanel;
    public TMP_Text summaryText;
    public Button closeButton;           // <-- Close button only
    public GameObject winPanel;
    public CanvasGroup winCanvasGroup;
    public TMP_Text loanText;
    public CanvasGroup fadePanelGroup;

    [Header("Systems")]
    public DayNightCycle dayNightCycle;
    public EconomySystem economy;

    [Header("Loan")]
    public int loanRemaining = 100000;
    public int loanRepaymentPerDay = 10000;

    private void Start()
    {
        if (summaryPanel) summaryPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        UpdateLoanText();

        // Wire the Close button
        if (closeButton) closeButton.onClick.AddListener(CloseSummaryPanel);
    }

    private void OnEnable()
    {
        DayNightCycle.OnNewDay += ShowSummary;
    }

    private void OnDisable()
    {
        DayNightCycle.OnNewDay -= ShowSummary;
    }

    void ShowSummary()
    {
        // Update loan numbers
        loanRemaining -= loanRepaymentPerDay;
        if (loanRemaining < 0) loanRemaining = 0;

        float cash = economy ? economy.currentCash : 0f;

        if (summaryText)
        {
            summaryText.text =
                $"☀ Day Ended\n" +
                $"- Cash: ${cash:F2}\n" +
                $"- Loan Paid: ${loanRepaymentPerDay}\n" +
                $"- Loan Left: ${loanRemaining}";
        }

        UpdateLoanText();
        if (summaryPanel) summaryPanel.SetActive(true);

        if (loanRemaining <= 0)
        {
            StartCoroutine(FadeOutBeforeWin());
        }
    }

    // Called by Close button
    public void CloseSummaryPanel()
    {
        if (summaryPanel) summaryPanel.SetActive(false);
    }

    IEnumerator FadeOutBeforeWin()
    {
        if (fadePanelGroup)
        {
            fadePanelGroup.gameObject.SetActive(true);
            float duration = 1.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadePanelGroup.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
        }

        ShowWinScreen();
    }

    void ShowWinScreen()
    {
        if (summaryPanel) summaryPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(true);

        AudioSource audio = winPanel ? winPanel.GetComponent<AudioSource>() : null;
        if (audio != null) audio.Play();

        Animator animator = winPanel ? winPanel.GetComponent<Animator>() : null;
        if (animator != null) animator.Play("WinPanelFadeIn");

        StartCoroutine(FadeInWinPanel());
    }

    IEnumerator FadeInWinPanel()
    {
        if (winCanvasGroup)
        {
            winCanvasGroup.alpha = 0f;
            float duration = 1.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                winCanvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
        }
    }

    void UpdateLoanText()
    {
        if (loanText != null)
            loanText.text = $"Loan Remaining: ${loanRemaining}";
    }
}
