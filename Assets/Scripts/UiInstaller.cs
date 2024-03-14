using UnityEngine;
using Zenject;
using TMPro;
using UnityEngine.UI;

public class UiInstaller : MonoInstaller
{
    [SerializeField] FloatingTextSpawner mobDamagedFloatingText;
    [Space]
    [SerializeField] Transform canvasTransform;
    [Space]
    [SerializeField] TextMeshProUGUI dpsLabel;
    [Space]
    [SerializeField] MobView mobView;
    [Space]
    [SerializeField] Button attackButton;
    [Space]
    [SerializeField] Transform talentsParent;
    [Space]
    [SerializeField] VaultView vaultView;
    [Space]
    [SerializeField] Transform shopPanel;
    [Space]
    [SerializeField] SegmentedProgressBar arenaProgressBar;
    [Space]
    [SerializeField] FloatingText prefabFloatingText;
    [SerializeField] CritFloatingText prefabCritFloatingText;
    [SerializeField] TalentView talentViewPrefab;
    [SerializeField] WeakPointView prefabWeakPoint;
    [Space]
    [SerializeField] DPSMeterView dpsMeterView;
    [Space]
    [SerializeField] Transform abilitiesParent;
    [SerializeField] AbilityButton abilityButtonPrefab;

    new void Start()
    {
        base.Start();
        shopPanel.gameObject.SetActive(true);
    }

    override public void InstallBindings()
    {
        Container .Bind<DPSMeterView>() .FromInstance(dpsMeterView);

        Container
            .BindMemoryPool<WeakPointView, WeakPointView.Pool>()
            .FromComponentInNewPrefab(prefabWeakPoint)
            .UnderTransform(canvasTransform);

        Container
            .Bind<CharacterController>()
            .FromComponentInHierarchy()
            .AsSingle();
       
        Container
            .Bind<SegmentedProgressBar>()
            .FromMethod(() => arenaProgressBar)
            .WhenInjectedInto<MobSpawner>();

        BindView(talentViewPrefab, talentsParent);
        BindView(abilityButtonPrefab, abilitiesParent);

        Container
            .Bind<VaultView>()
            .FromMethod(() => vaultView);

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

    void BindView<T>(T prefabView, Transform parent)
        where T : Component
    {
        Container
            .Bind<T>()
            .FromComponentInNewPrefab(prefabView)
            .AsTransient()
            .OnInstantiated<T>((ctx, view) =>
            {
                view.transform.SetParent(parent);
                view.transform.localScale = Vector3.one;
            });
    }

}
