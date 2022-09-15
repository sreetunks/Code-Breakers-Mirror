using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject saveMenu;
    public GameObject creditsMenu;
    public GameObject exitMenu;

    [SerializeField] Button continueButton;
    [SerializeField] Button exitButton;

    private void Awake()
    {
#if UNITY_WEBPLAYER || UNITY_WEBGL
        exitButton.enabled = false;
#endif
    }

    private void Start()
    {
        continueButton.interactable = GameManager.Instance.HasSaveGame();

        GameManager.Instance.saveMenu = saveMenu;
        GameManager.Instance.creditsMenu = creditsMenu;
        GameManager.Instance.exitMenu = exitMenu;

        GameManager.Instance.settingsMenu.OnScreenToggled += OnSettingsScreenToggled;

        GameManager.Instance.SoundManager.mainMenuTheme.Play();
    }

    private void OnDestroy()
    {
        GameManager.Instance.settingsMenu.OnScreenToggled -= OnSettingsScreenToggled;
    }

    private void OnSettingsScreenToggled(bool visible)
    {
        if (!visible)
        {
            GameManager.Instance.SoundManager.mainMenuTheme.Play();

            if (GameManager.Instance.SoundManager.settingsTestSFX.mute == false)
            {
                GameManager.Instance.SoundManager.ToggleEffects();
            }

            GameManager.Instance.SoundManager.menuSFX.mute = true;
            GameManager.Instance.SoundManager.menuSFX.Stop();
        }
    }
}
