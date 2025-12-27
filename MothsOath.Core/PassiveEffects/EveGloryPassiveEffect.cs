using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Common.EffectInterfaces.StatusEffect;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Entities;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect;
using MothsOath.Core.StatusEffect.StatusEffectsRandomGenerators;

namespace MothsOath.Core.PassiveEffects;

public class EveGloryPassiveEffect : BasePassiveEffect, ICombatStartReactor, IStatusEffectDoneReactor, ITurnStartReactor, ICombatEndReactor
{
    public override string Id { get; set; } = "eve_glory_passive";

    public override string Name { get; set; } = "Véspera de Glórias";

    public override string Description { get; set; } = "Concede uma glória ao começo de um combate.";

    public int Priority { get; set; } = 0;

    public int Counter { get; set; } = 0;

    public void OnCombatStart(BaseCharacter target, CombatState context)
    {
        var level = target is Player player ? player.Level : 1;

        var glory = RandomGloryGenerator.GenerateRandomGlory(level, 2);

        target.ApplyPureStatusEffect(glory);

        Console.WriteLine($"{target.Name} recebeu a glória {glory.Name} no começo do combate.");
    }


    public void OnCombatEnd(CombatState state, BaseCharacter source)
    {
        Counter = 0;
        Detransform(source);
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

        if(!target.IsTransformed)
            Counter++;

        Console.WriteLine($"{target.Name} recebeu um eco de {plan.StatusEffect.Name}.");
    }

    public void OnTurnStart(BaseCharacter target, CombatState context)
    {
        if (target.IsTransformed && Counter > 0)
            Counter--;

        if(target.IsTransformed && Counter == 0)
            Detransform(target);

        if (Counter >= 5)
        {
            Transform(context, target);
            Counter = 3;;
        }
    }

    private void Transform(CombatState context, BaseCharacter target)
    {
        target.IsTransformed = true;

        if (target is Player yulkin)
        {
            yulkin.Stats.BonusMaxHealth += 20;
            yulkin.Stats.BonusStrength += yulkin.Level;
            yulkin.Stats.BonusKnowledge += yulkin.Level + 1;
            yulkin.Stats.BonusDefense += 3;
            yulkin.Stats.BonusCriticalChance += 3;
            yulkin.Stats.Shield += 15;
        }
        else
        {
            target.Stats.BonusStrength += 3;
            target.Stats.BonusKnowledge += 3;
            target.Stats.BonusDefense += 1;
            target.Stats.BonusCriticalChance += 3;

        }

        target.ReceivePureHeal(target.Stats.Regeneration);

        var team = context.PlayerTeam;
        foreach (var ally in team)
        {
            var level = ally is Player player ? player.Level : 2;

            var glory = RandomGloryGenerator.GenerateRandomGlory(level, 2);
            ally.ApplyPureStatusEffect(glory);
            Console.WriteLine($"{ally.Name} recebeu a glória {glory.Name}.");
        }

        Console.WriteLine($"{target.Name} se transformou devido a {Name}!");
    }

    private void Detransform(BaseCharacter target)
    {
        target.IsTransformed = false;
        if (target is Player yulkin)
        {
            yulkin.Stats.BonusMaxHealth -= 20;
            yulkin.Stats.BonusStrength -= yulkin.Level;
            yulkin.Stats.BonusKnowledge -= yulkin.Level + 1;
            yulkin.Stats.BonusDefense -= 3;
            yulkin.Stats.BonusCriticalChance -= 3;
        }
        else
        {
            target.Stats.BonusStrength -= 3;
            target.Stats.BonusKnowledge -= 3;
            target.Stats.BonusDefense -= 1;
            target.Stats.BonusCriticalChance -= 3;
        }
        Console.WriteLine($"{target.Name} retornou à sua forma normal.");
    }
}
