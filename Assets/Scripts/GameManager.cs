using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SoundManager SoundManager { get; private set; }

    public AudioSource mainMenuSfx;

    public GameObject saveMenu;
    public MenuScreen settingsMenu;
    public GameObject creditsMenu;
    public GameObject exitMenu;

    [SerializeField] AudioClip mainMenuMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SoundManager = GetComponent<SoundManager>();
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        SceneManager.sceneLoaded -= OnGameSceneLoaded;
        SoundManager.PlayInGameMusic();
    }

    public void ToggleSaveMenu()
    {
        saveMenu.SetActive(!saveMenu.activeInHierarchy);
        StartCoroutine(MenuSoundEffect());
    }

    public void EnableSettingsMenu()
    {
        settingsMenu.Show();
    }

    public void ToggleSettingsMenu()
    {
        if (settingsMenu.Visible)
            settingsMenu.Hide();
        else
            settingsMenu.Show();

        SoundManager.PauseMusic();
        StartCoroutine(MenuSoundEffect());
    }

    public void ToggleCreditsMenu()
    {
        creditsMenu.SetActive(!creditsMenu.activeInHierarchy);
        StartCoroutine(MenuSoundEffect());
    }

    public void ToggleExitMenu()
    {
        exitMenu.SetActive(!exitMenu.activeInHierarchy);
        StartCoroutine(MenuSoundEffect());
    }

    public void LoadNextGameScene()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene(scene.buildIndex + 1);
    }

    public void ReloadGameScene()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void LoadMainMenuScene() { SceneManager.LoadScene(0); }

    public IEnumerator MenuSoundEffect()
    {
        mainMenuSfx.mute = false;
        mainMenuSfx.Play();
        yield return new WaitForSeconds(0.25f);
        mainMenuSfx.mute = true;
        mainMenuSfx.Stop();
    }
}