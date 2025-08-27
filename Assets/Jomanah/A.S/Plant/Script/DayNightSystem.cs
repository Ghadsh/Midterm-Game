using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    // ساعة حالية 0..23 – عدّليها لاحقاً حسب نظام الوقت عندك
    [Range(0, 23)] public int currentHour = 18;

    // مثال لتحديث الساعة بشكل تجريبي
    // احذفي/عدّلي هذا لاحقاً إذا عندك TimeManager حقيقي
    [Tooltip("سرعة تقدّم الوقت (ساعات لكل ثانية)")]
    public float hoursPerSecond = 0.5f;
    private float _acc;

    private void Update()
    {
        _acc += Time.deltaTime * hoursPerSecond;
        while (_acc >= 1f)
        {
            _acc -= 1f;
            currentHour = (currentHour + 1) % 24;
        }
    }
}
