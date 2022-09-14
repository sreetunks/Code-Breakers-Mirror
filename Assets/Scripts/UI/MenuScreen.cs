using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MenuScreen : MonoBehaviour
{
    public delegate void OnScreenToggledEventHandler(bool visible);
    public OnScreenToggledEventHandler OnScreenToggled;
    public bool Visible => _visible;

    private CanvasGroup canvasGroup;
    private bool _visible;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        _visible = true;

        OnScreenToggled?.Invoke(true);
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        _visible = false;

        OnScreenToggled?.Invoke(false);
    }
}
