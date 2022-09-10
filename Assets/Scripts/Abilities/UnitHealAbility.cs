using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Heal")]
public class UnitHealAbility : AbilityBase
{
    [SerializeField] int healAmount;
    [SerializeField] int cooldownDuration;
    [SerializeField] int actionPointCost;

    public override int CooldownDuration => cooldownDuration;

    public override int ActionPointCost => actionPointCost;

    public override void Use(Unit owningUnit)
    {
        owningUnit.Heal(healAmount);
        owningUnit.ConsumeAP(actionPointCost);
        owningUnit.OnAbilityUsed(this);
    }
}
