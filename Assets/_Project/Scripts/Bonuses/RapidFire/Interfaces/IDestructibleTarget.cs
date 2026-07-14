public interface IDestructibleTarget
{
    bool IsDestroyed { get; }
    void TakeDamage(int amount);
}
