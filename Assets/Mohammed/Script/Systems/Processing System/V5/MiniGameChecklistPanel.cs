using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MiniGameChecklistPanel : MonoBehaviour
{
    [Header("Text fields")]
    public TMP_Text tempLine;    // e.g., "Temp: 82°C (70–90)"
    public TMP_Text pourLine;    // e.g., "Water: 200 ml (180–260)"
    public TMP_Text modsLine;    // e.g., "Mods: 2 / 2"
    public TMP_Text stirLine;    // e.g., "Stir: 78% / 75%"

    [Header("Colors")]
    public Color okColor = new Color(0.2f, 0.85f, 0.2f, 1f);
    public Color badColor = new Color(0.9f, 0.2f, 0.2f, 1f);

    [Header("Refs (bound at focus)")]
    public StationController station;   // bind at focus

    void Awake() { gameObject.SetActive(false); }

    public void Bind(StationController ctrl)
    {
        station = ctrl;
        gameObject.SetActive(station && station.CurrentStage == StationStage.Minigame);
        Refresh();
    }

    void OnEnable() { Refresh(); }

    void Update()
    {
        if (!station) { gameObject.SetActive(false); return; }
        bool show = station.CurrentStage == StationStage.Minigame;
        if (gameObject.activeSelf != show) gameObject.SetActive(show);
        if (show) Refresh();
    }

    void Refresh()
    {
        if (!station || station.ActiveRecipe == null) return;

        // Temp
        station.GetTemp(out float tCur, out float tMin, out float tMax);
        bool tempOK = tCur >= tMin && tCur <= tMax;
        if (tempLine)
        {
            tempLine.text = $"Temp: {tCur:0.#}°C ({tMin:0.#}–{tMax:0.#})";
            tempLine.color = tempOK ? okColor : badColor;
        }

        // Pour
        station.GetPour(out float ml, out float pMin, out float pMax);
        bool pourOK = ml >= pMin && ml <= pMax;
        if (pourLine)
        {
            string status = ml > pMax ? "Too full" : (ml < pMin ? "Too low" : "OK");
            pourLine.text = $"Water: {ml:0.#} ml ({pMin:0.#}–{pMax:0.#})  [{status}]";
            pourLine.color = pourOK ? okColor : badColor;
        }

        // Mods
        station.GetModsProgress(out int have, out int need);
        bool modsOK = need > 0 && have == need;
        if (modsLine)
        {
            modsLine.text = $"Mods: {have} / {need}";
            modsLine.color = modsOK ? okColor : badColor;
        }

        if (stirLine)
        {
            bool showStir = station.RequireStir;
            if (!showStir)
            {
                if (stirLine.gameObject.activeSelf) stirLine.gameObject.SetActive(false);
            }
            else
            {
                if (!stirLine.gameObject.activeSelf) stirLine.gameObject.SetActive(true);
                station.GetStir(out float sCur, out float sReq);
                bool stirOK = sCur >= sReq;
                stirLine.text = $"Stir: {(sCur * 100f):0.#}% / {(sReq * 100f):0.#}%";
                stirLine.color = stirOK ? okColor : badColor;
            }
        }
        }
}
