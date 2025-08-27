// GameEvents.cs
using System;
using UnityEngine;

public static class GameEvents
{
    // Fired when player tries to plant a seed from hotbar.
    // Args: seedItem, world position (or hit point).
    public static Action<ItemDefinition, Vector3> OnSeedUseRequested;
}
