using UnityEngine;
using DG.Tweening;

public class SpeedModifierPad : MonoBehaviour
{
    [Header("Speed Settings")]
    [Tooltip("якщо б≥льше за стандартну швидк≥сть - прискорюЇ. якщо менше - спов≥льнюЇ.")]
    [SerializeField] private float _targetSpeed = 12f;
    [SerializeField] private float _cooldown = 0.2f;

    [Header("Visual Settings")]
    [SerializeField] private Renderer _padRenderer;
    [SerializeField] private Color _defaultColor = Color.yellow;
    [SerializeField] private Color _activeColor = Color.white;
    [SerializeField] private float _glowIntensity = 2f;
    [SerializeField] private float _hitScale = 0.8f;
    [SerializeField] private float _animDuration = 0.15f; 

    [Header("Effects (optional)")]
    [SerializeField] private ParticleSystem _activateFX;
    [SerializeField] private AudioSource _activateSound;

    private Vector3 _originalScale;
    private float _lastActivation = -10f;

    
    private MaterialPropertyBlock _propBlock;
    private static readonly int _emissionColorID = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        _originalScale = transform.localScale;
        _propBlock = new MaterialPropertyBlock();

        
        SetGlow(_defaultColor, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                TryActivate(pc);
            }
        }
    }

    private void TryActivate(PlayerController player)
    {
        if (Time.time - _lastActivation < _cooldown) return;
        _lastActivation = Time.time;

       
        player.ForwardSpeed = _targetSpeed;

        if (_activateFX != null) _activateFX.Play();
        if (_activateSound != null) _activateSound.Play();

        PlayPadAnimation();
    }

    private void PlayPadAnimation()
    {
        transform.DOKill();

       
        transform.DOScale(_originalScale * _hitScale, _animDuration)
                 .SetLoops(2, LoopType.Yoyo)
                 .SetEase(Ease.OutQuad)
                 .SetLink(gameObject);

      
        SetGlow(_activeColor, _glowIntensity);

        DOVirtual.DelayedCall(_animDuration, () =>
        {
            SetGlow(_defaultColor, 0f);
        }).SetLink(gameObject);
    }

    private void SetGlow(Color color, float intensity)
    {
        if (_padRenderer != null)
        {
            _padRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(_emissionColorID, color * intensity);
            _padRenderer.SetPropertyBlock(_propBlock);
        }
    }
}