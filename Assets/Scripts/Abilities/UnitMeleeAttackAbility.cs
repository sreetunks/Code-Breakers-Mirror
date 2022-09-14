using Units;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Melee Attack")]
    public class UnitMeleeAttackAbility : AbilityBase
    {
        [SerializeField] int attackDamage;
        [SerializeField] int cooldownDuration;
        [SerializeField] int actionPointCost;

        public override int CooldownDuration => cooldownDuration;

        public override int ActionPointCost => actionPointCost;

        public override void Use(Unit owningUnit)
        {
            owningUnit.MeleeAttack(attackDamage);
            owningUnit.ConsumeAP(actionPointCost);
            owningUnit.OnAbilityUsed(this);
        }
    }
}