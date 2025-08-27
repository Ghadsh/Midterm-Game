using UnityEngine;

public class CleanableObject : MonoBehaviour
{
    public string interactionMessage = "Press F to clean";
    public bool isPlayerInRange = false;
    public int rewardAmount = 10; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            Clean();
        }
    }

    void Clean()
    {
        
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.AddMoney(rewardAmount);
        }

        
        Destroy(gameObject);
    }
}
