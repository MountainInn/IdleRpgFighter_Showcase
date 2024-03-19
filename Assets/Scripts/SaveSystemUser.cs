using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Diagnostics;
using System;

public class SaveSystemUser : MonoBehaviour
{
    [Inject] SaveSystem saveSystem;

    [Inject] GameSettings gameSettings;

    void Start()
    {
        saveSystem.Load();

        Observable
            .Interval(TimeSpan.FromSeconds(gameSettings.autoSaveInterval))

            .DoOnSubscribe(() => saveSystem.Save())
            .Do(            _ => saveSystem.Save())
            .DoOnCancel(   () => saveSystem.Save())

            .Debug("SaveSystemUser")
            .Subscribe()
            .AddTo(this);
    }
}
