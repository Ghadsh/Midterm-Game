// StationController.cs
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum StationStage { Idle, Preview , Ready, Minigame, Completed }

public class StationController : MonoBehaviour
{

    [Header("Feature Flags")]
    [SerializeField] private bool requireStir = false;   // OFF = spoon minigame disabled
    public bool RequireStir => requireStir;

    [Header("Refs")]
    public StationInventory inventory;
    public HeaterController heater;
    public KettleHeat kettleHeat;
    public Transform kettleHome; //return home
    public StationFocusCamera focusCam; // optional, for saving cam pose
    public TeaRecipeResolver recipeResolver;
    public KettlePour kettlePour; // assign Kettle child
    public CupContainer cup;      // assign Cup child
    public CupIntake cupIntake;   // assign Cup child
    public SpoonStir spoon;       // assign Spoon child
    public TeaRecipe ActiveRecipe => _activeRecipe;     // expose

    [Header("Spawn")]
    public GameObject genericLeafPrefab;   // assign if you have one; otherwise we’ll fallback
    public GameObject genericModPrefab;
    public Transform modsTray;             // where items appear
    public Transform spawnArea;            // optional fallback pad
    public float spawnRadius = 0.25f;
    public float minDistanceFromCup = 0.35f;
    public float spawnHeight = 0.05f;

    [Header("State")]
    [SerializeField] StationStage _stage = StationStage.Idle;
    TeaRecipe _activeRecipe;

    [Header("Processing")]
    public float processDuration = 6f; // example total time (tunable)
    public AnimationCurve processCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public StationStage CurrentStage => _stage;

    float _t; // stage timer
    float _camYaw, _camPitch; // stored station-side

    bool HasRecipeReady()
    {
        if (inventory == null || recipeResolver == null) return false;
        var baseItem = inventory.baseSlot?.item;
        if (!baseItem) return false;
        return recipeResolver.FindMatch(baseItem, inventory.modSlots) != null;
    }

    // Set stage to Idle / Ready based on inventory, unless we’re in Minigame/Completed
    void RecomputeStageFromInventory()
    {
        if (_stage == StationStage.Minigame || _stage == StationStage.Completed) return;

        _stage = HasRecipeReady() ? StationStage.Ready : StationStage.Idle;
        SaveState();
    }

    public int RequiredCountFor(ItemDefinition mod)
    {
        if (_activeRecipe == null || mod == null) return 0;
        int need = 0;
        foreach (var r in _activeRecipe.mods)
            if (r.item == mod) need += Mathf.Max(1, r.count);
        return need;
    }

    public void GetTemp(out float cur, out float min, out float max)
    {
        cur = kettleHeat ? kettleHeat.tempC : 20f;
        if (_activeRecipe != null) { min = _activeRecipe.targetTempMinC; max = _activeRecipe.targetTempMaxC; }
        else { min = 0f; max = 0f; }
    }
    public void GetPour(out float ml, out float min, out float max)
    {
        ml = cup ? cup.waterMl : 0f;
        if (_activeRecipe != null) { min = _activeRecipe.pourMlMin; max = _activeRecipe.pourMlMax; }
        else { min = 0f; max = 0f; }
    }
    public void GetStir(out float stir01, out float required01)
    {
        stir01 = cup ? cup.stirProgress01 : 0f;
        required01 = _activeRecipe != null ? _activeRecipe.stirRequired01 : 1f;
    }
    public void GetModsProgress(out int haveTotal, out int needTotal)
    {
        haveTotal = 0; needTotal = 0;
        if (_activeRecipe == null) return;

        // build 'have'
        if (cup != null && cup.mods != null)
            foreach (var kv in cup.mods) haveTotal += kv.Value;

        // build 'need'
        foreach (var r in _activeRecipe.mods)
            needTotal += Mathf.Max(1, r.count);
    }

    public void SetSavedCamera(float yaw, float pitch) { _camYaw = yaw; _camPitch = pitch; }
    public bool TryGetSavedCamera(out float yaw, out float pitch)
    {
        yaw = _camYaw; pitch = _camPitch;
        return true;
    }
    // Recipe/preview resolver (plug your logic here)
    public IRecipeResolver resolver = new DefaultRecipeResolver();

