using MothsOath.Core.Abilities;
using MothsOath.Core.Common;

namespace MothsOath.Core.Factories;

public class ActionFactory
{
    private readonly Dictionary<string, BaseAction> _abilities;

    public ActionFactory()
    {
        _abilities = new Dictionary<string, BaseAction>();

        var assembly = typeof(ActionFactory).Assembly;
        var actionTypes = assembly.GetTypes()
            .Where(t => typeof(BaseAction).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in actionTypes)
        {
            try
            {
                if (Activator.CreateInstance(type) is BaseAction action)
                {
                    if (!_abilities.ContainsKey(action.Id))
                    {
                        _abilities.Add(action.Id, action);
                    }
                    else
                    {
                        Console.WriteLine($"[AVISO] Habilidade duplicada '{action.Id}' ignorada.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Não foi possível instanciar '{type.FullName}': {ex.Message}");
            }
        }

        Console.WriteLine($"AbilityFactory inicializado. {_abilities.Count} habilidades carregadas.");
    }

    public BaseAction GetAbility(string abilityId)
    {
        if (_abilities.TryGetValue(abilityId, out var ability))
        {
            return ability;
        }

        Console.WriteLine($"[ERRO CRÍTICO] Habilidade com ID '{abilityId}' não encontrada no AbilityFactory!");

        return new NullAbility(abilityId);
    }
}

public class NullAbility : BaseAction
{
    public override string Id { get; }


    public NullAbility(string missingId)
    {
        Id = $"missing_ability_{missingId}";
    }


    public override void Execute(ActionContext context)
    {
        Console.WriteLine($"[AVISO] Tentativa de executar uma habilidade não encontrada com ID '{Id}'. Nenhuma ação foi tomada.");
    }
}