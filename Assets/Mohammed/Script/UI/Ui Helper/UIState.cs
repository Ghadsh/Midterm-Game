// UIState.cs
using System;
using UnityEngine;

public static class UIState
{
    public static bool BlockGameplay { get; private set; }
    public static event Action<bool> OnBlockChanged;

    static int depth = 0;
    public static void PushBlock() { depth++; Set(true); }
    public static void PopBlock() { depth = Mathf.Max(0, depth - 1); Set(depth > 0); }

    static void Set(bool v)
    {
        if (BlockGameplay == v) return;
        BlockGameplay = v;
        OnBlockChanged?.Invoke(v);
    }
}
