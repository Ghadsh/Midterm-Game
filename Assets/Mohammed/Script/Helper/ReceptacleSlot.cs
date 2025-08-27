// ReceptacleSlot.cs
using UnityEngine;

public enum ReceptacleType { Cup, Kettle }

public class ReceptacleSlot : MonoBehaviour
{
    [Header("Type")]
    public ReceptacleType type = ReceptacleType.Cup;

    [Header("Visuals")]
    [Tooltip("Enable this when leaves/beans are added (a mesh inside the cup/kettle).")]
    public GameObject leavesVisual;  // assign your small leaves/beans mesh here

    [Header("State (runtime)")]
    public bool hasLeaves;

    void Reset()
    {
        if (leavesVisual) leavesVisual.SetActive(false);
    }

    public bool TryAddLeavesVisual()
    {
        if (hasLeaves) return false;
        hasLeaves = true;
        if (leavesVisual) leavesVisual.SetActive(true);
        return true;
    }

    public void Clear()
    {
        hasLeaves = false;
        if (leavesVisual) leavesVisual.SetActive(false);
    }
}
