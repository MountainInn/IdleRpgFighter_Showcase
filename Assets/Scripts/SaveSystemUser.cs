using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Diagnostics;
using System;

public class SaveSystemUser : MonoBehaviour
{
    [Inject] SaveSystem saveSystem;

    void Start()
    {
        saveSystem.Load();

        Observable
            .Interval(TimeSpan.FromSeconds(10))
            .DoOnSubscribe(() => saveSystem.Save())
            .Do(_ => saveSystem.Save())
            .DoOnCancel(() => saveSystem.Save())
            .Debug("SaveSystemUser")
            .Subscribe()
            .AddTo(this);
    }
}
