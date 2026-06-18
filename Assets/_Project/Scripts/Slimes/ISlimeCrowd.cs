using System;
public interface ISlimeCrowd
{
    int SlimeCount { get; }
    event Action<int> OnSlimeCountChanged;
    
}