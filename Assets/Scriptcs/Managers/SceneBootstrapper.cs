using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneBootstrapper : MonoBehaviour
{
    private string _uiSceneName = "UIScene";

    private void Start()
    {
        if (SceneManager.GetSceneByName(_uiSceneName).isLoaded == false)
            SceneManager.LoadSceneAsync(_uiSceneName, LoadSceneMode.Additive);
    }
}
