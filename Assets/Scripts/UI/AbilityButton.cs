using Abilities;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text abilityNameText;
        [SerializeField] private CanvasGroup abilityCooldownCanvasGroup;
        [SerializeField] private TMP_Text abilityCooldownCount;
        [SerializeField] private Button abilityButton;

        [SerializeField] private CanvasGroup tooltipCanvasGroup;
        [SerializeField] private TMP_Text tooltipAbilityNameText;
        [SerializeField] private TMP_Text tooltipAbilityDescText;
        [SerializeField] private TMP_Text tooltipAbilityCooldownText;
        [SerializeField] private TMP_Text tooltipAbilityAPCostText;

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
            tooltipAbilityNameText.text = ability.name;
            tooltipAbilityDescText.text = ability.ToolTipDescriptionString;
            tooltipAbilityCooldownText.text = ability.ToolTipCooldownString;
            tooltipAbilityAPCostText.text = ability.ToolTipAPCostString;
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
        }

        public void UpdateCooldown()
        {
            var coolDownRemaining = PlayerScript.CurrentlySelectedUnit.GetAbilityCooldown(_abilityToTrigger);
            if (coolDownRemaining != 0)
            {
                DisableButton();
                abilityCooldownCount.text = coolDownRemaining.ToString();
            }
            else
            {
                EnableButton();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipCanvasGroup.alpha = 1;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipCanvasGroup.alpha = 0;
        }
    }
}
