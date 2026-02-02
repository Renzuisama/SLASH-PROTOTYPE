using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button resumeButton;

    [Header("Settings")]
    [SerializeField] private KeyCode menuKey = KeyCode.Escape;
    [SerializeField] private bool pauseGameWhenOpen = true;

    private bool isMenuOpen = false;

    private void Start()
    {
        // Setup button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
        }

        // Hide menu at start
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Toggle menu with ESC key
        if (Input.GetKeyDown(menuKey))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (menuPanel != null)
        {
            menuPanel.SetActive(isMenuOpen);
        }

        if (pauseGameWhenOpen)
        {
            Time.timeScale = isMenuOpen ? 0f : 1f;
        }
    }

    public void OpenMenu()
    {
        isMenuOpen = true;

        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 0f;
        }
    }

    public void CloseMenu()
    {
        isMenuOpen = false;

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 1f;
        }
    }

    private void OnRestartClicked()
    {
        Debug.Log("Restart button clicked");

        // Reset time scale before reloading
        Time.timeScale = 1f;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit button clicked");

        // Reset time scale before quitting
        Time.timeScale = 1f;

#if UNITY_EDITOR
        // If running in editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running as build, quit application
        Application.Quit();
#endif
    }

    private void OnResumeClicked()
    {
        CloseMenu();
    }

    private void OnDestroy()
    {
        // Make sure time scale is reset when this object is destroyed
        Time.timeScale = 1f;
    }
}
