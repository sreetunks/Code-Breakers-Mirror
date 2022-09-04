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
    public TMP_Text HealthNumber;
    public TMP_Text APNumber;
    //Turn Order List
    //Turn Order Icons

    // Start is called before the first frame update
    void Awake()
    {
        HUD = this;
        HackImageLeft.enabled = false;
        HackImageRight.enabled = false;
    }

    public void UpdateHack()
    {
        //When multiple hacks are avalible the center button can switch between them.
        if (gameManager.player.HackLearned)
        {
            HackImageLeft.enabled = true;
            HackImageRight.enabled = true;
        }
    }
}
