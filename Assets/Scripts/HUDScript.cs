using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class HUDScript : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TMP_Text healthBarText;
    [SerializeField] private TMP_Text turnFactionLabel;

    public GameObject ActionLog;
    public TMP_Text ALText;

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

        UpdateHealth();
    }

    public void UpdateHealth()
    {
        healthBarText.text = string.Format("{0} / {1}", _selectedUnit.CurrentHealth, _selectedUnit.MaximumHealth);
        healthBarImage.fillAmount = (float)_selectedUnit.CurrentHealth / _selectedUnit.MaximumHealth;
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
}
