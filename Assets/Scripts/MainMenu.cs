using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject saveMenu;
    public GameObject exitMenu;

    [SerializeField] Button continueButton;
    [SerializeField] Button newGameButton;
    [SerializeField] Button exitButton;

    [SerializeField] AudioClip uiButtonClip;

    private void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        exitButton.enabled = false;
        exitButton.gameObject.SetActive(false);
#endif
    }

    private void Start()
    {
        if (GameManager.Instance.HasSaveGame())
        {
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);

            continueButton.interactable = true;

            var navigation = newGameButton.navigation;
            navigation.selectOnUp = continueButton;
            newGameButton.navigation = navigation;

            navigation = exitButton.navigation;
            navigation.selectOnDown = continueButton;
            exitButton.navigation = navigation;
        }

        GameManager.Instance.exitMenu = exitMenu;

        GameManager.Instance.SoundManager.PlayMenuMusic();
    }

    public void PlayUISound()
    {
        GameManager.Instance.SoundManager.PlayEffect(uiButtonClip);
    }
}
