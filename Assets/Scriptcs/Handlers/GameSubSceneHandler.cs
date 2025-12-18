using UnityEngine;

public class GameSubSceneHandler : MonoBehaviour
{
    public static GameSubSceneHandler Instance { get; private set; }

    [SerializeField] private GameObject gameSubSceneObject;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void EnableGameScene() => gameSubSceneObject.SetActive(true);
    public void DisableGameScene() => gameSubSceneObject.SetActive(false);
    
}
