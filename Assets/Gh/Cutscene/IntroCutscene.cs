using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text loanText;
    public CanvasGroup fadeImage;

    [Header("Camera")]
    public Camera mainCamera;
    public Transform[] cameraPoints;
    public float panDuration = 3f;

    [Header("Player")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    [Header("Game Settings")]
    public int startingCash = 1000;
    public int startingLoan = 100000;

    private bool cutsceneSkipped = false;

    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cutsceneSkipped = true;
        }
    }

    IEnumerator PlayCutscene()
    {
        yield return StartCoroutine(FadeIn());


        loanText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < cameraPoints.Length; i++)
        {
            if (cutsceneSkipped) break;

            yield return StartCoroutine(PanTo(cameraPoints[i]));
            yield return new WaitForSeconds(0.5f);
        }

        loanText.gameObject.SetActive(false);
        yield return StartCoroutine(FadeOut());

        yield return StartCoroutine(SpawnPlayer());

        yield return StartCoroutine(FadeIn());
    }


    IEnumerator PanTo(Transform target)
    {
        Vector3 startPos = mainCamera.transform.position;
        Vector3 endPos = target.position;
        float elapsed = 0f;

        while (elapsed < panDuration)
        {
            if (cutsceneSkipped) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / panDuration;
            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        mainCamera.transform.position = endPos;
    }

    IEnumerator FadeIn()
    {
        fadeImage.alpha = 1f;
        fadeImage.gameObject.SetActive(true);

        while (fadeImage.alpha > 0f)
        {
            fadeImage.alpha -= Time.deltaTime;
            yield return null;
        }

        fadeImage.alpha = 0f;
        fadeImage.gameObject.SetActive(false);
    }

    IEnumerator FadeOut()
    {
        fadeImage.alpha = 0f;
        fadeImage.gameObject.SetActive(true);

        while (fadeImage.alpha < 1f)
        {
            fadeImage.alpha += Time.deltaTime;
            yield return null;
        }

        fadeImage.alpha = 1f;
    }

    //void SpawnPlayer()
    //{

    //    if (GameObject.FindWithTag("Player") != null)
    //    {
    //        Debug.Log("Player already exists. Skipping spawn.");
    //        return;
    //    }

    //    GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
    //    player.tag = "Player"; 

    //    PlayerStats.Cash = startingCash;
    //    PlayerStats.LoanRemaining = startingLoan;
    //}


    IEnumerator SpawnPlayer()

    {
        if (GameObject.FindWithTag("Player") != null)
        {
            Debug.Log("Player already exists. Skipping spawn.");
            yield break;
        }

        GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        player.tag = "Player";

        PlayerStats.Cash = startingCash;
        PlayerStats.LoanRemaining = startingLoan;

        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(2);
    }
}
