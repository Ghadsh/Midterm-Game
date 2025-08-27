using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoilTaim : MonoBehaviour
{
    // لا نحتاج أي حقول عامة — كله ينتهي تهيئته من الكود
    private Canvas _canvas;
    private GameObject _panel;
    private TextMeshProUGUI _interactionText;

    // نخزّن قائمة الـSoil لتقليل بحث كل فريم
    private Soil[] _allSoils;

    void Awake()
    {
        // 1) أنشئ Canvas + Panel + TextMeshPro إذا ما كانت موجودة
        CreateUIIfNeeded();

        // 2) جمّع كل الـSoil الموجودة بالمشهد (مرّة واحدة)
        // ملاحظة: FindObjectsOfType أصبح قديم بآخر الإصدارات، والأفضل FindObjectsByType إن كان متوفرًا.
#if UNITY_2023_1_OR_NEWER
        _allSoils = UnityEngine.Object.FindObjectsByType<Soil>(FindObjectsSortMode.None);
#else
        _allSoils = UnityEngine.Object.FindObjectsOfType<Soil>();
#endif
    }

    void Update()
    {
        bool showUI = false;

        // نحدّد الـSoil “المختارة” بأنها الأقرب للّاعب و playerInRange == true
        Soil soil = GetClosestInRangeSoil();

        if (soil != null)
        {
            showUI = true;

            if (soil.isEmpty)
            {
                _interactionText.text = "Soil (Press E to plant)";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    soil.PlantSeed();
                }
            }
            else
            {
                // نجلب النبتة الحالية ونفحص هل جاهزة للحصاد
                Plant currentPlant = soil.GetComponentInChildren<Plant>();
                if (currentPlant != null && currentPlant.isReadyToHarvest)
                {
                    _interactionText.text = $"Ready to harvest {soil.plantName} (Press F)";
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        soil.HarvestPlant();
                    }
                }
                else
                {
                    // تربة مزروعة بس مو جاهزة
                    _interactionText.text = soil.plantName;
                }
            }
        }

        _panel.SetActive(showUI);
    }

    // ———————————————————— Helpers ————————————————————

    private Soil GetClosestInRangeSoil()
    {
        if (_allSoils == null || _allSoils.Length == 0) return null;

        Vector3 p = transform.position; // نفترض إن السكربت مركّب على اللاعب
        float bestDist = float.MaxValue;
        Soil best = null;

        for (int i = 0; i < _allSoils.Length; i++)
        {
            Soil s = _allSoils[i];
            if (s == null) continue;

            // نعتمد فلاغ playerInRange اللي سكربت Soil يحدّثه (Triggers مثلا)
            if (s.playerInRange)
            {
                float d = (s.transform.position - p).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    best = s;
                }
            }
        }
        return best;
    }

    private void CreateUIIfNeeded()
    {
        // ابحث عن Canvas باسم محدد، وإن ما وُجد أنشئ واحد
        var canvasGo = GameObject.Find("__AutoCanvas__");
        if (canvasGo == null)
        {
            canvasGo = new GameObject("__AutoCanvas__");
            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasGo.AddComponent<GraphicRaycaster>();
        }
        else
        {
            _canvas = canvasGo.GetComponent<Canvas>();
        }

        // Panel للخلفية
        _panel = GameObject.Find("__AutoPanel__");
        if (_panel == null)
        {
            _panel = new GameObject("__AutoPanel__");
            _panel.transform.SetParent(_canvas.transform, false);

            var img = _panel.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.45f); // شبه شفاف

            var rect = _panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0, 40);   // فوق أسفل الشاشة بقليل
            rect.sizeDelta = new Vector2(900, 90);
        }

        // TextMeshProUGUI للنص
        var textGo = GameObject.Find("__AutoInteractionText__");
        if (textGo == null)
        {
            textGo = new GameObject("__AutoInteractionText__");
            textGo.transform.SetParent(_panel.transform, false);
            _interactionText = textGo.AddComponent<TextMeshProUGUI>();
            _interactionText.alignment = TextAlignmentOptions.Center;
            _interactionText.fontSize = 36;
            _interactionText.enableWordWrapping = true;

            var rect = _interactionText.rectTransform;
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(20, 10);
            rect.offsetMax = new Vector2(-20, -10);
        }
        else
        {
            _interactionText = textGo.GetComponent<TextMeshProUGUI>();
        }

        // نبدأ مطفي، ونشغّله فقط عند وجود هدف
        _panel.SetActive(false);
    }
}
