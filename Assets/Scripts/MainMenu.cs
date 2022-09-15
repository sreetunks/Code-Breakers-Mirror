using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
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
        GameManager.Instance.saveMenu = saveMenu;
        GameManager.Instance.creditsMenu = creditsMenu;
        GameManager.Instance.exitMenu = exitMenu;
    }
}
