using UnityEngine;
using Zenject;
using UniRx;
using System;

public class Gulag : MonoBehaviour
{
    [Inject] LevelSwitcher levelSwitcher;
    [Inject] MobView rockView;
    [Inject] GameSettings gameSettings;
    [Inject] Cheats cheats;
    [Inject]
    public void Construct()
    {
        gameSettings.gulagDuration
            .Subscribe(duration => timer.ResizeAndRefill(duration))
            .AddTo(this);
    }

    Volume timer = new();

    void Start()
    {
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
