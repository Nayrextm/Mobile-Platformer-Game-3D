using UnityEngine;
using DG.Tweening;

public class PlayerPortalFX : MonoBehaviour
{
    [Header("Посилання")]
    [Tooltip("Перетягніть сюди ДОЧІРНІЙ об'єкт гравця (з MeshRenderer), який вказано у PlayerController")]
    [SerializeField] private Transform _visualModel;

    [Header("Налаштування Ефекту (Mario 64)")]
    [Tooltip("Масштаб (X, Y, Z) при всмоктуванні. Тепер X великий (витягування вперед).")]
    [SerializeField] private Vector3 _suckInScale = new Vector3(10f, 0.1f, 0.1f); // Дуже сильне витягування вперед!

    [Tooltip("Тривалість фази всмоктування (трохи збільшено для кращої видимості)")]
    [SerializeField] private float _suckInTime = 0.25f;

    [Tooltip("Тривалість фази виходу з порталу")]
    [SerializeField] private float _popOutTime = 0.35f;

    private Sequence _activeSequence;

    public void PlayPortalTransition()
    {
        if (_visualModel == null) return;

        // Зупиняємо попередні анімації, щоб не було конфліктів
        _activeSequence?.Kill();
        _visualModel.DOKill();

        _activeSequence = DOTween.Sequence();

        // 1. Всмоктування (витягуємо вперед, сплющуємо по боках)
        _activeSequence.Append(_visualModel.DOScale(_suckInScale, _suckInTime).SetEase(Ease.InBack));

        // 2. Відскок (пружинисте повернення до нормального розміру)
        _activeSequence.Append(_visualModel.DOScale(Vector3.one, _popOutTime).SetEase(Ease.OutBack));
    }

    private void OnDestroy()
    {
        _activeSequence?.Kill();
    }
}