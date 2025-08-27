using UnityEngine;

// IInventory.cs
public interface IInventory
{
    bool Add(ItemDefinition item, int amount);
}
