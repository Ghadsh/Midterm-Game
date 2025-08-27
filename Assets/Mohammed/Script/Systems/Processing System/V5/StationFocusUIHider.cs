// StationFocusUIHider.cs
using System.Collections.Generic;
using UnityEngine;

public class StationFocusUIHider : MonoBehaviour
{
    [Tooltip("Any UI panels you want auto-hidden while using the station (e.g., InventoryPanel, QuestLog, Map, etc.).")]
    public GameObject[] panelsToHide;

    readonly List<(GameObject go, bool wasActive)> _states = new();

    public void HideExtras()
    {
        _states.Clear();
        foreach (var go in panelsToHide)
        {
            if (!go) continue;
            _states.Add((go, go.activeSelf));
            go.SetActive(false);
        }
    }

    public void RestoreExtras()
    {
        foreach (var s in _states)
            if (s.go) s.go.SetActive(s.wasActive);
        _states.Clear();
    }
}
