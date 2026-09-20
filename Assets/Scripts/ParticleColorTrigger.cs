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
    [SerializeField] private Sprite _modeIcon;
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
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out PlayerController player))
        {
            if (player.IconFX != null && _modeIcon != null)
            {
                player.IconFX.PlayIconAnimation(_modeIcon, _iconDuration);
            }
        }

        if (BackgroundFX.Instance != null)
        {
            BackgroundFX.Instance.ChangeEnvironmentSmoothly(
                _targetParticleColor, _targetSkyboxColor, _targetFogColor, _targetPlatformColor, _transitionDuration
            );
        }

        if (_useCameraShake && CameraFollow.Instance != null)
        {
            CameraFollow.Instance.TriggerShake(_shakeDuration, _shakeMagnitude);
        }
    }
}