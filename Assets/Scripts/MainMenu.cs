using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public AudioSource mainMenuTheme;
    public AudioSource mainMenuSfx;

    public GameObject saveMenu;
    public GameObject creditsMenu;
    public GameObject exitMenu;

    private void Start()
    {
        GameManager.Instance.mainMenuTheme = mainMenuTheme;
        GameManager.Instance.mainMenuSfx = mainMenuSfx;

        GameManager.Instance.saveMenu = saveMenu;
        GameManager.Instance.creditsMenu = creditsMenu;
        GameManager.Instance.exitMenu = exitMenu;
    }
}
