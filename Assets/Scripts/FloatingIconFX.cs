using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class FloatingIconFX : MonoBehaviour
{
    [Header("Налаштування Анімації")]
    [SerializeField] private float _maxScale = 0.8f;
    [SerializeField] private float _floatHeight = 0.5f;

    [Header("Налаштування Камери (Billboard)")]
    [Tooltip("Якщо увімкнено, у 3D-режимі іконка автоматично обертатиметься до камери.")]
    [SerializeField] private bool _faceCamera = true;

    private SpriteRenderer _spriteRenderer;
    private Vector3 _startLocalPosition;
    private Sequence _animationSequence;

    private Camera _mainCam;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startLocalPosition = transform.localPosition;
        _spriteRenderer.enabled = false;

        _mainCam = Camera.main;
    }

    public void PlayIconAnimation(Sprite iconSprite, float duration)
    {
        if (iconSprite == null) return;

        _animationSequence?.Kill();
        transform.DOKill();

        _spriteRenderer.sprite = iconSprite;
        _spriteRenderer.enabled = true;

        transform.localPosition = _startLocalPosition;
        transform.localScale = Vector3.zero;

        Color resetColor = _spriteRenderer.color;
        resetColor.a = 1f;
        _spriteRenderer.color = resetColor;

        float appearTime = duration * 0.15f;
        float fadeTime = duration * 0.40f;
        float stayTime = duration - appearTime - fadeTime;

        _animationSequence = DOTween.Sequence();

        transform.DOLocalMoveY(_startLocalPosition.y + _floatHeight, duration).SetEase(Ease.OutQuad);

        _animationSequence.Append(transform.DOScale(_maxScale, appearTime).SetEase(Ease.OutBack));
        _animationSequence.AppendInterval(stayTime);
        _animationSequence.Append(_spriteRenderer.DOFade(0f, fadeTime).SetEase(Ease.InSine));

        _animationSequence.OnComplete(() =>
        {
            _spriteRenderer.enabled = false;
        });
    }

    private void LateUpdate()
    {
        // РОЗУМНА ОПТИМІЗАЦІЯ: Обертаємося тільки якщо іконка світиться І камера зараз у 3D-режимі
        if (_faceCamera && _spriteRenderer.enabled && _mainCam != null)
        {
            if (CameraFollow.Instance != null && CameraFollow.Instance.Is3DMode)
            {
                transform.forward = _mainCam.transform.forward;
            }
        }
    }

    private void OnDestroy()
    {
        _animationSequence?.Kill();
        transform.DOKill();
    }
}