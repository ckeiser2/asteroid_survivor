using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    void Start()
    {
    if (SceneManager.GetActiveScene().name == "Menu")
        Time.timeScale = 0f;
    else
        Time.timeScale = 1f;
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            Time.timeScale = 0f; // Pause game
        }
        else
        {
            Time.timeScale = 1f; // Resume game
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f; // resume time
        SceneManager.LoadScene("GameScene");
    }
}