public interface IDamageable
{
    int CurrentHealth { get; }
    void TakeDamage(int amount);
}
