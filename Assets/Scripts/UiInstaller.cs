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
    [Space]
    [SerializeField] TalentView talentViewPrefab;
    [SerializeField] Transform talentsParent;
    [Space]
    [SerializeField] VaultView vaultView;
    [Space]
    [SerializeField] Transform shopPanel;
    [Space]
    [SerializeField] SegmentedProgressBar arenaProgressBar;

    new void Start()
    {
        base.Start();
        shopPanel.gameObject.SetActive(true);
    }

    override public void InstallBindings()
    {
        Container
            .Bind<CharacterController>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<SegmentedProgressBar>()
            .FromMethod(() => arenaProgressBar)
            .WhenInjectedInto<MobSpawner>();

        BindView(talentViewPrefab, talentsParent);

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