    public void ResetAfterCollect()
    {
        // Clear world-side state (keep it minimal)
        if (cup) cup.ResetAll();

        if (kettleHeat)
        {
            kettleHeat.tempC = 20f;
            // Optional: reset position/orientation if you set kettleHome
            if (kettleHome)
                kettleHeat.transform.SetPositionAndRotation(kettleHome.position, kettleHome.rotation);

            if (spoon) spoon.gameObject.SetActive(true);
        }

        _activeRecipe = null;
        _stage = StationStage.Idle;
        SaveState();
    }


    void Awake()
    {
        if (inventory) inventory.Changed += OnInventoryChanged;
        // optional safety
        if (kettlePour) kettlePour.cup = cup;
    }

    void OnDestroy()
    {
        if (inventory) inventory.Changed -= OnInventoryChanged;
    }

    void OnInventoryChanged()
    {
        RecomputeStageFromInventory();
    }

    void Start()
    {
        LoadState();
        if (_stage == StationStage.Idle || _stage == StationStage.Preview) _stage = StationStage.Preview;
        SaveState(); // ensure file exists early
    }

    void Update()
    {
        if (_stage == StationStage.Minigame)
            TryCompleteMinigame();
    }
    // StationController.cs
    public bool TryBeginProcess()
    {
        Debug.Log("[Station] TryBeginProcess()");

        if (!requireStir && spoon) spoon.gameObject.SetActive(false);


        // If we’re not truly in Minigame/Completed, recompute Ready/Idle now
        if (_stage != StationStage.Minigame && _stage != StationStage.Completed)
            RecomputeStageFromInventory();

        if (_stage == StationStage.Minigame || _stage == StationStage.Completed)
        {
            Debug.Log("…blocked: stage (" + _stage + ")");
            return false;
        }

        if (!inventory || !recipeResolver)
        {
            Debug.Log("…blocked: missing refs (inventory/resolver)");
            return false;
        }

        // READ base item & count BEFORE we clear it
        var baseStack = inventory.baseSlot;
        var baseItem = baseStack != null ? baseStack.item : null;
        var baseCount = baseStack != null ? Mathf.Max(1, baseStack.count) : 0;

        if (!baseItem || baseCount <= 0)
        {
            Debug.Log("…blocked: no base in station");
            return false;
        }

        // Validate exact staged mods for the recipe
        var recipe = recipeResolver.FindMatch(baseItem, inventory.modSlots);
        if (recipe == null)
        {
            Debug.Log("…blocked: no recipe for current Base+Mods");
            return false;
        }

        _activeRecipe = recipe;

        // 1) Consume BASE from the station UI (so you can’t change it mid-minigame)
        inventory.ClearBase();

        // 2) Spawn physical LEAVES (count is informational; Cup takes 1 "unit")
        SpawnLeaves(baseItem, baseCount);

        // 3) Spawn physical MOD items for each staged slot (don’t clear station’s mod slots yet;
        //    we’ll clear them only on successful completion)
        for (int i = 0; i < inventory.modSlots.Count; i++)
        {
            var s = inventory.modSlots[i];
            if (s != null && s.item != null && s.count > 0)
                SpawnMod(s.item, s.count);
        }

        _stage = StationStage.Minigame;
        SaveState();
        Debug.Log("[Station] Minigame started. Base spawned = " + baseItem.displayName + " x" + baseCount);
        return true;
    }


    void CompleteProcess()
    {
        _stage = StationStage.Completed;

        if (kettleHeat && kettleHome)
            kettleHeat.transform.SetPositionAndRotation(kettleHome.position, kettleHome.rotation);

        SaveState();
    }


    public (Sprite icon, string name) GetPreviewForCurrentContents()
    {
        if (inventory == null)
            return (null, "Add a Base item");

        var baseItem = inventory.baseSlot?.item;
        if (baseItem == null)
            return (null, "Add a Base item");

        var recipe = recipeResolver ? recipeResolver.FindMatch(baseItem, inventory.modSlots) : null;
        if (recipe == null)
            return (baseItem.icon, "No known recipe");

        var outItem = recipe.resultItem;
        return (outItem ? outItem.icon : null, outItem ? outItem.displayName : "Recipe result");
    }

