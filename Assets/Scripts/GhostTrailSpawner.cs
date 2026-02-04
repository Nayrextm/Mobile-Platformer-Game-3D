using UnityEngine;

public class GhostTrailSpawner : MonoBehaviour
{
    [Header("Налаштування")]
    public GameObject ghostPrefab; // Сюди перетягнути префаб привида
    public float spawnInterval = 0.1f; // Як часто створювати клонів (чим менше, тим густіший слід)
    public float minDistanceToSpawn = 0.1f; // Мінімальна відстань руху для створення клона

    private float _spawnTimer;
    private Vector3 _lastSpawnPosition;

    void Start()
    {
        _lastSpawnPosition = transform.position;
    }

    void Update()
    {
        // Рахуємо час
        _spawnTimer += Time.deltaTime;

        // Перевіряємо дві умови:
        // 1. Чи настав час для нового спавну?
        // 2. Чи відсунувся гравець достатньо далеко від попереднього клона? (Щоб не спавнити їх в одній точці, коли стоїш)
        if (_spawnTimer >= spawnInterval && Vector3.Distance(transform.position, _lastSpawnPosition) > minDistanceToSpawn)
        {
            SpawnGhost();
            _spawnTimer = 0f; // Скидаємо таймер
        }
    }

    void SpawnGhost()
    {
        // Створюємо клона в поточній позиції та ротації гравця
        Instantiate(ghostPrefab, transform.position, transform.rotation);

        // Запам'ятовуємо позицію
        _lastSpawnPosition = transform.position;
    }
}