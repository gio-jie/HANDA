using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerLevel2 : MonoBehaviour
{
    public static GameManagerLevel2 Instance;

    [Header("Pause")]
    public GameObject pauseMenu;

    internal bool isPaused = false;
    public GameObject winPanel;
    public GameObject losePanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        Resume();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void ShowWinPanel()
    {
        Time.timeScale = 0f;
        winPanel.SetActive(true);
    }

    public void ShowLosePanel()
    {
        Time.timeScale = 0f;
        losePanel.SetActive(true);
    }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isPaused = true;

        Time.timeScale = 0f;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
            if (AudioManager.instance != null) AudioManager.instance.PauseBGM();
        }
    }

    public void Resume()
    {
        isPaused = false;

        Time.timeScale = 1f;

        AudioListener.pause = false;

        if (pauseMenu != null)
        {
            if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
        }
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("No more levels!");
        }
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
    }
}