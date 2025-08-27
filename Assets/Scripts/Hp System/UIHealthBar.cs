using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHealthBar : MonoBehaviour
{
    public HealthManager playerHealth;
    public Image healthFillImage;
    public TextMeshProUGUI healthText;

    void Update()
    {
        if (playerHealth == null) return;

        float percent = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        healthFillImage.fillAmount = percent;

        if (healthText != null)
            healthText.text = $"HP: {playerHealth.currentHealth} / {playerHealth.maxHealth}";
    }
}
