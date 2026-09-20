using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class DetonatorTrigger : MonoBehaviour
{
    [Header("Посилання")]
    [Tooltip("Динаміт, який вибухне при активації цього тригера")]
    [SerializeField] private ExplosiveController _targetDynamite;

    [Header("Анімація ручки (DOTween)")]
    [SerializeField] private Transform _handleModel;
    [Tooltip("Фінальна локальна позиція ручки після натискання (зміщення по осі Z)")]
    [SerializeField] private Vector3 _pressedLocalPosition = new Vector3(0f, 0f, -0.2f);
    [SerializeField] private float _pressDuration = 0.2f;
    [SerializeField] private Ease _pressEase = Ease.OutBack;

    [Header("Звук")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _pressSfx;

    private bool _isTriggered = false;
    private Vector3 _originalLocalPosition;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        if (_handleModel != null) _originalLocalPosition = _handleModel.localPosition;
    }

    private void Start()
    {
        if (LevelManager.Instance != null) LevelManager.Instance.OnLevelReset += ResetTrigger;
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null) LevelManager.Instance.OnLevelReset -= ResetTrigger;
        if (_handleModel != null) _handleModel.DOKill();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isTriggered || !other.CompareTag("Player")) return;

        _isTriggered = true;

        if (_handleModel != null)
        {
            _handleModel.DOKill();

            _handleModel.DOLocalMove(_pressedLocalPosition, _pressDuration)
                        .SetEase(_pressEase)
                        .SetLink(gameObject);
        }

        if (_audioSource != null && _pressSfx != null) _audioSource.PlayOneShot(_pressSfx);

        if (_targetDynamite != null) _targetDynamite.Detonate();
        else Debug.LogWarning("Детонатор натиснуто, але динаміт не підключено!");
    }

    private void ResetTrigger()
    {
        _isTriggered = false;
        if (_handleModel != null)
        {
            _handleModel.DOKill();
            _handleModel.localPosition = _originalLocalPosition;
        }
    }
}