    void SpawnLeaves(ItemDefinition baseItem, int count)
    {
        var prefab = SafePrefab(genericLeafPrefab, "Leaf_Fallback", PrimitiveType.Sphere, 0.06f);
        Vector3 pos = SafeSpawnPos();
        var go = Instantiate(prefab, pos, Quaternion.identity);
        PrepareIngestible(go, CupIngestible.Kind.Leaves, baseItem, Mathf.Max(1, count));
        Debug.Log($"[Station] Spawned LEAVES x{count} at {pos}");
    }


    void SpawnMod(ItemDefinition modItem, int count)
    {
        var prefab = SafePrefab(genericModPrefab, "Mod_Fallback", PrimitiveType.Cube, 0.05f);
        Vector3 pos = SafeSpawnPos();
        var go = Instantiate(prefab, pos, Quaternion.identity);
        PrepareIngestible(go, CupIngestible.Kind.Mod, modItem, Mathf.Max(1, count));
        Debug.Log($"[Station] Spawned MOD {modItem?.displayName} x{count} at {pos}");
    }
    GameObject SafePrefab(GameObject src, string name, PrimitiveType fallback, float uniformScale)
    {
        if (src) return src;
        var prim = GameObject.CreatePrimitive(fallback);
        prim.name = name;
        prim.transform.localScale = Vector3.one * uniformScale;
        return prim; // has a collider by default
    }
    Vector3 SafeSpawnPos()
    {
        Vector3 baseP = modsTray ? modsTray.position : (spawnArea ? spawnArea.position : transform.position);
        for (int i = 0; i < 8; i++)
        {
            var rnd = Random.insideUnitCircle * spawnRadius;
            var p = new Vector3(baseP.x + rnd.x, baseP.y + spawnHeight, baseP.z + rnd.y);
            if (!cup || Vector3.Distance(p, cup.transform.position) >= minDistanceFromCup)
                return p;
        }
        return baseP + Vector3.right * minDistanceFromCup + Vector3.up * spawnHeight;
    }

    void PrepareIngestible(GameObject go, CupIngestible.Kind kind, ItemDefinition def, int count)
    {
        var ing = go.GetComponent<CupIngestible>() ?? go.AddComponent<CupIngestible>();
        ing.kind = kind;
        ing.itemDef = def;
        ing.count = count;

        if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().radius = 0.05f;
        var rb = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (!go.GetComponent<Drag3DHandle>()) go.AddComponent<Drag3DHandle>();
    }


    Vector3 Near(Vector3 p, float r)
    {
        var rnd = Random.insideUnitCircle * r;
        return new Vector3(p.x + rnd.x, p.y + 0.02f, p.z + rnd.y);
    }

    void EnsureDraggable(GameObject go)
    {
        if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().radius = 0.05f;
        if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
        if (!go.GetComponent<Draggable3D>()) go.AddComponent<Draggable3D>(); // your existing script
    }

    // Called by CupIntake when contents change, and we also poll in Update()
    public void OnCupContentsChanged() => TryCompleteMinigame();


    void TryCompleteMinigame()
    {
        if (_activeRecipe == null || _stage != StationStage.Minigame || cup == null || kettleHeat == null) return;

        bool hasLeaves = cup.leavesUnits >= 1f;
        bool modsOk = ModsMatch(_activeRecipe.mods, cup.mods);
        bool tempOk = kettleHeat.tempC >= _activeRecipe.targetTempMinC &&
                         kettleHeat.tempC <= _activeRecipe.targetTempMaxC;
        bool pourOk = cup.waterMl >= _activeRecipe.pourMlMin &&
                         cup.waterMl <= _activeRecipe.pourMlMax;

        // ⬇️ stir is considered satisfied if requireStir == false
        bool stirOk = !requireStir || (cup.stirProgress01 >= _activeRecipe.stirRequired01);

        if (hasLeaves && modsOk && tempOk && pourOk && stirOk)
        {
            for (int i = inventory.modSlots.Count - 1; i >= 0; i--)
                inventory.RemoveModAt(i);

            inventory.AddToResult(_activeRecipe.resultItem, _activeRecipe.resultCount);
            _stage = StationStage.Completed;
            SaveState();
        }
    }


