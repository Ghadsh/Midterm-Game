using UnityEngine;

[CreateAssetMenu(menuName = "LanternLight/Input Config")]
public class InputConfig : ScriptableObject
{
    public KeyCode pickKey = KeyCode.E;        // pick/drop
    public KeyCode splitKey = KeyCode.F;        // split blob
    public KeyCode pourKeyHold = KeyCode.Mouse0;// hold to pour
}
