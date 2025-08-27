// ResultTestHelper.cs  (attach anywhere in the scene)
using UnityEngine;

public class ResultTestHelper : MonoBehaviour
{
    public StationController controller;
    public StationInventory inventory;
    public ItemDefinition testResult;
    public int testCount = 1;
    public KeyCode hotkey = KeyCode.F6;

    void Update()
    {
        if (Input.GetKeyDown(hotkey))
        {
            if (!controller || !inventory || !testResult) { Debug.LogWarning("Assign controller, inventory, and testResult."); return; }
            inventory.AddToResult(testResult, Mathf.Max(1, testCount));
            // Put station into Completed so ResultGroup shows
            var method = controller.GetType().GetMethod("SaveState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            // set stage via a public API if you have one; otherwise:
            typeof(StationController).GetField("_stage", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(controller, StationStage.Completed);
            if (method != null) method.Invoke(controller, null);
            Debug.Log("Test result injected. Press F to focus and Collect.");
        }
    }
}
