namespace Units
{
    public interface IDamageable
    {
        public int MaximumHealth { get; }
        public int CurrentHealth { get; }

        void TakeDamage(int damageDealt);
        void Heal(int healthRestored);
    }
}
