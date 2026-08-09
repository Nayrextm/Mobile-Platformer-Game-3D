using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(BoxCollider))]
public class PortalVisualDoors : MonoBehaviour
{
    [Header("Двері (Квади)")]
    [SerializeField] private Transform _leftDoor;
    [SerializeField] private Transform _rightDoor;

    [Header("Налаштування Анімації")]
    [Tooltip("Відстань, на яку від'їдуть двері (по осі X або Z, залежно від орієнтації)")]
    [SerializeField] private Vector3 _slideOffset = new Vector3(2.5f, 0f, 0f);
    [SerializeField] private float _duration = 0.35f;
    [Tooltip("Ease.OutBack дасть мультяшний відскок у кінці")]
    [SerializeField] private Ease _openEase = Ease.OutBack;

    private Vector3 _leftDoorStartPos;
    private Vector3 _rightDoorStartPos;
    private bool _isOpened = false;

    private void Awake()
    {
        // Запам'ятовуємо стартові позиції дверей для рестарту
        if (_leftDoor != null) _leftDoorStartPos = _leftDoor.localPosition;
        if (_rightDoor != null) _rightDoorStartPos = _rightDoor.localPosition;
    }

    private void Start()
    {
        // Якщо у вас є менеджер рівнів, розкоментуйте це, щоб двері самі закривалися при рестарті
        // if (LevelManager.Instance != null) LevelManager.Instance.OnLevelReset += ResetDoors;
    }

    private void OnDestroy()
    {
        // if (LevelManager.Instance != null) LevelManager.Instance.OnLevelReset -= ResetDoors;

        // Захист від витоків пам'яті
        if (_leftDoor != null) _leftDoor.DOKill();
        if (_rightDoor != null) _rightDoor.DOKill();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Якщо двері вже відкриті, ігноруємо
        if (_isOpened) return;

        if (other.CompareTag("Player"))
        {
            OpenDoors();
        }
    }

    private void OpenDoors()
    {
        _isOpened = true;

        if (_leftDoor != null)
        {
            _leftDoor.DOLocalMove(_leftDoorStartPos - _slideOffset, _duration)
                     .SetEase(_openEase)
                     .SetLink(gameObject); // Прив'язка до життєвого циклу об'єкта
        }

        if (_rightDoor != null)
        {
            _rightDoor.DOLocalMove(_rightDoorStartPos + _slideOffset, _duration)
                     .SetEase(_openEase)
                     .SetLink(gameObject);
        }
    }

    // Викликайте цей метод при рестарті рівня (смерті гравця)
    public void ResetDoors()
    {
        _isOpened = false;

        if (_leftDoor != null)
        {
            _leftDoor.DOKill();
            _leftDoor.localPosition = _leftDoorStartPos;
        }

        if (_rightDoor != null)
        {
            _rightDoor.DOKill();
            _rightDoor.localPosition = _rightDoorStartPos;
        }
    }
}