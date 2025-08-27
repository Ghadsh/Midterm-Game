using UnityEngine;

[CreateAssetMenu(menuName = "LanternLight/Interaction Config")]
public class InteractionConfig : ScriptableObject
{
    [Header("Raycast")]
    public float pickDistance = 4f;
    public LayerMask interactMask;

    [Header("Hold Point")]
    public Vector3 holdLocalPosition = new Vector3(0f, -0.05f, 0.35f);
    public Vector3 holdLocalEuler = Vector3.zero;
}
