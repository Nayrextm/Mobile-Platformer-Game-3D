using UnityEngine;

public class GhostTrailSpawner : MonoBehaviour
{
    [Header("Налаштування")]
    public GameObject ghostPrefab; 
    public float spawnInterval = 0.1f; 
    public float minDistanceToSpawn = 0.1f; 

    private float _spawnTimer;
    private Vector3 _lastSpawnPosition;

    void Start()
    {
        _lastSpawnPosition = transform.position;
    }

    void Update()
    {
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= spawnInterval && Vector3.Distance(transform.position, _lastSpawnPosition) > minDistanceToSpawn)
        {
            SpawnGhost();
            _spawnTimer = 0f; 
        }
    }

    void SpawnGhost()
    {
        Instantiate(ghostPrefab, transform.position, transform.rotation);
        _lastSpawnPosition = transform.position;
    }
}