using TMPro;
using Unity.Entities;
using UnityEngine;

public class ScoreCountDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreCountText;

    public string GetScoreText() { return scoreCountText.text; }

    private EntityManager _entityManager;
    private EntityQuery _scoreQuery;
    private int _lastScore = -1;

    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _scoreQuery = _entityManager.CreateEntityQuery(typeof(ScoreComponent));
    }

    void Update()
    {
        if (_scoreQuery.IsEmptyIgnoreFilter == false)
        {
            var scoreComp = _scoreQuery.GetSingleton<ScoreComponent>();
            int currentScore = scoreComp.Score;

            if (currentScore != _lastScore)
            {
                _lastScore = currentScore;
                scoreCountText.text = $"Score: {_lastScore}";
            }
        }
    }
}
