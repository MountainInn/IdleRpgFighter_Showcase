using UnityEngine;
using Zenject;
using UniRx;

public class Gulag : MonoBehaviour
{
    [Inject] SceneLoader sceneLoader;
    [Inject] MobView rockView;
    [Inject] GameSettings gameSettings;

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
                sceneLoader.SwitchToArena();
            })
            .AddTo(this);
    }

    void Update()
    {
        timer.Subtract(Time.deltaTime);
    }
}
