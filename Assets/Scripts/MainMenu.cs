using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject saveMenu;
    public GameObject settingsMenu;
    public GameObject creditsMenu;
    public GameObject exitMenu;

    private void Start()
    {
        GameManager.Instance.saveMenu = saveMenu;
        GameManager.Instance.settingsMenu = settingsMenu;
        GameManager.Instance.creditsMenu = creditsMenu;
        GameManager.Instance.exitMenu = exitMenu;
    }
}
