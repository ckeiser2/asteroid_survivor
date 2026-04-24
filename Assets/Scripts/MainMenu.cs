using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using static PlayerController;

public class MainMenu : MonoBehaviour
{
    public UIDocument uiDocument;
    private Label highScoreText;
    private Button startButton;
    private Button optionsButton;
    private Button backButton;
    private Button muteButton;

    private bool isMuted = false;

    void Start()
    {
        var root = uiDocument.rootVisualElement;

        // Buttons
        startButton = root.Q<Button>("StartButton");
        optionsButton = root.Q<Button>("OptionsButton");
        backButton = root.Q<Button>("BackButton");
        muteButton = root.Q<Button>("MuteButton");
        highScoreText = root.Q<Label>("HighScoreLabel");

        float highScore = PlayerPrefs.GetFloat("HighScore", 0);
        highScoreText.text = "High Score: " + Mathf.FloorToInt(highScore);

        if (startButton == null || optionsButton == null || backButton == null || muteButton == null)
        {
            Debug.LogError("One or more buttons not found!");
            return;
        }

        // Initial state (main menu)
        SetMainMenuState();

        // Events
        startButton.clicked += StartGame;
        optionsButton.clicked += OpenOptions;
        backButton.clicked += CloseOptions;
        muteButton.clicked += ToggleAudio;
    }

    void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    void OpenOptions()
    {
        // Hide main menu buttons
        startButton.style.display = DisplayStyle.None;
        optionsButton.style.display = DisplayStyle.None;

        // Show options buttons
        backButton.style.display = DisplayStyle.Flex;
        muteButton.style.display = DisplayStyle.Flex;
    }

    void CloseOptions()
    {
        SetMainMenuState();
    }

    void SetMainMenuState()
    {
        // Show main menu buttons
        startButton.style.display = DisplayStyle.Flex;
        optionsButton.style.display = DisplayStyle.Flex;

        // Hide options buttons
        backButton.style.display = DisplayStyle.None;
        muteButton.style.display = DisplayStyle.None;
    }

    void ToggleAudio()
    {
        isMuted = !isMuted;

        AudioListener.volume = isMuted ? 0f : 1f;
        muteButton.text = isMuted ? "SFX: Unmute" : "SFX: Mute";
    }
}