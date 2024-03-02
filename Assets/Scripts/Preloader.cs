using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Preloader : MonoBehaviour
{
    void Start()
    {
        SceneManager
            .sceneCountInBuildSettings.ToRange()
            .Map(i =>
            {
                Scene sc = default;

                try
                {
                    sc = SceneManager.GetSceneAt(i);
                }
                catch
                {
                    if (!sc.isLoaded)
                    {
                        SceneManager
                            .LoadSceneAsync(i,
                                            new LoadSceneParameters
                                            {
                                                loadSceneMode = LoadSceneMode.Additive
                                            });

                    }
                }
            });
    }
}
