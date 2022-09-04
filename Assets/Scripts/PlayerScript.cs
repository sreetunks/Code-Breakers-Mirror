using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : EntityScript
{
    bool summon = false;
    public bool HackLearned;
    [SerializeField] int HealAPConsume;
    [SerializeField] int HealAbilityHP;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateHUD()
    {
        if (!summon)
        {
            HUDScript.HUD.HPBar.fillAmount = HP / HPMax;
            HUDScript.HUD.APBar.fillAmount = AP / APMax;
            HUDScript.HUD.HealthNumber.text = HP + "/" + HPMax;
            HUDScript.HUD.APNumber.text = AP + "/" + APMax;
            //Needs Status effects to update.
        }
        //Will update hud when hud variables are done.
    }

    public void HealAction()
    {
        if(AP >= HealAPConsume)
        {
            Heal(HealAbilityHP);
        }
    }

    public void Dash()
    {

    }
}
