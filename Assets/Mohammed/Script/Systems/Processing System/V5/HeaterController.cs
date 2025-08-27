// HeaterController.cs
using UnityEngine;
using UnityEngine.Events;

public class HeaterController : MonoBehaviour
{
    [Range(0, 3)] public int Level = 0; // 0=off, 1-3 heat
    public UnityEvent<int> onLevelChanged;

    public void SetLevel(int level, bool invokeChanged = true)
    {
        Level = Mathf.Clamp(level, 0, 3);
        if (invokeChanged) onLevelChanged?.Invoke(Level);
    }

    // Example keybinds while testing (remove in production)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) SetLevel(0);
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetLevel(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetLevel(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetLevel(3);
    }
}
