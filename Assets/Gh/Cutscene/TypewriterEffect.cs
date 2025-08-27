//using UnityEngine;
//using TMPro;
//using System.Collections;

//public class TypewriterEffect : MonoBehaviour
//{
//    public TMP_Text textMeshPro;
//    [TextArea] public string fullText;
//    public float delay = 0.2f; 

//    void OnEnable()
//    {

//        StartCoroutine(ShowTextWordByWord());
//    }

//    IEnumerator ShowTextWordByWord()
//    {
//        textMeshPro.text = "";
//        string[] words = fullText.Split(' ');

//        for (int i = 0; i < words.Length; i++)
//        {
//            if (i > 0) textMeshPro.text += " ";
//            textMeshPro.text += words[i];
//            yield return new WaitForSeconds(delay);
//        }
//    }

//    void Awake()
//    {

//        if (textMeshPro != null)
//            textMeshPro.text = "";
//    }
//}

using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    public TMP_Text textMeshPro;
    [TextArea] public string fullText;
    public float delay = 0.25f;

    public AudioSource audioSource;
    public AudioClip typingClip;

    void OnEnable()
    {
        StartCoroutine(ShowTextWordByWord());
    }

    IEnumerator ShowTextWordByWord()
    {
        textMeshPro.text = "";
        string[] words = fullText.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            if (i > 0) textMeshPro.text += " ";
            textMeshPro.text += words[i];


            if (typingClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(typingClip);
            }

            yield return new WaitForSeconds(delay);
        }
    }

    void Awake()
    {
        if (textMeshPro != null)
            textMeshPro.text = "";
    }
}

