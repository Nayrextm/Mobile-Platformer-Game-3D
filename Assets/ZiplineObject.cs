using UnityEngine;

public class ZiplineObject : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Множник швидкості (1 = стандартна, 1.5 = швидше)")]
    public float speedMultiplier = 1.0f;

    void Start()
    {
        // Автоматичне налаштування
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        if (!gameObject.CompareTag("Zipline"))
        {
            gameObject.tag = "Zipline";
        }
    }

    // ВАЖЛИВО: Ця штука малює лінію в редакторі.
    // Червона стрілка показує, куди полетить гравець.
    // Обертайте об'єкт (Rotation Z), щоб змінити кут нахилу.
    private void OnDrawGizmos()
    {
        // 1. Зберігаємо правильну матрицю повороту/масштабу об'єкта
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.yellow;
        // 2. Малюємо куб відносно центру об'єкта (тепер він буде повертатися разом з трубою)
        // Використовуємо Vector3.one, бо реальний розмір вже врахований в матриці
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        Gizmos.color = Color.red;
        // 3. Малюємо напрямок (вісь X - праворуч)
        Gizmos.DrawRay(Vector3.zero, Vector3.right * 3f);

        // Стрілочка на кінці
        Gizmos.DrawSphere(Vector3.right * 3f, 0.2f);
    }
}