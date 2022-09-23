using Units;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Hack/Shield")]
    public class HackShieldAbility : AbilityBase
    {
        [SerializeField] int shieldCount;
        [SerializeField] int cooldownDuration;
        [SerializeField] int actionPointCost;
        [SerializeField, TextArea(5, 10)] private string description;

        public override int CooldownDuration => cooldownDuration;

        public override int ActionPointCost => actionPointCost;

        public override string ToolTipDescriptionString => description;

        public override string ToolTipCooldownString => $"Cooldown - {cooldownDuration}";

        public override string ToolTipAPCostString => $"AP Cost - {actionPointCost}";


        public override void Use(Unit owningUnit)
        {
            owningUnit.GainShields(shieldCount);
            owningUnit.ConsumeAP(actionPointCost);
            owningUnit.OnAbilityUsed(this);
        }
    }
}
