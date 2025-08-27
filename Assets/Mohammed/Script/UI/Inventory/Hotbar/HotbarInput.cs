// HotbarInput.cs
using UnityEngine;

public class HotbarInput : MonoBehaviour
{
    [SerializeField] private HotbarController hotbar;
    [SerializeField] private HotbarUI hotbarUI;

    void Awake()
    {
        if (!hotbar) hotbar = FindObjectOfType<HotbarController>();
        if (!hotbarUI) hotbarUI = FindObjectOfType<HotbarUI>();
    }

    void Update()
    {
        if (UIState.BlockGameplay) return;

        // Number keys 1..9 -> select index 0..8
        for (int k = 0; k < hotbar.Size && k < 9; k++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + k))
            {
                hotbar.SelectIndex(k);
                hotbarUI.ForceRefresh();
            }
        }

        // Mouse wheel
        float scroll = Input.mouseScrollDelta.y;
        if (scroll > 0.1f) { hotbar.SelectNext(+1); hotbarUI.ForceRefresh(); }
        else if (scroll < -0.1f) { hotbar.SelectNext(-1); hotbarUI.ForceRefresh(); }
    }
}
