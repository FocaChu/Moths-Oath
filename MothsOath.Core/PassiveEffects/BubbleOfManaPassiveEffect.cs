using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Death;
using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.PassiveEffects;

public class BubbleOfManaPassiveEffect : BasePassiveEffect, IDeathReactor
{
    public override string Id { get; set; } = "bubble_of_mana_passive";

    public override string Name { get; set; } = "Bolha de Mana";

    public int Priority { get; set; } = 1;

    public override string Description { get; set; } = "Restaura uma quantidade de mana do Personagem ao sair de cena.";

    public void OnDeath(ActionContext context, MortuaryPlan plan, BaseCharacter victim)
    {
        var player = context.GameState.Player;
        int manaRestored = 10;
        player.CurrentMana = Math.Min(player.CurrentMana + manaRestored, player.MaxMana);
    }
}
