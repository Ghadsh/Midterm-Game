using UnityEngine;

// IEconomy.cs
public interface IEconomy
{
    int Coins { get; }
    bool TrySpend(int amount);
    void AddCoins(int amount);
}
