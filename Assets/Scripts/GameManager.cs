using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SoundManager SoundManager { get; private set; }

    public GameObject saveMenu;
    public GameObject settingsMenu;
    public GameObject creditsMenu;
    public GameObject exitMenu;

    public bool isPaused = false;

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

    public void ToggleSaveMenu()
    {
        saveMenu.SetActive(!saveMenu.activeInHierarchy);
        StartCoroutine(MenuSoundEffect());
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeInHierarchy);
        SoundManager.mainMenuTheme.Stop();
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

    public void LoadGameScene() { SceneManager.LoadScene(1); }

    public void LoadMainMenuScene() { SceneManager.LoadScene(0); }

    public IEnumerator MenuSoundEffect()
    {
        SoundManager.menuSFX.mute = false;
        SoundManager.menuSFX.Play();
        yield return new WaitForSeconds(0.25f);
        SoundManager.menuSFX.mute = true;
        SoundManager.menuSFX.Stop();
    }
}