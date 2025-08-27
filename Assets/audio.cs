using UnityEngine;

public class StartVoiceOnce : MonoBehaviour
{
    [SerializeField] private AudioSource source; 
    [SerializeField] private AudioClip clip;     
    
    private static bool hasPlayed = false;

   
    public void PlayNow()
    {
        if (!hasPlayed && source != null && clip != null)
        {
            source.PlayOneShot(clip);
            hasPlayed = true;
        }
    }

    
    public static void ResetFlag() => hasPlayed = false;
}
