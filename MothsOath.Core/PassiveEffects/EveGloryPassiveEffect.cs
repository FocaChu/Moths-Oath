using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.Common.EffectInterfaces.StatusEffect;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Entities;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect;
using MothsOath.Core.StatusEffect.StatusEffectsRandomGenerators;

namespace MothsOath.Core.PassiveEffects;

public class EveGloryPassiveEffect : BasePassiveEffect, ICombatStartReactor, IStatusEffectDoneReactor
{
    public override string Id { get; set; } = "eve_glory_passive";

    public override string Name { get; set; } = "Véspera de Glórias";

    public override string Description { get; set; } = "Concede uma glória ao começo de um combate.";

    public int Priority { get; set; } = 0;

    public void OnCombatStart(BaseCharacter target, CombatState context)
    {
        var level = target is Player player ? player.Level : 1;

        var glory = RandomGloryGenerator.GenerateRandomGlory(level, 2);

        target.ApplyPureStatusEffect(glory);

        Console.WriteLine($"{target.Name} recebeu a glória {glory.Name} no começo do combate.");
    }

    public void OnStatusEffectDone(ActionContext context, StatusEffectPlan plan, BaseCharacter target)
    {
        if (plan.StatusEffect is EchoStatusEffect || !plan.StatusEffect.IsEchoable)
            return;

        if (target == context.Source)
            return;

        var effect = plan.StatusEffect.Clone();
        effect.Level = 1;
        effect.Duration = 1;

        var echo = new EchoStatusEffect(effect);

        target.ApplyPureStatusEffect(echo);

        Console.WriteLine($"{target.Name} recebeu um eco de {plan.StatusEffect.Name}.");

    }
}
