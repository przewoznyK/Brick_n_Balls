using TMPro;
using Unity.Entities;
using UnityEngine;

public class BallCountDisplay : MonoBehaviour
{
    public TextMeshProUGUI ballCountText;
    private SpawnBallSystem spawnBallSystem;
    private EntityManager entityManager;
    private EntityQuery _playerQuery;
    private int _lastKnownAmmo = -1;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _playerQuery = entityManager.CreateEntityQuery(typeof(ShooterComponent));
    }

    void Update()
    {
        if (_playerQuery.IsEmptyIgnoreFilter == false)
        {
            var playerComp = _playerQuery.GetSingleton<ShooterComponent>();

            if (playerComp.BallCount != _lastKnownAmmo)
            {
                _lastKnownAmmo = playerComp.BallCount;
                ballCountText.text = $"Balls: {_lastKnownAmmo}";
            }
        }
    }
}
