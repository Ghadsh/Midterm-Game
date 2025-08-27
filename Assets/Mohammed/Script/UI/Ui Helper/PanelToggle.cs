// PanelToggle.cs (attach to each UI panel root)
using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    public GameObject panel;

    public void Open()
    {
        panel.SetActive(true);
        UIState.PushBlock();
    }
    public void Close()
    {
        panel.SetActive(false);
        UIState.PopBlock();
    }
}
