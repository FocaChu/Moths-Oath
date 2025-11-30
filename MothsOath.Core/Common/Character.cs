namespace MothsOath.Core.Common;

public abstract class Character
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public int CurrentHP { get; set; }

    public int MaxHP { get; set; }

    public int BaseStrength { get; set; }

    public int BonusStrength { get; set; } = 0;

    public int BaseDefense { get; set; }

    public int BonusDefense { get; set; } = 0;

    public int Shield { get; set; } = 0;

    public int TotalStrength => BaseStrength + BonusStrength;

    public int TotalDefense => BaseDefense + BonusDefense;

    public bool IsAlive => CurrentHP > 0;

    public event Action<Character, int> OnDamageTaken;

    public void TakeDamage(int damage, bool bypass)
    {
        if (damage > 0)
        {
            if(bypass)
            {
                this.CurrentHP -= damage;
                return;
            }

            damage -= TotalDefense;

            if (this.Shield > 0)
            {
                int absorvedDamage = Math.Min(damage, this.Shield);
                damage -= absorvedDamage;
                this.Shield -= absorvedDamage;
            }

            int finalDamage = Math.Max(damage, 0);

            if (finalDamage > 0)
            {
                CurrentHP -= finalDamage;
                OnDamageTaken?.Invoke(this, finalDamage); 
            }
        }
    }
}