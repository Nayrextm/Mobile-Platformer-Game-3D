using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ParticleColorTrigger : MonoBehaviour
{
    [Header("Налаштування Порталу/Зони")]
    [Tooltip("Як швидко змінити колір усього оточення (в секундах)?")]
    [SerializeField] private float _transitionDuration = 1.0f;

    [Header("Кольори для активації")]
    [Tooltip("Новий колір для квадратиків (частинок)")]
    [SerializeField] private Color _targetParticleColor = Color.cyan;

    [Tooltip("Новий колір для Скайбоксу (неба)")]
    [ColorUsage(true, true)]
    [SerializeField] private Color _targetSkyboxColor = Color.blue;

    [Tooltip("Новий колір для глобального туману")]
    [SerializeField] private Color _targetFogColor = Color.gray;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (BackgroundFX.Instance != null)
            {
                // Передаємо всі три кольори в наш універсальний менеджер
                BackgroundFX.Instance.ChangeEnvironmentSmoothly(
                    _targetParticleColor,
                    _targetSkyboxColor,
                    _targetFogColor,
                    _transitionDuration
                );
            }
        }
    }
}