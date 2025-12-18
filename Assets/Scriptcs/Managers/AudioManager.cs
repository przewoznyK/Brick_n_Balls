using UnityEngine;
using Unity.Entities;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip brickHitSound;
    [Header("Audio Settings")]
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;
    [SerializeField] private float minVolume = 0.8f;
    [SerializeField] private float maxVolume = 1.0f;

    private EntityManager _entityManager;
    private EntityQuery _soundQuery;

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _soundQuery = _entityManager.CreateEntityQuery(typeof(BrickHitSoundTag));
    }

    private void Update()
    {
        if (_soundQuery.IsEmptyIgnoreFilter == false)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            float randomVolume = Random.Range(minVolume, maxVolume);
           
            audioSource.PlayOneShot(brickHitSound, randomVolume);
            _entityManager.RemoveComponent<BrickHitSoundTag>(_soundQuery);
        }
    }
}
