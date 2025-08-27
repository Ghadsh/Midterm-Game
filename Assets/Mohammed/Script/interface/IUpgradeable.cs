using UnityEngine;

// IUpgradeable.cs
public interface IUpgradeable
{
    string UpgradeId { get; }
    bool IsUnlocked { get; }
    bool TryUnlock(IEconomy econ);
}
