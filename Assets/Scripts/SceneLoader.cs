using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] int arenaSceneBuildIndex;
    [SerializeField] int gulagSceneBuildIndex;

    [Inject] FullScreenCover fullscreenCover;
    [Inject] Character character;

    public async UniTask SwitchToGulag()
    {
        await SwitchScenes(arenaSceneBuildIndex, gulagSceneBuildIndex);
    }

    public async UniTask SwitchToArena()
    {
        await SwitchScenes(gulagSceneBuildIndex, arenaSceneBuildIndex);
    }

    async Task SwitchScenes(int exitSceneIndex, int enterSceneIndex)
    {
        await fullscreenCover.FadeIn();

        await UniTask.WhenAll(
            SceneManager.UnloadSceneAsync(exitSceneIndex).ToUniTask(),
            SceneManager.LoadSceneAsync(enterSceneIndex,
                                        LoadSceneMode.Additive).ToUniTask());

        character.Respawn();

        await fullscreenCover.FadeOut();
    }
}
