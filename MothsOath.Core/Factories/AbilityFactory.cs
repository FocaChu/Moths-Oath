using MothsOath.Core.Abilities;
using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Factories;

public class AbilityFactory
{
    private readonly Dictionary<string, IAbility> _abilities;

    public AbilityFactory()
    {
        _abilities = new Dictionary<string, IAbility>();

        var basicAttack = new BasicAttackAbility();
        _abilities.Add(basicAttack.Id, basicAttack);

        var powerStrike = new PowerStrikeAbility();
        _abilities.Add(powerStrike.Id, powerStrike);

        Console.WriteLine($"AbilityFactory inicializado. {_abilities.Count} habilidades carregadas.");
    }

    public IAbility GetAbility(string abilityId)
    {
        if (_abilities.TryGetValue(abilityId, out var ability))
        {
            return ability;
        }

        Console.WriteLine($"[ERRO CRÍTICO] Habilidade com ID '{abilityId}' não encontrada no AbilityFactory!");

        return new NullAbility(abilityId);
    }
}

public class NullAbility : IAbility
{
    public string Id { get; }

    public NullAbility(string missingId)
    {
        Id = $"missing_ability_{missingId}";
    }

    public void Execute(Character source, Character target, CombatState gameState)
    {
        Console.WriteLine($"[AVISO] Tentativa de executar uma habilidade não encontrada com ID '{Id}'. Nenhuma ação foi tomada.");
    }
}