using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] public GameObject saveMenu;
    [SerializeField] public GameObject settingsMenu;
    [SerializeField] public GameObject creditsMenu;
    [SerializeField] public GameObject exitMenu;
    [SerializeField] public AudioSource mainMenuTheme;
    [SerializeField] public AudioSource mainMenuSFX;

    public static PlayerScript player;
    public bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        player = new PlayerScript();
    }

    public void ToggleSaveMenu()
    {
        saveMenu.SetActive(!saveMenu.activeInHierarchy);
        StartCoroutine(MenuSoundEffect());
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeInHierarchy);
        mainMenuTheme.Stop();
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

    public IEnumerator MenuSoundEffect()
    {
        mainMenuSFX.mute = false;
        mainMenuSFX.Play();
        yield return new WaitForSeconds(0.25f);
        mainMenuSFX.mute = true;
        mainMenuSFX.Stop();
    }
}