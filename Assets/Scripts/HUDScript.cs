using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDScript : MonoBehaviour
{
    public static HUDScript HUD;

    public Image HPBar;
    public Image APBar;
    //Status List

    public Image Ability1Image;
    public Image Ability2Image;
    public Image HackImage;
    public Image Ability4Image;
    public Image Ability5Image;
    public Image HackImageLeft;
    public Image HackImageRight;
    public GameObject ActionLog;
    public TMP_Text ALText;
    public TMP_Text HealthNumber;
    public TMP_Text APNumber;

    public bool DropDown;
    private Vector3 ActionLogOrigin;
    //Turn Order List
    //Turn Order Icons

    private List<string> ActionLogText;
    // Start is called before the first frame update
    void Awake()
    {
        HUD = this;
        HackImageLeft.enabled = false;
        HackImageRight.enabled = false;
        DropDown = false;
        ActionLogOrigin = ActionLog.transform.localPosition;
        ActionLogText = new List<string>();
    }

    public void UpdateHack()
    {
        //When multiple hacks are avalible the center button can switch between them.
        if (GameManager.player.HackLearned)
        {
            HackImageLeft.enabled = true;
            HackImageRight.enabled = true;
        }
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
            TempTransform.y += -510;
            ActionLog.transform.localPosition =  TempTransform * 4 * Time.deltaTime;
        }
        else
        {
            ActionLog.transform.localPosition = ActionLogOrigin;
        }
        DropDown = !DropDown;
    }
}
