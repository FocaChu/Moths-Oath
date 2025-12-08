using MothsOath.Core.PassiveEffects;

namespace MothsOath.Core.Factories;

public class PassiveEffectFactory
{
    private readonly Dictionary<string, BasePassiveEffect> _passives;

    public PassiveEffectFactory()
    {
        _passives = new Dictionary<string, BasePassiveEffect>();
        var assembly = typeof(PassiveEffectFactory).Assembly;
        var passiveTypes = assembly.GetTypes()
            .Where(t => typeof(BasePassiveEffect).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        foreach (var type in passiveTypes)
        {
            try
            {
                if (Activator.CreateInstance(type) is BasePassiveEffect passive)
                {
                    if (!_passives.ContainsKey(passive.Id))
                    {
                        _passives.Add(passive.Id, passive);
                    }
                    else
                    {
                        Console.WriteLine($"[AVISO] Efeito passivo duplicado '{passive.Id}' ignorado.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Não foi possível instanciar '{type.FullName}': {ex.Message}");
            }
        }
        Console.WriteLine($"PassiveEffectFactory inicializado. {_passives.Count} efeitos passivos carregados.");
    }

    public List<BasePassiveEffect> GetPassiveEffects(List<string> passivesIds)
    {
        var result = new List<BasePassiveEffect>();
        foreach (var passiveId in passivesIds)
        {
            if (_passives.TryGetValue(passiveId, out var passive))
            {
                result.Add(passive);
            }
            else
            {
                Console.WriteLine($"[AVISO] Efeito passivo '{passiveId}' não encontrado. Adicionando efeito nulo.");
                result.Add(new NullPassiveEffect(passiveId));
            }
        }
        return result;
    }

    public BasePassiveEffect GetPassiveEffect(string passiveId)
    {
        if (_passives.TryGetValue(passiveId, out var passive))
        {
            return passive;
        }
        else
        {
            Console.WriteLine($"[AVISO] Efeito passivo '{passiveId}' não encontrado. Retornando efeito nulo.");
            return new NullPassiveEffect(passiveId);
        }
    }


}
public class NullPassiveEffect : BasePassiveEffect
{
    public override string Id { get; set; }

    public override string Name { get; set; }

    public override string Description { get; set; }

    public NullPassiveEffect(string missingId)
    {
        Id = missingId;
        Name = "Efeito Passivo Desconhecido";
        Description = "Este efeito passivo não pôde ser encontrado.";
    }
}