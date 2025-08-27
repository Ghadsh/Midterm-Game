//using System.Collections;
//using System.Collections.Genric;
//using UnityEngine;


//public enum SoundType
//{
//    WALKING,
//    PLANTS,
//    HARVEST,
//    LEVELUP,
//    SELL,
//    BACKGROUND,


//}

//[RequireComponent(typeof(AudioSource))]

//public class SoundManager : MonoBehaviour
//{
//    [SerializeField] private AudioClip[] soundList;
//    private static SoundManager instance;
//    private AudioSource audioSource;

//    private void Awake()
//    {

//    instance = this;

//    }

//    private void Start()
//    {
//        audioSource = GetComponent<AudioSource>();
//    }

//    public static PlaySound(SoundType sound, float volume = 1)
//    {

//        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
//    }
//}
