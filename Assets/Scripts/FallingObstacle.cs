using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(BoxCollider))]
public class FallingObstacle : MonoBehaviour
{
    [Header("Зв'язки")]
    [Tooltip("Перетягніть сюди дочірній об'єкт із самою 3D-моделлю та колайдером перешкоди")]
    [SerializeField] private Transform _visualStalactite;

    [Tooltip("Система частинок удару об землю (Burst)")]
    [SerializeField] private ParticleSystem _impactParticles;

    [Tooltip("Система частинок піску/пилу, що сиплеться ДО падіння")]
    [SerializeField] private ParticleSystem _warningDustParticles;

    [Header("Налаштування Падіння")]
    [SerializeField] private float _fallDistance = 8f;
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private Vector3 _shakeStrength = new Vector3(15f, 0f, 15f);
    [SerializeField] private float _fallDuration = 0.25f;

    private Vector3 _startPos;
    private Quaternion _startRot;
    private bool _isTriggered = false;

    private void Awake()
    {
        if (_visualStalactite != null)
        {
            _startPos = _visualStalactite.localPosition;
            _startRot = _visualStalactite.localRotation;
        }
    }

    private void Start()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelReset += ResetObstacle;
        }
    }

    private void OnEnable()
    {
        if (_warningDustParticles != null && !_isTriggered)
        {
            _warningDustParticles.Play();
        }
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelReset -= ResetObstacle;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isTriggered) return;

        if (other.CompareTag("Player"))
        {
            TriggerFall();
        }
    }

    private void TriggerFall()
    {
        _isTriggered = true;

        if (_visualStalactite == null) return;

        // Зупиняємо сипучий пісок, бо сталактит зараз впаде
        if (_warningDustParticles != null) _warningDustParticles.Stop();

        Sequence fallSeq = DOTween.Sequence().SetLink(gameObject);

        fallSeq.Append(_visualStalactite.DOShakeRotation(_shakeDuration, _shakeStrength, 10, 90, false));

        fallSeq.Append(_visualStalactite.DOLocalMoveY(_startPos.y - _fallDistance, _fallDuration)
               .SetEase(Ease.InQuad));

        fallSeq.OnComplete(() =>
        {
            if (_impactParticles != null) _impactParticles.Play();
        });
    }

    public void ResetObstacle()
    {
        _isTriggered = false;

        if (_visualStalactite != null)
        {
            _visualStalactite.DOKill();
            _visualStalactite.localPosition = _startPos;
            _visualStalactite.localRotation = _startRot;
        }

        // Знову запускаємо пісок при рестарті рівня
        if (_warningDustParticles != null) _warningDustParticles.Play();
    }
}