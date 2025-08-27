// UIOverlayController.cs
using UnityEngine;

public class UIOverlayController : MonoBehaviour
{
    [Tooltip("Scripts to disable when any UI is open (look, move, interactors, hotbar use, etc.)")]
    public MonoBehaviour[] gameplayScripts;

    void OnEnable() { UIState.OnBlockChanged += Apply; }
    void OnDisable() { UIState.OnBlockChanged -= Apply; }

    void Apply(bool blocked)
    {
        foreach (var mb in gameplayScripts) if (mb) mb.enabled = !blocked;
        Cursor.lockState = blocked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = blocked;
        Time.timeScale = 1f; // keep time running; pause only if YOU want it
    }
}
