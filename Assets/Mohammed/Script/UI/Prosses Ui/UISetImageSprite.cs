// UISetImageSprite.cs
using UnityEngine;
using UnityEngine.UI;

public class UISetImageSprite : MonoBehaviour
{
    public Image target;

    public void SetSprite(Sprite s)
    {
        if (!target) target = GetComponent<Image>();
        if (!target) return;

        target.sprite = s;

        // Keep the Image component ON, just fade alpha
        var c = target.color;
        c.a = s ? 1f : 0f;
        target.color = c;

        // NEVER touch target.enabled here
        // NEVER touch Raycast Target here
    }
}
