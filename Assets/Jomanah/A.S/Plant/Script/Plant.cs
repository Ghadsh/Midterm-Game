using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Plant lifecycle & produce generation.
/// Requires a scene-level TimeManager singleton with a public UnityEvent OnDayPass.
/// </summary>
public class Plant : MonoBehaviour
{
    // ---------------- Visual States ----------------
    [Header("Plant Visual States (assign in Inspector)")]
    [SerializeField] private GameObject seedMode1;
    [SerializeField] private GameObject youngPlantMode1;
    [SerializeField] private GameObject maturePlantMode1;

    // ---------------- Produce ----------------
    [Header("Produce Spawns & Prefab")]
    [Tooltip("Transforms where produce will appear. Each should be an empty parent.")]
    [SerializeField] private List<GameObject> PlantProduceSpawns = new List<GameObject>();
    [SerializeField] private GameObject producePrefab;

    // ---------------- Ages ----------------
    [Header("Ages (in in-game days)")]
    [Tooltip("Current age; increments on day pass if watered.")]
    [SerializeField] private int plantAge = 0;
    [Tooltip("Age at which young plant model appears.")]
    [SerializeField] private int ageForYoungMode1 = 1;
    [Tooltip("Age at which mature plant model appears.")]
    [SerializeField] private int ageForMatureMode1 = 3;
    [Tooltip("Age when the first batch of produce is generated.")]
    [SerializeField] private int ageForFirstProduceBatch = 3;

    // ---------------- Produce Timing ----------------
    [Header("Produce Timing")]
    [Tooltip("Days between new produce batches after first batch.")]
    [SerializeField] private int dayForNewProduce = 2;
    [Tooltip("Internal counter; counts down to next batch.")]
    [SerializeField] private int daysRemainingForNewProduceCounter = 0;

    // ---------------- Flags & State ----------------
    [Header("Flags")]
    [SerializeField] private bool isOneTimeHarvest = false;
    [SerializeField] private bool isWatered = false;

    [Header("Debug/State")]
    public bool isReadyToHarvest = false;

    // Optional: for analytics or save systems
    public int dayOfPlanting;

    // ---------------- Unity Messages ----------------
    private void OnEnable()
    {
        var tm = TimeManager.Instance;
        if (tm != null)
        {
            tm.OnDayPass.AddListener(DayPass);
        }
        else
        {
            Debug.LogWarning("[Plant] TimeManager not ready in OnEnable; will try again in Start.", this);
        }
    }

    private void Start()
    {
        var tm = TimeManager.Instance;
        if (tm != null)
        {
            // تأكيد عدم التكرار
            tm.OnDayPass.RemoveListener(DayPass);
            tm.OnDayPass.AddListener(DayPass);
        }
        else
        {
            Debug.LogError("[Plant] TimeManager.Instance is null in Start. Ensure a TimeManager exists in the scene and initializes first.", this);
        }

        if (daysRemainingForNewProduceCounter <= 0)
            daysRemainingForNewProduceCounter = dayForNewProduce;

        ApplyGrowthVisuals();
        UpdateHarvestReady();

        // تحققات
        if (producePrefab == null)
            Debug.LogError("[Plant] Produce prefab is not assigned.", this);
        if (PlantProduceSpawns == null || PlantProduceSpawns.Count == 0)
            Debug.LogWarning("[Plant] PlantProduceSpawns list is empty or null.", this);
        if (!seedMode1 && !youngPlantMode1 && !maturePlantMode1)
            Debug.LogWarning("[Plant] No visual state GameObjects assigned.", this);
    }

    private void OnDisable()
    {
        var tm = TimeManager.Instance;
        if (tm != null)
            tm.OnDayPass.RemoveListener(DayPass);
    }

    // ---------------- Day Tick ----------------
    private void DayPass()
    {
        if (isWatered)
            plantAge++;

        CheckGrowth();
        CheckProduce();
        UpdateHarvestReady();
    }

    // ---------------- Growth ----------------
    private void CheckGrowth() => ApplyGrowthVisuals();

    private void ApplyGrowthVisuals()
    {
        if (seedMode1)
            seedMode1.SetActive(plantAge < ageForYoungMode1);

        if (youngPlantMode1)
            youngPlantMode1.SetActive(plantAge >= ageForYoungMode1 && plantAge < ageForMatureMode1);

        if (maturePlantMode1)
            maturePlantMode1.SetActive(plantAge >= ageForMatureMode1);
    }

    // ---------------- Produce ----------------
    private void CheckProduce()
    {
        if (producePrefab == null || PlantProduceSpawns == null) return;

        if (plantAge == ageForFirstProduceBatch)
        {
            GenerateProduceForEmptySpawns();
        }
        else if (plantAge > ageForFirstProduceBatch)
        {
            if (daysRemainingForNewProduceCounter <= 0)
            {
                GenerateProduceForEmptySpawns();
                daysRemainingForNewProduceCounter = Mathf.Max(1, dayForNewProduce);
            }
            else
            {
                daysRemainingForNewProduceCounter--;
            }
        }
    }

    private void GenerateProduceForEmptySpawns()
    {
        if (producePrefab == null || PlantProduceSpawns == null) return;

        foreach (var spawn in PlantProduceSpawns)
        {
            if (!spawn) continue;

            // لازم تكون نقطة سباون فاضية (بدون أطفال)
            if (spawn.transform.childCount == 0)
            {
                var produce = Instantiate(producePrefab, spawn.transform);
                produce.transform.localPosition = Vector3.zero;
                // (اختياري) ثبتي دوران/حجم هنا لو احتجتي
                // produce.transform.localRotation = Quaternion.identity;
                // produce.transform.localScale    = Vector3.one * 0.25f;
            }
        }
    }

    private void UpdateHarvestReady()
    {
        isReadyToHarvest = false;
        if (PlantProduceSpawns == null) return;

        foreach (var spawn in PlantProduceSpawns)
        {
            if (spawn != null && spawn.transform.childCount > 0)
            {
                isReadyToHarvest = true;
                break;
            }
        }
    }

    public void Harvest()
    {
        if (PlantProduceSpawns != null)
        {
            foreach (var spawn in PlantProduceSpawns)
            {
                if (!spawn) continue;

                // إزالة كل الثمار الموجودة تحت كل سباون
                for (int i = spawn.transform.childCount - 1; i >= 0; i--)
                    Destroy(spawn.transform.GetChild(i).gameObject);
            }
        }

        if (isOneTimeHarvest)
        {
            Destroy(gameObject);
        }
        else
        {
            daysRemainingForNewProduceCounter = Mathf.Max(1, dayForNewProduce);
            isReadyToHarvest = false;
        }
    }
}
