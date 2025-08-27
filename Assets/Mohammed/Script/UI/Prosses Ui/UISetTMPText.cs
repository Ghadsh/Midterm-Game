// UISetTMPText.cs
using UnityEngine;
using TMPro;

public class UISetTMPText : MonoBehaviour
{
    public TMP_Text target;

    // Must be public and take exactly one string
    public void SetText(string value)
    {
        if (!target) target = GetComponent<TMP_Text>();
        if (!target) return;
        target.text = value ?? "";
    }
}
