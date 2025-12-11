using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Death;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.PassiveEffects;

public class CleaveDamagePassiveEffect : BasePassiveEffect, IKillReactor
{
    public override string Id { get; set; } = "cleave_damage_passive";

    public override string Name { get; set; } = "Dano em Varrimento";

    public override string Description { get; set; } = "Dano excedente em ataques é distribuído entre inimigos próximos.";

    public int Priority { get; set; } = 1;

    public void OnKill(ActionContext context, MortuaryPlan plan, BaseCharacter target)
    {
        if (plan.ExcessDamage <= 0 || target.Allegiance == context.Source.Allegiance)
            return;

        if (target.Allegiance == Allegiance.Enemy)
        {
            var enemies = context.GameState.Enemies;

            if (enemies.Count <= 1)
                return;

            var targetIndex = enemies.IndexOf(target);

            if (targetIndex == -1)
                return;

            var adjacentTargets = new List<BaseCharacter>();

            if (targetIndex > 0)
            {
                var leftNeighborIndex = targetIndex - 1;
                var leftNeighbor = enemies[leftNeighborIndex];

                if(leftNeighbor.Stats.IsAlive)
                    adjacentTargets.Add(leftNeighbor);
            }

            if (targetIndex < enemies.Count - 1)
            {
                var rightNeighborIndex = targetIndex + 1;
                var rightNeighbor = enemies[rightNeighborIndex];

                if(rightNeighbor.Stats.IsAlive)
                    adjacentTargets.Add(rightNeighbor);
            }

            if (adjacentTargets.Count == 0)
                return;

            var random = new Random();
            var randomIndex = random.Next(0, adjacentTargets.Count); 
            var newTarget = adjacentTargets[randomIndex];

            var newContext = new ActionContext(context.Source, new List<BaseCharacter> { newTarget }, context.GameState, null);
            var damagePlan = new HealthModifierPlan(plan.ExcessDamage, HealthModifierType.Damage);

            newTarget.HandleHealthModifier(newContext, damagePlan);
        }
    }
}