    bool ModsMatch(List<TeaRecipe.ModReq> reqs, Dictionary<ItemDefinition, int> have)
    {
        // build needed
        var need = new Dictionary<ItemDefinition, int>();
        foreach (var r in reqs)
        {
            if (!r.item || r.count <= 0) continue;
            need[r.item] = need.TryGetValue(r.item, out var c) ? c + r.count : r.count;
        }
        // compare
        if (need.Count != have.Count) return false;
        foreach (var kv in need)
            if (!have.TryGetValue(kv.Key, out var c) || c != kv.Value) return false;
        return true;
    }

    // ---------- Persistence ----------
    string SavePath => Path.Combine(Application.persistentDataPath, $"station_{inventory.stationId}.json");

    void OnApplicationQuit() => SaveState();
    void OnDisable() => SaveState();

    public void SaveState()
    {
        if (inventory == null) return;

        var data = inventory.BuildPersist(
            id => resolver.FindById(id),
            def => resolver.IdOf(def),
            heater ? heater.Level : 0,
            kettleHeat ? kettleHeat.transform.localPosition : Vector3.zero,
            kettleHeat ? kettleHeat.transform.localRotation : Quaternion.identity,
            _stage.ToString(),
            _t
        );
        data.camYaw = _camYaw;
        data.camPitch = _camPitch;

        var json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(SavePath, json);

        // Optional: camera memory
        // (Not strictly required; focusCam itself handles enter/exit behavior)
        File.WriteAllText(SavePath, json);
    }

    public void LoadState()
    {

        if (!File.Exists(SavePath)) { _stage = StationStage.Idle; return; }

        var json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<StationInventory.PersistData>(json);
        inventory.RestorePersist(data, id => resolver.FindById(id));

        if (heater) heater.SetLevel(data.heaterLevel, invokeChanged: false);
        if (kettleHeat)
        {
            kettleHeat.transform.localPosition = data.kettlePos;
            kettleHeat.transform.localRotation = data.kettleRot;
        }

        // parse stage with safety
        if (!System.Enum.TryParse<StationStage>(data.stage, out _stage))
            _stage = StationStage.Idle;

        // SANITIZE: if saved as Minigame but the cup is empty & no recipe is active, go back to Ready/Idle
        bool looksEmpty = (cup == null) || (cup.leavesUnits <= 0f && cup.waterMl <= 0f && (cup.mods == null || cup.mods.Count == 0));
        if (_stage == StationStage.Minigame && looksEmpty)
            _stage = HasRecipeReady() ? StationStage.Ready : StationStage.Idle;

        // SANITIZE: if saved as Completed but there’s no result, go back to Ready/Idle
        bool hasResult = inventory.resultSlot != null && inventory.resultSlot.item != null && inventory.resultSlot.count > 0;
        if (_stage == StationStage.Completed && !hasResult)
            _stage = HasRecipeReady() ? StationStage.Ready : StationStage.Idle;

        _t = data.stageTime;
        _camYaw = data.camYaw;
        _camPitch = data.camPitch;

    }
}

// ---------- Preview / recipe hook ----------
public interface IRecipeResolver
{
    (Sprite icon, string name) Preview(ItemDefinition baseItem, System.Collections.Generic.List<StationItemStack> mods);
    ItemDefinition FindById(string id);
    string IdOf(ItemDefinition def);
}

// Minimal default so UI can show *something*; replace with your real logic.
public class DefaultRecipeResolver : IRecipeResolver
{
    public (Sprite icon, string name) Preview(ItemDefinition baseItem, System.Collections.Generic.List<StationItemStack> mods)
    {
        string name = baseItem.displayName;
        if (mods != null && mods.Count > 0) name += $" + {mods.Count} mod(s)";
        return (baseItem.icon, name);
    }

    public ItemDefinition FindById(string id)
    {
        // You can keep a dictionary in a bootstrapper; for demo we try Resources
        var all = Resources.LoadAll<ItemDefinition>("");
        foreach (var d in all) if (d.itemId == id) return d;
        return null;
    }

    public string IdOf(ItemDefinition def) => def ? def.itemId : "";
}
