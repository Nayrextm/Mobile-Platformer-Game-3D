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
        if (_leftDoor != null) _leftDoorStartPos = _leftDoor.localPosition;
        if (_rightDoor != null) _rightDoorStartPos = _rightDoor.localPosition;
    }

    private void OnEnable()
    {
        ResetDoors();
    }

    private void OnDestroy()
    {
        if (_leftDoor != null) _leftDoor.DOKill();
        if (_rightDoor != null) _rightDoor.DOKill();
    }

    private void OnTriggerEnter(Collider other)
    {
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
                     .SetLink(gameObject);
        }

        if (_rightDoor != null)
        {
            _rightDoor.DOLocalMove(_rightDoorStartPos + _slideOffset, _duration)
                     .SetEase(_openEase)
                     .SetLink(gameObject);
        }
    }

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