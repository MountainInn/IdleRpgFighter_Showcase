using UnityEngine;
using Zenject;
using UniRx;

public class Gulag : MonoBehaviour
{
    [SerializeField] float gulagDuration;

    [Inject] SceneLoader sceneLoader;
    [Inject] MobView rockView;

    Volume timer;

    void Start()
    {
        timer = new(gulagDuration, gulagDuration);

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
