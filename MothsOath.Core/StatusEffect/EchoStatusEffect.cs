using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect;

public class EchoStatusEffect : BaseStatusEffect, ITurnStartReactor
{
    public override string Id { get; set; } = "echo_effect";

    public override string Name { get; set; } = "echo";

    public override string Description { get; set; } = "Um eco de alguma coisa...";

    public override bool IsEndless { get; set; } = true;

    public override bool IsEchoable { get; set; } = false;

    public override bool IsVisible { get; set; } = true;

    public int Priority { get; set; } = 0;

    public override StatusEffectType EffectType { get; set; } = StatusEffectType.Neutral;

    public BaseStatusEffect _echoedEffect { get; set; }

    public EchoStatusEffect(BaseStatusEffect echoedEffect)
    {
        _echoedEffect = echoedEffect.Clone();
        Id = $"echo_of_{echoedEffect.Id}";
        Name = $"Eco de {_echoedEffect.Name}";
        Description = $"Um eco de um {_echoedEffect.Name}.";
        Level = echoedEffect.Level;
        Duration = echoedEffect.Duration;
    }

    public override void TickTime(BaseCharacter holder)
    {
        return;
    }

    public override void StackEffect(BaseCharacter owner, BaseStatusEffect newEffect)
    {
        if(newEffect is EchoStatusEffect echo)
        {
            _echoedEffect.StackEffect(owner, echo._echoedEffect);
            this.Level = _echoedEffect.Level;
            this.Duration = _echoedEffect.Duration;
        }
    }

    public void OnTurnStart(BaseCharacter target, CombatState context)
    {
        if (target.StatusEffects.Any(s => s.Id == _echoedEffect.Id))
            return;

        this.Duration = 0;
        this.IsEndless = false;
        target.ApplyPureStatusEffect(_echoedEffect.EchoEffect());
    }
}
