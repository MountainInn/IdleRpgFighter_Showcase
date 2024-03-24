using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Zenject;
using Cysharp.Threading.Tasks;
using System;

public class LevelSwitcher : MonoBehaviour
{
    [SerializeField] int gulagSceneBuildIndex = 3;
    [SerializeField] float transitionDuration;

    public Scene currentLevelScene {get; private set;}

    int previousArenaSceneIndex;
    bool firstCheck = false;

    Action onSceneLoadCallback;

    [Inject] FullScreenCover cover;
    [Inject]
    void Construct()
    {
        cover.duration = transitionDuration;
    }

    public void AddSceneLoadedCallback(Action onSceneLoadCallback)
    {
        this.onSceneLoadCallback += onSceneLoadCallback;
    }

    public async UniTask SwitchToGulag()
    {
        previousArenaSceneIndex = currentLevelScene.buildIndex;

        await MaybeSwitchLevel(gulagSceneBuildIndex);
    }

    public async UniTask SwitchToArena()
    {
        await MaybeSwitchLevel(previousArenaSceneIndex);
    }

    public async UniTask<bool> MaybeSwitchLevel(int levelSceneBuildIndex)
    {
        int sceneIndex =
            SceneManager
            .sceneCount
            .ToRange()
            .Select(SceneManager.GetSceneAt)
            .ToList()
            .FindIndex(sc => (sc.buildIndex == levelSceneBuildIndex));

        bool alreadyLoaded = (sceneIndex != -1);
        bool haveToLoad = (!alreadyLoaded);

        if (alreadyLoaded)
            GetCurrentLevelScene(levelSceneBuildIndex);

        if (haveToLoad)
        {
            await cover.FadeIn();

            UniTask unloadTask = UniTask.CompletedTask;
            if (currentLevelScene.IsValid())
            {
                unloadTask =
                    SceneManager
                    .UnloadSceneAsync(currentLevelScene)
                    .ToUniTask();
            }

            UniTask loadTask =
                SceneManager
                .LoadSceneAsync(levelSceneBuildIndex, LoadSceneMode.Additive)
                .ToUniTask();

            await UniTask.WhenAll(unloadTask, loadTask);

            GetCurrentLevelScene(levelSceneBuildIndex);

            onSceneLoadCallback?.Invoke();

            await cover.FadeOut();
        }

        return haveToLoad;
    }

    private void GetCurrentLevelScene(int levelSceneBuildIndex)
    {
        string targetSceneName =
            SceneManager
            .GetSceneByBuildIndex(levelSceneBuildIndex)
            .name;

        currentLevelScene = SceneManager.GetSceneByName(targetSceneName);
    }
}
