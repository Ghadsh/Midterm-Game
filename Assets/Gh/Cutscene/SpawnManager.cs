using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform spawnPointStart;
    public Transform spawnPointLevel2;
    public GameObject player;

    void Start()
    {
        int spawnID = PlayerPrefs.GetInt("SpawnID", 0); 
        if (spawnID == 0)
        {
            player.transform.position = spawnPointStart.position;
            player.transform.rotation = spawnPointStart.rotation;
        }
        else
        {
            player.transform.position = spawnPointLevel2.position;
            player.transform.rotation = spawnPointLevel2.rotation;
        }
    }
}
