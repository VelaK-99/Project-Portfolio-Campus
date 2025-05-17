using UnityEngine;

public interface IBoss
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
}
