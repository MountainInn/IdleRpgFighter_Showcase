using UnityEngine;
using Zenject;
using TMPro;
using UnityEngine.UI;

public class UiInstaller : MonoInstaller
{
    [SerializeField] FloatingText prefabFloatingText;
    [SerializeField] CritFloatingText prefabCritFloatingText;
    [SerializeField] FloatingTextSpawner mobDamagedFloatingText;
    [Space]
    [SerializeField] Transform canvasTransform;
    [Space]
    [SerializeField] TextMeshProUGUI dpsLabel;
    [Space]
    [SerializeField] MobView mobView;
    [Space]
    [SerializeField] Button attackButton;

    override public void InstallBindings()
    {
        Container
            .Bind<Button>()
            .FromMethod(() => attackButton)
            .WhenInjectedInto<CharacterController>();

        Container
            .Bind<MobView>()
            .FromMethod(() => mobView)
            .AsSingle();

        Container
            .Bind<TextMeshProUGUI>()
            .FromMethod(() => dpsLabel)
            .AsSingle()
            .WhenInjectedInto<DPSMeter>();


        Container
            .BindMemoryPool<FloatingText, FloatingText.Pool>()
            .WithInitialSize(5)
            .FromComponentInNewPrefab(prefabFloatingText)
            .UnderTransform(canvasTransform);

        Container
            .BindMemoryPool<CritFloatingText, CritFloatingText.Pool>()
            .WithInitialSize(3)
            .FromComponentInNewPrefab(prefabCritFloatingText)
            .UnderTransform(canvasTransform);

        Container
            .Bind<FloatingTextSpawner>()
            .FromMethod(_ => mobDamagedFloatingText)
            .AsSingle()
            .WhenInjectedInto<Mob>();

    }
}
