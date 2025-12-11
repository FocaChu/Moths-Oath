using MothsOath.Core.Behaviors;
using MothsOath.Core.Common;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect.DiseaseEffect;
using MothsOath.Core.StatusEffect.DiseaseEffect.Symptoms;

namespace MothsOath.Core.Entities.Archetypes;

public class Doctor : Player
{
    public DiseaseEffect Disease { get; set; }

    public Doctor(Player player, DiseaseEffect? disease = null)
    {
        this.Id = player.Id;

        this.Name = player.Name;
        this.Archetype = player.Archetype;
        this.Race = player.Race;
        this.Allegiance = player.Allegiance;

        var s = player.Stats;
        this.Stats = new Stats
        {
            MaxHealth = s.MaxHealth,
            CurrentHealth = s.CurrentHealth,
            BaseStrength = s.BaseStrength,
            BaseKnowledge = s.BaseKnowledge,
            BaseDefense = s.BaseDefense,
            BaseCriticalChance = s.BaseCriticalChance,
            BaseCriticalDamageMultiplier = s.BaseCriticalDamageMultiplier,
            Shield = s.Shield,
            Regeneration = s.Regeneration
        };

        this.MaxMana = player.MaxMana;
        this.CurrentMana = player.CurrentMana;
        this.MaxStamina = player.MaxStamina;
        this.CurrentStamina = player.CurrentStamina;
        this.Gold = player.Gold;
        this.XpMultiplier = player.XpMultiplier;
        this.CardsByTurn = player.CardsByTurn;

        this.PassiveEffects = player.PassiveEffects.ToList();
        this.StatusEffects = player.StatusEffects.ToList();

        this.Deck.Clear();
        this.Deck.AddRange(player.Deck);

        this.Hand.Clear();
        this.Hand.AddRange(player.Hand);

        this.DiscartPile.Clear();
        this.DiscartPile.AddRange(player.DiscartPile);

        Disease = disease ?? new DiseaseEffect("unknown_disease", "Nenhuma", "Sem doença atribuída.", false, new TargetAllBehavior(), new List<BaseSymptomEffect>(), new List<BaseSymptomEffect>());
    }

    public override void OnTurnStart(CombatState state)
    {
        base.OnTurnStart(state);

        SpreadDisease(state);

        Console.WriteLine("Sou um médico e estou cuidando da minha doença.");
    }

    public void SpreadDisease(CombatState state)
    {
        var validEnemies = state.Enemies.Where(p => p.StatusEffects.Any(s => s is DiseaseEffect)).ToList();

        foreach (var enemy in validEnemies)
        {
            var disease = enemy.StatusEffects.OfType<DiseaseEffect>().FirstOrDefault();

            if (disease == null)
                continue;

            if (disease.Behavior == null)
            {
                Console.WriteLine($"[ERRO] Disease '{disease.Name}' em '{enemy.Name}' sem Behavior (Behavior == null). DiseaseId={disease.Id}");
                continue;
            }

            List<BaseCharacter>? targets = null;
            try
            {
                targets = disease.Behavior.GetTargets(enemy, state);
            }
            catch (MissingMethodException mmex)
            {
                Console.WriteLine($"[ERRO] MissingMethodException ao chamar GetTargets em '{disease.Behavior.GetType().FullName}': {mmex.Message}");
                Console.WriteLine($"[ERRO] Verifique se a implementação de IBehavior está compilada com a mesma assinatura de interface.");
                continue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Exceção ao chamar GetTargets em '{disease.Behavior.GetType().FullName}': {ex}");
                continue;
            }

            Console.WriteLine($"1 - {Name} está tentando espalhar a doença {disease.Name} de {enemy.Name} para outros inimigos. targets.Count = {targets?.Count}");

            if (targets == null || targets.Count == 0)
                continue;

            targets.Remove(this);


            Console.WriteLine($"2 - {Name} está tentando espalhar a doença {disease.Name} de {enemy.Name} para outros inimigos. targets.Count = {targets?.Count}");

            if (targets.Count == 0)
                continue;

            var rng = new Random();

            var chance = 15 + disease.Level * 5;

            if (rng.Next(1, 101) <= chance)
                continue;

            var randomIndex = rng.Next(targets.Count);

            var infection = disease.Clone();
            infection.Duration = Math.Max(disease.Duration, this.Disease.Duration);

            targets[randomIndex].ApplyPureStatusEffect(infection);
        }
    }

    public void Infect(BaseCharacter target)
    {
        Console.WriteLine($"{Name} está infectando {target.Name} com a doença {Disease.Name}.");

        this.CurrentMana -= 50;
        this.Gold -= 10;

        target.ApplyPureStatusEffect(Disease.Clone());
    }
}
