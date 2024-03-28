using UnityEngine;
using Zenject;
using UniRx;

public class Gulag : MonoBehaviour
{
    [Inject] LevelSwitcher levelSwitcher;
    [Inject] MobView rockView;
    [Inject] GameSettings gameSettings;
    [Inject] Cheats cheats;

    Volume timer;

    void Start()
    {
        timer = new(gameSettings.gulagDuration);

        rockView.SubscribeGulagTimer(this, timer);

        timer
            .ObserveEmpty()
            .WhereEqual(true)
            .Subscribe(_ =>
            {
                levelSwitcher.SwitchToArena();
            })
            .AddTo(this);
    }

    void Update()
    {
        if (cheats.everlastingGulag.Value)
            return;
       
        timer.Subtract(Time.deltaTime);
    }
}
