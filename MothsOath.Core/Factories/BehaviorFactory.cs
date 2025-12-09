using System.Reflection;
using MothsOath.Core.Behaviors;
using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Factories;

public class BehaviorFactory
{
    private readonly Dictionary<string, IBehavior> _behaviors;

    public BehaviorFactory()
    {
        _behaviors = new Dictionary<string, IBehavior>();

        RegisterBehaviorsFromAssembly(typeof(BehaviorFactory).Assembly);

        Console.WriteLine($"BehaviorFactory initialized. {_behaviors.Count} behaviors loaded.");
    }

    public IBehavior GetBehavior(string behaviorId)
    {
        if (_behaviors.TryGetValue(behaviorId, out var behavior))
        {
            return behavior;
        }
        Console.WriteLine($"[CRITICAL ERROR] Behavior with ID '{behaviorId}' not found in BehaviorFactory!");
        return new NullBehavior(behaviorId);
    }

    private void RegisterBehaviorsFromAssembly(Assembly assembly)
    {
        if (assembly == null)
        {
            Console.WriteLine("[ERROR] Assembly nula fornecida para RegisterBehaviorsFromAssembly.");
            return;
        }

        var behaviorTypes = assembly
            .GetTypes()
            .Where(t => typeof(IBehavior).IsAssignableFrom(t) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null);

        foreach (var type in behaviorTypes)
        {
            try
            {
                if (Activator.CreateInstance(type) is not IBehavior instance)
                {
                    Console.WriteLine($"[WARN] Tipo {type.FullName} não pôde ser convertido para IBehavior após instanciação.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(instance.Id))
                {
                    Console.WriteLine($"[WARN] Comportamento {type.FullName} possui Id nulo ou vazio. Ignorando.");
                    continue;
                }

                if (_behaviors.ContainsKey(instance.Id))
                {
                    Console.WriteLine($"[WARN] Comportamento com Id '{instance.Id}' já registrado. Tipo atual: {type.FullName}. Ignorando duplicata.");
                    continue;
                }

                _behaviors.Add(instance.Id, instance);
                Console.WriteLine($"[INFO] Registered behavior: {instance.Id} ({type.FullName})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Falha ao instanciar comportamento do tipo {type.FullName}: {ex.Message}");
            }
        }
    }
}

public class NullBehavior : IBehavior
{
    public string Id { get; }

    public NullBehavior(string missingId)
    {
        Id = $"missing_ability_{missingId}";
    }

    public List<BaseCharacter> GetTargets(BaseCharacter source, CombatState context)
    {
        Console.WriteLine($"[AVISO] Tentativa de executar um comportamento não encontrado com ID '{Id}'. Nenhuma ação foi tomada.");
        return new List<BaseCharacter>();
    }
}