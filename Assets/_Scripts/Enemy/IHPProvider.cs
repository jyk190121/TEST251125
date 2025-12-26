// IHPProvider.cs
public interface IHPProvider
{
    float CurrentHP { get; }
    float MaxHP { get; }
    bool IsAlive { get; }
}

