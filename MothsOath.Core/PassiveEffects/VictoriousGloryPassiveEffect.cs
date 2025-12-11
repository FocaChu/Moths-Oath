using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Death;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Entities;
using MothsOath.Core.StatusEffect.StatusEffectsRandomGenerators;

namespace MothsOath.Core.PassiveEffects;

public class VictoriousGloryPassiveEffect : BasePassiveEffect, IKillReactor
{
    public override string Id { get; set; } = "victorious_glory_passive";

    public override string Name { get; set; } = "Glória Vitoriosa";

    public override string Description { get; set; } = "Ao derrotar um inimigo concede uma glória aleatória a um aliado.";

    public int Priority { get; set; } = 2;

    public void OnKill(ActionContext context, MortuaryPlan plan, BaseCharacter victim)
    {
        var source = context.Source;    

        var level = (int)(source.Stats.TotalKnowledge / 2) > 1 ? (int)(source.Stats.TotalKnowledge / 2) : 2;

        var glory = RandomGloryGenerator.GenerateRandomGlory(level, 2);

        var targets = context.GameState.BuildPlayerTeam().Where(c => c.Id != source.Id).ToList();

        var rng = new Random();
        var target = targets[rng.Next(targets.Count)];

        target.ApplyPureStatusEffect(glory);
    }
}
