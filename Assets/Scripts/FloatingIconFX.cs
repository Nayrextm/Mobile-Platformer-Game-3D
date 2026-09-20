using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class FloatingIconFX : MonoBehaviour
{
    [Header("Налаштування Анімації")]
    [SerializeField] private float _maxScale = 0.8f;
    [SerializeField] private float _floatHeight = 0.5f;

    [Header("Налаштування Камери (Billboard)")]
    [Tooltip("Якщо увімкнено, іконка завжди дивитиметься в камеру (Billboard).")]
    [SerializeField] private bool _faceCamera = true;

    private SpriteRenderer _spriteRenderer;
    private Vector3 _startLocalPosition;
    private Sequence _animationSequence;

    private Transform _mainCamTransform;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startLocalPosition = transform.localPosition;
        _spriteRenderer.enabled = false;

        if (Camera.main != null)
        {
            _mainCamTransform = Camera.main.transform;
        }
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
        if (_faceCamera && _spriteRenderer.enabled && _mainCamTransform != null)
        {
            transform.forward = _mainCamTransform.forward;
        }
    }

    private void OnDestroy()
    {
        _animationSequence?.Kill();
        transform.DOKill();
    }
}