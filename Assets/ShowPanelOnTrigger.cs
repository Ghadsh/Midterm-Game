using UnityEngine;

public class ShowPanelOnTrigger : MonoBehaviour
{
    public GameObject panel; // «”Õ» «·»«‰Ì· Â‰« „‰ «·‹ Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // «··«⁄» ·«“„ ÌﬂÊ‰ ·Â Tag = Player
        {
            panel.SetActive(true); //  ŸÂ— «·»«‰Ì·
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            panel.SetActive(false); //  Œ ›Ì ·„« Ì»⁄œ
        }
    }
}