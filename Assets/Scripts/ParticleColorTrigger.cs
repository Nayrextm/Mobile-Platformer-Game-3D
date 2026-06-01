using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ParticleColorTrigger : MonoBehaviour
{
    [Header("Налаштування Порталу/Зони")]
    [SerializeField] private float _transitionDuration = 1.0f;

    [Header("Налаштування Трясіння")]
    [SerializeField] private bool _useCameraShake = true;
    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private float _shakeMagnitude = 0.2f;

    [Header("Налаштування Іконки Режиму")]
    [Tooltip("Перетягніть сюди 2D Спрайт іконки для цього порталу (напр. іконку павука чи кораблика)")]
    [SerializeField] private Sprite _modeIcon;
    [Tooltip("Скільки секунд іконка буде висіти над гравцем (рекомендую від 1.5 до 2.5 секунд)")]
    [SerializeField] private float _iconDuration = 2.0f;

    [Header("Кольори для активації")]
    [SerializeField] private Color _targetParticleColor = Color.cyan;
    [ColorUsage(true, true)][SerializeField] private Color _targetSkyboxColor = Color.blue;
    [SerializeField] private Color _targetFogColor = Color.gray;
    [SerializeField] private Color _targetPlatformColor = Color.white;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Зміна кольорів оточення
            if (BackgroundFX.Instance != null)
            {
                BackgroundFX.Instance.ChangeEnvironmentSmoothly(
                    _targetParticleColor, _targetSkyboxColor, _targetFogColor, _targetPlatformColor, _transitionDuration
                );
            }

            // 2. Трясіння камери
            if (_useCameraShake && CameraFollow.Instance != null)
            {
                CameraFollow.Instance.TriggerShake(_shakeDuration, _shakeMagnitude);
            }

            // 3. АКТИВАЦІЯ ІКОНКИ НА ГРАВЦІ
            // Шукаємо компонент FloatingIconFX у дочірніх об'єктах гравця
            FloatingIconFX iconFX = other.GetComponentInChildren<FloatingIconFX>();
            if (iconFX != null && _modeIcon != null)
            {
                iconFX.PlayIconAnimation(_modeIcon, _iconDuration);
            }
        }
    }
}