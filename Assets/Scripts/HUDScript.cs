using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class HUDScript : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TMP_Text healthBarText;
    [SerializeField] private Image actionPointBarImage;
    [SerializeField] private TMP_Text actionPointBarText;
    [SerializeField] private TMP_Text turnFactionLabel;

    [SerializeField] private CanvasGroup ingameHUDScreen;
    [SerializeField] private CanvasGroup levelCompleteScreen;
    [SerializeField] private CanvasGroup defeatedScreen;

    public GameObject ActionLog;
    public TMP_Text ALText;

    [SerializeField] private List<AbilityButton> abilityButtons;

    public bool DropDown;
    private Vector3 ActionLogOrigin;
    private List<string> ActionLogText;

    private Unit _selectedUnit;
    private Canvas _hudCanvas;

    void Awake()
    {
        _hudCanvas = GetComponent<Canvas>();

        DropDown = false;
        ActionLogOrigin = ActionLog.transform.localPosition;
        ActionLogText = new List<string>();
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
        {
            abilityButtons[i].Initialize(unit.AbilityList[i].ability);

            if (unit.Controller == PlayerScript.Instance)
                abilityButtons[i].EnableButton();
        }

        UpdateHealth();
        UpdateActionPoints();
    }

    public void UpdateHealth()
    {
        healthBarText.text = string.Format("{0} / {1}", _selectedUnit.CurrentHealth, _selectedUnit.MaximumHealth);
        healthBarImage.fillAmount = (float)_selectedUnit.CurrentHealth / _selectedUnit.MaximumHealth;
    }

    public void UpdateActionPoints()
    {
        actionPointBarText.text = string.Format("{0} / {1}", _selectedUnit.CurrentAP, _selectedUnit.MaximumAP);
        actionPointBarImage.fillAmount = (float)_selectedUnit.CurrentAP / _selectedUnit.MaximumAP;

        if (_selectedUnit.Controller != PlayerScript.Instance) return;

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

    public void UpdateTurnLabel(string labelString)
    {
        turnFactionLabel.text = string.Format("Current Turn: {0}", labelString);
    }

    public void ActionLogEvent(string LogMessage)
    {
        ALText.text = "";
        ActionLogText.Add(LogMessage);
        foreach(string msg in ActionLogText)
        {
            ALText.text += msg + "\n";
        }
        if(ActionLogText.Count > 10)
        {
            ActionLogText.RemoveAt(0);
        }
        //TODO Add small text box that displays the added message outside of the drop down -Atticus
    }

    public void ToggleLog()
    {
        var TempTransform = ActionLog.transform.localPosition;
        if (!DropDown)
        {
            TempTransform.y -= 510;
            ActionLog.transform.localPosition =  TempTransform;
        }
        else
        {
            TempTransform.y += 510;
            ActionLog.transform.localPosition = TempTransform;
        }
        DropDown = !DropDown;
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

    public void ReloadLevel()
    {
        GameManager.Instance.LoadGameScene();
    }

    public void LoadMainMenu()
    {
        GameManager.Instance.LoadMainMenuScene();
    }
}
