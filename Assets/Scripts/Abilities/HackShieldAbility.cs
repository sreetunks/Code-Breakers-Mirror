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

        public override int CooldownDuration => cooldownDuration;

        public override int ActionPointCost => actionPointCost;

        public override void Use(Unit owningUnit)
        {
            owningUnit.GainShields(shieldCount);
            owningUnit.ConsumeAP(actionPointCost);
            owningUnit.OnAbilityUsed(this);
        }
    }
}
