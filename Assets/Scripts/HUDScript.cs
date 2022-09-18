using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UI;
using Units;

[RequireComponent(typeof(Canvas))]
public class HUDScript : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TMP_Text healthBarText;
    [SerializeField] private Image actionPointBarImage;
    [SerializeField] private TMP_Text actionPointBarText;
    [SerializeField] private Image shieldBarImage;
    [SerializeField] private TMP_Text turnFactionLabel;
    [SerializeField] private TMP_Text floorIndicator;
    [SerializeField] private Button endTurnButton;

    [SerializeField] private CanvasGroup ingameHUDScreen;
    [SerializeField] private CanvasGroup pauseScreen;
    [SerializeField] private CanvasGroup levelCompleteScreen;
    [SerializeField] private CanvasGroup defeatedScreen;

    public GameObject actionLog;
    public TMP_Text alText;

    [SerializeField] private List<AbilityButton> abilityButtons;

    public bool dropDown;
    private Vector3 _actionLogOrigin;
    private List<string> _actionLogText;

    private Unit _selectedUnit;
    private Canvas _hudCanvas;

    void Awake()
    {
        _hudCanvas = GetComponent<Canvas>();
        SetFloorNumber();
        dropDown = false;
        _actionLogOrigin = actionLog.transform.localPosition;
        _actionLogText = new List<string>();
    }

    public void Show() { _hudCanvas.enabled = true; }

    public void Hide() { _hudCanvas.enabled = false; }

    public void UpdateSelectedUnit(Unit unit)
    {
        _selectedUnit = unit;

        foreach (var abilityButton in abilityButtons)
        {
            abilityButton.DisableButton();
            abilityButton.ResetButton();
        }

        for (var i = 0; i < unit.AbilityList.Count; ++i)
            abilityButtons[i].Initialize(unit.AbilityList[i].ability);

        if (unit.Controller == PlayerScript.Instance && unit.Controller == TurnOrderSystem.ActiveController)
        {
            for (var i = 0; i < unit.AbilityList.Count; ++i)
            {
                abilityButtons[i].EnableButton();        
            }
                
            SetEndTurnButtonEnabled(true);
        }
        UpdateHealth();
        UpdateActionPoints();
    }

    public void UpdateHealth()
    {
        healthBarText.text = $"{_selectedUnit.CurrentHealth} / {_selectedUnit.MaximumHealth}";
        healthBarImage.fillAmount = (float)_selectedUnit.CurrentHealth / _selectedUnit.MaximumHealth;

        shieldBarImage.rectTransform.sizeDelta = new Vector2(25 * _selectedUnit.CurrentShields, shieldBarImage.rectTransform.sizeDelta.y);
    }

    public void UpdateActionPoints()
    {
        actionPointBarText.text = $"{_selectedUnit.CurrentAP} / {_selectedUnit.MaximumAP}";
        actionPointBarImage.fillAmount = (float)_selectedUnit.CurrentAP / _selectedUnit.MaximumAP;

        if (_selectedUnit.Controller != PlayerScript.Instance || _selectedUnit.Controller != TurnOrderSystem.ActiveController)
            return;

        for (var i = 0; i < _selectedUnit.AbilityList.Count; ++i)
        {
            if(_selectedUnit.GetAbilityCooldown(abilityButtons[i].Ability) > 0)
                abilityButtons[i].UpdateCooldown();

            if (_selectedUnit.GetAbilityCooldown(abilityButtons[i].Ability) == 0)
            {
                if(_selectedUnit.CurrentAP < abilityButtons[i].Ability.ActionPointCost)
                    abilityButtons[i].DisableButton();
                else
                    abilityButtons[i].EnableButton();
            }
        }
    }

    public void SetEndTurnButtonEnabled(bool enabled)
    {
        endTurnButton.interactable = enabled;
    }

    public void UpdateTurnLabel(string labelString)
    {
        turnFactionLabel.text = $"Current Turn: {labelString}";
    }

    public void SetFloorNumber()
    {
        floorIndicator.text = $"Floor: {SceneManager.GetActiveScene().name}";
    }
    public void ActionLogEvent(string logMessage)
    {
        alText.text = "";
        _actionLogText.Add(logMessage);
        foreach(string msg in _actionLogText)
        {
            alText.text += msg + "\n";
        }
        if(_actionLogText.Count > 10)
        {
            _actionLogText.RemoveAt(0);
        }
        //TODO Add small text box that displays the added message outside of the drop down -Atticus
    }

    public void ToggleLog()
    {
        var tempTransform = actionLog.transform.localPosition;
        if (!dropDown)
        {
            tempTransform.y -= 510;
            actionLog.transform.localPosition =  tempTransform;
        }
        else
        {
            tempTransform.y += 510;
            actionLog.transform.localPosition = tempTransform;
        }
        dropDown = !dropDown;
    }

    public void ShowPauseScreen()
    {
        pauseScreen.alpha = 1;
        pauseScreen.interactable = true;
        pauseScreen.blocksRaycasts = true;
        Time.timeScale = 0;

        ingameHUDScreen.alpha = 0;
        ingameHUDScreen.interactable = false;
        ingameHUDScreen.blocksRaycasts = false;
    }

    public void ToggleSettingsMenu()
    {
        GameManager.Instance.EnableSettingsMenu();
    }

    public void ShowLevelCompleteScreen()
    {
        levelCompleteScreen.alpha = 1;
        levelCompleteScreen.interactable = true;
        levelCompleteScreen.blocksRaycasts = true;

        ingameHUDScreen.alpha = 0;
        ingameHUDScreen.interactable = false;
        ingameHUDScreen.blocksRaycasts = false;
    }

    public void ShowDefeatedScreen()
    {
        defeatedScreen.alpha = 1;
        defeatedScreen.interactable = true;
        defeatedScreen.blocksRaycasts = true;

        ingameHUDScreen.alpha = 0;
        ingameHUDScreen.interactable = false;
        ingameHUDScreen.blocksRaycasts = false;
    }

    public void ResumeLevel()
    {
        pauseScreen.alpha = 0;
        pauseScreen.interactable = false;
        pauseScreen.blocksRaycasts = false;
        Time.timeScale = 1;

        ingameHUDScreen.alpha = 1;
        ingameHUDScreen.interactable = true;
        ingameHUDScreen.blocksRaycasts = true;
    }

    public void Continue()
    {
        GameManager.Instance.LoadNextGameScene();
    }

    public void ReloadLevel()
    {
        GameManager.Instance.ReloadGameScene();
    }

    public void LoadMainMenu()
    {
        GameManager.Instance.LoadMainMenuScene();
    }
}
