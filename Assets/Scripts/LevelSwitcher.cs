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
    Action onSceneLoadCallback;
    private const string V = "Stupid Hack Scene";

    [Inject] FullScreenCover cover;
    [Inject]
    void Construct()
    {
        cover.duration = transitionDuration;

        bool isStupidHackScenePresent =
            SceneManager
            .sceneCount
            .ToRange()
            .Select(SceneManager.GetSceneAt)
            .Any(sc => (sc.name == V));

        if (!isStupidHackScenePresent)
        {
            SceneManager.CreateScene(V);
        }
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
        bool alreadyLoaded =
            SceneManager
            .sceneCount
            .ToRange()
            .Select(SceneManager.GetSceneAt)
            .Any(sc => (sc.buildIndex == levelSceneBuildIndex));

        bool haveToLoad = !alreadyLoaded;

        if (alreadyLoaded)
        {
            GetCurrentLevelScene(levelSceneBuildIndex);
        }
        else
        {
            await cover.FadeIn();

            UniTask loadTask =
                SceneManager
                .LoadSceneAsync(levelSceneBuildIndex, LoadSceneMode.Additive)
                .ToUniTask();

            UniTask unloadTask;
           
            if (currentLevelScene.IsValid())
            {
                unloadTask =
                    SceneManager
                    .UnloadSceneAsync(currentLevelScene)
                    .ToUniTask();
            }
            else
            {
                unloadTask = UniTask.CompletedTask;
            }

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
