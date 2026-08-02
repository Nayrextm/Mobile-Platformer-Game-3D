using UnityEngine;
using DG.Tweening;
using System;

public class ExplosiveVFX : MonoBehaviour
{
    [Header("Налаштування ефектів")]
    [SerializeField] private string _explosionPoolTag = "DeathExplosion";
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionSfx;

    [Header("Трясіння камери")]
    [SerializeField] private bool _useCameraShake = true;
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private float _shakeMagnitude = 0.5f;

    [Header("Візуал самого динаміту")]
    [SerializeField] private Transform _dynamiteVisualModel;

    public void PlayExplosionEffects(GameObject[] targets, Action onComplete)
    {
        if (_audioSource != null && _explosionSfx != null) _audioSource.PlayOneShot(_explosionSfx);

        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.SpawnFromPool(_explosionPoolTag, transform.position, Quaternion.identity);
        }

        if (_useCameraShake && CameraFollow.Instance != null)
        {
            CameraFollow.Instance.TriggerShake(_shakeDuration, _shakeMagnitude);
        }

        // Створюємо секвенцію і прив'язуємо її до цього об'єкта
        Sequence explosionSequence = DOTween.Sequence().SetLink(gameObject);

        if (_dynamiteVisualModel != null)
        {
            explosionSequence.Join(_dynamiteVisualModel.DOScale(Vector3.zero, _animationDuration).SetEase(Ease.InBack).SetLink(gameObject));
        }

        foreach (var target in targets)
        {
            if (target != null)
            {
                explosionSequence.Join(target.transform.DOScale(Vector3.zero, _animationDuration).SetEase(Ease.InBack).SetLink(target));
            }
        }

        explosionSequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void ResetVisuals()
    {
        if (_dynamiteVisualModel != null)
        {
            _dynamiteVisualModel.DOKill();
            _dynamiteVisualModel.localScale = Vector3.one;
        }
    }

    private void OnDestroy()
    {
        transform.DOKill();
        if (_dynamiteVisualModel != null) _dynamiteVisualModel.DOKill();
    }
}