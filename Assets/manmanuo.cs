using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        // استبدلي "GameScene" باسم المشهد حق لعبتك
        SceneManager.LoadScene("Cutscene");
    }
}
