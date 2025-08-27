//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DayNightCycle : MonoBehaviour
//{
//    public float dayLengthInMinutes = 10f;
//    public Light sun;
//    public Gradient lightColorGradient;
//    public Gradient ambientColorGradient;
//    public AnimationCurve lightIntensityCurve;


//    private float timeOfDay = 0f;

//    public delegate void DayStarted();
//    public static event DayStarted OnNewDay;

//    void Update()
//    {
//        timeOfDay += Time.deltaTime / (dayLengthInMinutes * 60f);

//        if (timeOfDay >= 1f)
//        {
//            timeOfDay = 0f;
//            OnNewDay?.Invoke();
//        }

//        UpdateLighting(timeOfDay);
//    }

//    void UpdateLighting(float time)
//    {
//        float sunAngle = time * 360f - 90f;
//        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0);
//        sun.color = lightColorGradient.Evaluate(time);
//        sun.intensity = lightIntensityCurve.Evaluate(time);
//        RenderSettings.ambientLight = ambientColorGradient.Evaluate(time);
//    }
//}

//using UnityEngine;

//public class DayNightCycle : MonoBehaviour
//{
//    public Light sun;
//    public Gradient lightColorGradient;
//    public Gradient ambientColorGradient;
//    public AnimationCurve lightIntensityCurve;
//    public GameObject startDayButton;

//    public delegate void DayStarted();
//    public static event DayStarted OnNewDay;

//    public float timeOfDay = 0f;

//    public void StartNewDay()
//    {
//        timeOfDay = 0f;

//        UpdateLighting(timeOfDay);

//        OnNewDay?.Invoke();
//        if (startDayButton != null)
//            startDayButton.SetActive(false);
//    }

//    void UpdateLighting(float time)
//    {
//        float sunAngle = time * 360f - 90f;
//        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0);
//        sun.color = lightColorGradient.Evaluate(time);
//        sun.intensity = lightIntensityCurve.Evaluate(time);
//        RenderSettings.ambientLight = ambientColorGradient.Evaluate(time);
//    }
//}

using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float dayLengthInMinutes = 3f; // Real-world duration of full day-night cycle
    public Light sun;
    public Gradient lightColorGradient;
    public Gradient ambientColorGradient;
    public AnimationCurve lightIntensityCurve;

    public delegate void DayStarted();
    public static event DayStarted OnNewDay;

    private float timeOfDay = 0f;

    void Start()
    {
        RenderSettings.ambientLight = ambientColorGradient.Evaluate(0f);
        sun.color = lightColorGradient.Evaluate(0f);
        sun.intensity = lightIntensityCurve.Evaluate(0f);
        sun.transform.rotation = Quaternion.Euler(-90f, 170f, 0);

        StartNewDay();
    }

    void Update()
    {
        timeOfDay += Time.deltaTime / (dayLengthInMinutes * 60f);

        if (timeOfDay >= 1f)
        {
            StartNewDay(); // Ì»œ√ ÌÊ„ ÃœÌœ  ·ﬁ«∆Ì
        }

        UpdateLighting(timeOfDay);
    }

    public void StartNewDay()
    {
        timeOfDay = 0f;

        UpdateLighting(timeOfDay);
        OnNewDay?.Invoke();
    }

    void UpdateLighting(float time)
    {
        float sunAngle = time * 360f - 30f;
        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0);
        sun.color = lightColorGradient.Evaluate(time);
        sun.intensity = lightIntensityCurve.Evaluate(time);
        RenderSettings.ambientLight = ambientColorGradient.Evaluate(time);
    }
}