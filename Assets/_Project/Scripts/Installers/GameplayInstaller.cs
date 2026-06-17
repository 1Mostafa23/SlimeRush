using System.ComponentModel;
using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [Header("Scene References")]
    [SerializeField] private SlimeCrowdManager slimeCrowdManager;

    public override void InstallBindings()
    {
        Container.Bind<SlimeCrowdManager>().FromInstance(slimeCrowdManager).AsSingle();
    }
}
