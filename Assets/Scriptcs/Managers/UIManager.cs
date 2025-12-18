using TMPro;
using Unity.Entities;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject uiInGame;
    [SerializeField] private GameObject goBackToMenuPanel;
    [SerializeField] private LoadingScreenUI loadingScreenUI;
    [SerializeField] private ScoreCountDisplay scoreCountDisplay;
    [SerializeField] private TextMeshProUGUI totalScoreText;

    private EntityManager _entityManager;
    private EntityQuery _gameOverQuery;

    public void StartGame()
    {
        menuPanel.SetActive(false);
        uiInGame.SetActive(true);
        loadingScreenUI.ActiveLoadingScreen();
        GameSubSceneHandler.Instance.EnableGameScene();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OpenGoBackToMenuPanel()
    {
        totalScoreText.text = scoreCountDisplay.GetScoreText();
        goBackToMenuPanel.SetActive(true);
        uiInGame.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GoBackToMenu()
    {
        goBackToMenuPanel.SetActive(false);
        GameSubSceneHandler.Instance.DisableGameScene();
        menuPanel.SetActive(true);
    }

    public bool LoadingComplete()
    {
        var query = _entityManager.CreateEntityQuery(typeof(ShooterComponent));

        if (query.CalculateEntityCount() > 0)
        {
            Entity shooterEntity = query.GetSingletonEntity();
            _entityManager.AddComponent<GameStartedTag>(shooterEntity);
            return true;
        }

        return false;
    }

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _gameOverQuery = _entityManager.CreateEntityQuery(
            typeof(ShooterComponent),
            typeof(GameOverTag)
        );

        goBackToMenuPanel.SetActive(false);
    }

    private void Update()
    {
        if (_gameOverQuery.IsEmptyIgnoreFilter == false)
        {
            OpenGoBackToMenuPanel();
            _entityManager.RemoveComponent<GameOverTag>(_gameOverQuery);
        }
    }
}
