using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnTrigger : MonoBehaviour
{
    public string sceneToLoad = "NextScene"; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player reached spawn point. Loading scene...");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
