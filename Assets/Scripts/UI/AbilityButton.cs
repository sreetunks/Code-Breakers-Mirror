using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityButton : MonoBehaviour
{
    [SerializeField] private TMP_Text abilityNameText;
    [SerializeField] private CanvasGroup abilityCooldownCanvasGroup;
    [SerializeField] private TMP_Text abilityCooldownCount;
    [SerializeField] private Button abilityButton;

    private AbilityBase _abilityToTrigger;

    public AbilityBase Ability => _abilityToTrigger;

    public void ResetButton()
    {
        gameObject.SetActive(false);
    }

    public void Initialize(AbilityBase ability)
    {
        gameObject.SetActive(true);
        _abilityToTrigger = ability;
        abilityNameText.text = ability.name;
    }

    public void EnableButton()
    {
        abilityCooldownCanvasGroup.alpha = 0;
        abilityButton.interactable = true;
    }

    public void DisableButton()
    {
        abilityCooldownCount.text = "";
        abilityCooldownCanvasGroup.alpha = 1;
        abilityButton.interactable = false;
    }

    public void TriggerAbility()
    {
        _abilityToTrigger.Use(PlayerScript.CurrentlySelectedUnit);
        PlayerScript.CurrentlySelectedUnit.OnAbilityUsed(_abilityToTrigger);
    }

    public void UpdateCooldown()
    {
        int coolDownRemaining = PlayerScript.CurrentlySelectedUnit.GetAbilityCooldown(_abilityToTrigger);
        if (coolDownRemaining != 0)
        {
            DisableButton();
            abilityCooldownCount.text = coolDownRemaining--.ToString();
        }
        else
        {
            EnableButton();
        }
    }
}
