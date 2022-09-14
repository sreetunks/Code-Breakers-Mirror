using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioSource mainMenuSfx;

    public GameObject saveMenu;
    public GameObject creditsMenu;
    public GameObject exitMenu;

    [SerializeField] Button exitButton;

    private void Awake()
    {
#if UNITY_WEBPLAYER || UNITY_WEBGL
        exitButton.enabled = false;
#endif
    }

    private void Start()
    {
        GameManager.Instance.mainMenuSfx = mainMenuSfx;

        GameManager.Instance.saveMenu = saveMenu;
        GameManager.Instance.creditsMenu = creditsMenu;
        GameManager.Instance.exitMenu = exitMenu;

        GameManager.Instance.settingsMenu.OnScreenToggled += OnSettingsScreenToggled;

        GameManager.Instance.SoundManager.PlayMenuMusic();
    }

    private void OnDestroy()
    {
        GameManager.Instance.settingsMenu.OnScreenToggled -= OnSettingsScreenToggled;
    }

    private void OnSettingsScreenToggled(bool visible)
    {
        if (!visible)
        {
            GameManager.Instance.SoundManager.PlayMenuMusic();

            if (GameManager.Instance.SoundManager.effectSource.mute == false)
            {
                GameManager.Instance.SoundManager.ToggleEffects();
            }

            mainMenuSfx.mute = true;
            mainMenuSfx.Stop();
        }
    }
}
