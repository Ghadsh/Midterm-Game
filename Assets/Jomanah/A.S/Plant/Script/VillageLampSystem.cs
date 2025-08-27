using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageLampSystem : MonoBehaviour
{
    [Header("Lamp Schedule (24h)")]
    [Tooltip("ساعة تشغيل المصابيح (0..23)")]
    [Range(0, 23)] public int timeLampsTurnOn = 18;

    [Tooltip("ساعة إطفاء المصابيح (0..23)")]
    [Range(0, 23)] public int timeLampsTurnOff = 6;

    [Tooltip("حالة المصابيح حالياً (للمراقبة فقط)")]
    public bool lampsAreOn;

    [Header("References")]
    [SerializeField] private DayNightSystem dayNightSystem;

    
    private readonly List<Light> _lights = new List<Light>();

    private void Awake()
    {
        CacheLights();
    }

    private void Reset()
    {
        CacheLights();
    }

    private void OnValidate()
    {
        
        if (!Application.isPlaying)
            CacheLights();
    }

    private void CacheLights()
    {
        _lights.Clear();
        foreach (Transform child in transform)
        {
            var l = child.GetComponent<Light>();
            if (l != null)
                _lights.Add(l);
        }
    }

    private void Update()
    {
        if (dayNightSystem == null) return;

        
        int hour = dayNightSystem.currentHour;

        bool shouldBeOn = IsHourInRange(hour, timeLampsTurnOn, timeLampsTurnOff);

        if (shouldBeOn != lampsAreOn)
            SetLamps(shouldBeOn);
    }

    
    private bool IsHourInRange(int hour, int start, int end)
    {
        if (start == end) return false;

        if (start < end)
        {
            
            return hour >= start && hour < end;
        }

        
        return hour >= start || hour < end;
    }

    private void SetLamps(bool on)
    {
        for (int i = 0; i < _lights.Count; i++)
        {
            if (_lights[i] != null)
                _lights[i].enabled = on;
        }

        lampsAreOn = on;
    }
}

