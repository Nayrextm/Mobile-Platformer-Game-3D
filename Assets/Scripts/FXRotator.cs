using UnityEngine;

public class FXRotator : MonoBehaviour
{
    [Header("Налаштування ефекту")]
    [Tooltip("Осі обертання (наприклад, X=0, Y=1, Z=0 для обертання по вертикалі)")]
    [SerializeField] private Vector3 _rotationAxis = new Vector3(0f, 1f, 0f);

    [Tooltip("Загальна швидкість обертання (градуси за секунду)")]
    [SerializeField] private float _speed = 90f;

    // Менеджер забирає звідси готовий результат: Напрямок помножений на Швидкість
    public Vector3 RotationVelocity => _rotationAxis * _speed;

    // Інкапсульований кеш Transform
    public Transform CachedTransform { get; private set; }

    private void Awake()
    {
        CachedTransform = transform;
    }

    private void OnEnable()
    {
        FXRotationManager.Register(this);
    }

    private void OnDisable()
    {
        FXRotationManager.Unregister(this);
    }
}