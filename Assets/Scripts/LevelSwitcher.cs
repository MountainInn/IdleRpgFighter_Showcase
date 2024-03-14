using UnityEngine;
using Zenject;
using System.Threading.Tasks;

public class LevelSwitcher : MonoBehaviour
{
    [SerializeField] GameObject defaultLevel;
    [SerializeField] float transitionDuration;

    GameObject currentLevel;

    [Inject] Transform levelHolder;
    [Inject] Fade cover;
    [Inject]
    void Construct()
    {
        cover.duration = transitionDuration;
    }

    public async Task<bool> MaybeSwitchLevel(GameObject prefabLevel)
    {
        bool needToSwitch = (currentLevel?.name != prefabLevel.name);

        if (needToSwitch)
        {
            await cover.FadeIn();

            if (currentLevel != null)
            {
                Destroy(currentLevel.gameObject);
                currentLevel = null;
            }

            foreach (Transform child in levelHolder)
            {
                Destroy(child.gameObject);
            }

            currentLevel = Instantiate(prefabLevel, levelHolder);
            currentLevel.transform.localScale = Vector3.one;
            currentLevel.transform.localPosition = Vector3.zero;

            await cover.FadeOut();
        }

        return needToSwitch;
    }
}
