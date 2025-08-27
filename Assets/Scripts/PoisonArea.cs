using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class PoisonArea : MonoBehaviour
{
    public int damagePerSecond = 5;
    public float damageInterval = 1f;
    public float poisonDuration = 5f;
    public float exitDelay = 2f;
    public bool isActive = true;

    [Header("UI Settings")]
    public Slider poisonSlider;
    public CanvasGroup poisonUI;

    private Dictionary<HealthManager, Coroutine> activePoisonCoroutines = new();

    private void Start()
    {
        if (poisonSlider != null)
            poisonSlider.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (!isActive) return;

        HealthManager health = trigger.GetComponent<HealthManager>();
        if (health != null && !activePoisonCoroutines.ContainsKey(health))
        {
            Coroutine poison = StartCoroutine(ApplyPoison(health));
            activePoisonCoroutines[health] = poison;
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (!isActive) return;

        HealthManager health = trigger.GetComponent<HealthManager>();
        if (health != null && activePoisonCoroutines.ContainsKey(health))
        {
            Coroutine linger = StartCoroutine(StopPoisonAfterDelay(health, exitDelay));
            activePoisonCoroutines[health] = linger;
        }
    }

    private IEnumerator ApplyPoison(HealthManager health)
    {
        float elapsed = 0f;

        // Show UI
        if (poisonSlider != null)
        {
            poisonSlider.maxValue = poisonDuration;
            poisonSlider.value = poisonDuration;
            poisonSlider.gameObject.SetActive(true);
        }

        while (elapsed < poisonDuration)
        {
            if (!isActive) yield break;

            // Damage player
            health.TakeDamage(damagePerSecond);

            // Update slider
            if (poisonSlider != null)
                poisonSlider.value = poisonDuration - elapsed;

            yield return new WaitForSeconds(damageInterval);
            elapsed += damageInterval;
        }

        // Hide UI
        if (poisonSlider != null)
            poisonSlider.gameObject.SetActive(false);

        activePoisonCoroutines.Remove(health);
    }

    private IEnumerator StopPoisonAfterDelay(HealthManager health, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Hide UI
        if (poisonSlider != null)
            poisonSlider.gameObject.SetActive(false);

        activePoisonCoroutines.Remove(health);
    }

    public void TogglePoison(bool state)
    {
        isActive = state;
    }
}
