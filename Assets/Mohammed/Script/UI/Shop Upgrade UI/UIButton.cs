// UIButton.cs (optional helper)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    Button _btn;

    void Awake()
    {
        _btn = GetComponent<Button>();
        if (!label) label = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void SetText(string t) { if (label) label.text = t; }
    public void OnClick(System.Action action)
    {
        _btn.onClick.RemoveAllListeners();
        _btn.onClick.AddListener(() => action?.Invoke());
    }
}
