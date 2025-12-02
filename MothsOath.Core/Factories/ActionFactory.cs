using MothsOath.Core.Abilities;
using MothsOath.Core.Common;

namespace MothsOath.Core.Factories;

public class ActionFactory
{
    private readonly Dictionary<string, IAction> _abilities;

    public ActionFactory()
    {
        _abilities = new Dictionary<string, IAction>();

        var basicAttack = new BasicAttackAbility();
        _abilities.Add(basicAttack.Id, basicAttack);

        var powerStrike = new PowerStrikeAbility();
        _abilities.Add(powerStrike.Id, powerStrike);

        Console.WriteLine($"AbilityFactory inicializado. {_abilities.Count} habilidades carregadas.");
    }

    public IAction GetAbility(string abilityId)
    {
        if (_abilities.TryGetValue(abilityId, out var ability))
        {
            return ability;
        }

        Console.WriteLine($"[ERRO CRÍTICO] Habilidade com ID '{abilityId}' não encontrada no AbilityFactory!");

        return new NullAbility(abilityId);
    }
}

public class NullAbility : IAction
{
    public string Id { get; }

    public NullAbility(string missingId)
    {
        Id = $"missing_ability_{missingId}";
    }

    public void Execute(ActionContext context)
    {
        Console.WriteLine($"[AVISO] Tentativa de executar uma habilidade não encontrada com ID '{Id}'. Nenhuma ação foi tomada.");
    }
}