using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryService : MonoBehaviour, IInventory
{
    [Serializable]
    public class Stack
    {
        public ItemDefinition item;
        public int count;
        public Stack() { }
        public Stack(ItemDefinition i, int c) { item = i; count = c; }
    }

    [SerializeField] private int slots = 20;
    [SerializeField] private List<Stack> _stacks = new();

    public Action OnInventoryChanged;
    public Action OnCapacityChanged;

    public int Capacity => Mathf.Max(0, slots);
    public IReadOnlyList<Stack> Stacks => _stacks;

    void Awake() => EnsureSize();

    void EnsureSize()
    {
        if (_stacks == null) _stacks = new List<Stack>();
        while (_stacks.Count < Capacity) _stacks.Add(null);
        if (_stacks.Count > Capacity) _stacks.RemoveRange(Capacity, _stacks.Count - Capacity);
    }

    public Stack GetAt(int index)
    {
        if (index < 0 || index >= Capacity) return null;
        return _stacks[index];
    }

    void SetAt(int index, Stack s)
    {
        if (index < 0 || index >= Capacity) return;
        _stacks[index] = s;
    }

    // IInventory
    public bool Add(ItemDefinition item, int amount)
    {
        if (item == null || amount <= 0) return false;
        EnsureSize();

        // 1) Stack into existing
        for (int i = 0; i < Capacity && amount > 0; i++)
        {
            var s = _stacks[i];
            if (s != null && s.item == item && item.stackable && s.count < item.maxStack)
            {
                int canAdd = Mathf.Min(amount, item.maxStack - s.count);
                s.count += canAdd;
                amount -= canAdd;
            }
        }

        // 2) Use empty slots
        for (int i = 0; i < Capacity && amount > 0; i++)
        {
            if (_stacks[i] == null)
            {
                int toAdd = item.stackable ? Mathf.Min(amount, item.maxStack) : 1;
                _stacks[i] = new Stack(item, toAdd);
                amount -= toAdd;
            }
        }

        OnInventoryChanged?.Invoke();
        return amount == 0;
    }

    // Optional convenience for bridges (non-breaking):
    // Remove a count of a specific ItemDefinition across the bag.
    public bool Remove(ItemDefinition item, int amount)
    {
        if (item == null || amount <= 0) return false;
        EnsureSize();

        // Count available
        int total = 0;
        for (int i = 0; i < Capacity; i++)
        {
            var s = _stacks[i];
            if (s != null && s.item == item) total += s.count;
        }
        if (total < amount) return false;

        // Consume across stacks
        for (int i = 0; i < Capacity && amount > 0; i++)
        {
            var s = _stacks[i];
            if (s == null || s.item != item) continue;
            int take = Mathf.Min(s.count, amount);
            s.count -= take;
            amount -= take;
            if (s.count <= 0) _stacks[i] = null;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public void MoveOrMerge(int from, int to)
    {
        EnsureSize();
        if (from == to) return;
        if (from < 0 || from >= Capacity) return;
        if (to < 0 || to >= Capacity) return;

        var a = _stacks[from];
        var b = _stacks[to];
        if (a == null) return;

        // Merge if same item & stackable
        if (b != null && a.item == b.item && a.item.stackable)
        {
            int space = a.item.maxStack - b.count;
            if (space > 0)
            {
                int moved = Mathf.Min(space, a.count);
                b.count += moved;
                a.count -= moved;
                if (a.count <= 0) _stacks[from] = null;
                OnInventoryChanged?.Invoke();
                return;
            }
        }

        // Swap/move
        _stacks[to] = a;
        _stacks[from] = b;
        OnInventoryChanged?.Invoke();
    }

    // Safe consume by slot index
    public bool TryConsumeAt(int index, int amount = 1)
    {
        var s = GetAt(index);
        if (s == null || s.count < amount) return false;
        s.count -= amount;
        if (s.count <= 0) SetAt(index, null);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int FindFirstIndexOf(ItemDefinition item, int start = 0, int endExclusive = -1)
    {
        if (endExclusive < 0) endExclusive = Capacity;
        for (int i = start; i < endExclusive; i++)
        {
            var s = GetAt(i);
            if (s != null && s.item == item) return i;
        }
        return -1;
    }

    public void SetCapacity(int newCap)
    {
        newCap = Mathf.Max(1, newCap);
        while (_stacks.Count < newCap) _stacks.Add(null);
        if (_stacks.Count > newCap) _stacks.RemoveRange(newCap, _stacks.Count - newCap);
        slots = newCap;
        OnCapacityChanged?.Invoke();
        OnInventoryChanged?.Invoke();
    }
}
