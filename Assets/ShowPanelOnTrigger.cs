using UnityEngine;

public class ShowPanelOnTrigger : MonoBehaviour
{
    public GameObject panel; // ���� ������� ��� �� ��� Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ������ ���� ���� �� Tag = Player
        {
            panel.SetActive(true); // ���� �������
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            panel.SetActive(false); // ����� ��� ����
        }
    }
}