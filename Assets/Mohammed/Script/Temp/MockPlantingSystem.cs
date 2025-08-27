// MockPlantingSystem.cs
using System.Collections.Generic;
using UnityEngine;

public class MockPlantingSystem : MonoBehaviour
{
    [Header("Placement")]
    public LayerMask groundMask;
    public float cellSize = 1f;
    public Transform plantedParent;      // optional, for hierarchy cleanliness
    public GameObject placeholderPlant;  // e.g., a green cube with a collider

    [Header("Hotbar Link")]
    public HotbarController hotbar;      // to know hotbar range (0..Size-1)

    private HashSet<Vector2Int> occupied = new(); // which cells are used
    private InventoryService inventory;

    void Awake()
    {
        inventory = FindObjectOfType<InventoryService>();
        if (!hotbar) hotbar = FindObjectOfType<HotbarController>();
    }
    void OnEnable() { GameEvents.OnSeedUseRequested += TryPlant; }
    void OnDisable() { GameEvents.OnSeedUseRequested -= TryPlant; }

    void TryPlant(ItemDefinition seed, Vector3 worldPos)
    {
        // snap to grid
        Vector3 p = worldPos;
        p.x = Mathf.Round(p.x / cellSize) * cellSize;
        p.z = Mathf.Round(p.z / cellSize) * cellSize;

        var cell = new Vector2Int(Mathf.RoundToInt(p.x / cellSize),
                                  Mathf.RoundToInt(p.z / cellSize));
        if (occupied.Contains(cell))
        {
            Debug.Log("Cell occupied.");
            return;
        }

        // spawn placeholder plant
        if (!placeholderPlant)
        {
            // auto-build a tiny cube if missing
            placeholderPlant = GameObject.CreatePrimitive(PrimitiveType.Cube);
            placeholderPlant.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
        var go = Instantiate(placeholderPlant, plantedParent ? plantedParent : null);
        go.transform.position = new Vector3(p.x, p.y, p.z);
        occupied.Add(cell);

        // consume 1 seed from HOTBAR RANGE ONLY (so other inventory slots aren’t touched)
        if (inventory && hotbar)
        {
            int idx = inventory.FindFirstIndexOf(seed, 0, hotbar.Size);
            if (idx >= 0) inventory.TryConsumeAt(idx, 1);
        }
    }
}
