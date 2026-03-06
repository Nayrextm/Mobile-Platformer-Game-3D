using UnityEngine;
using DG.Tweening; 
public class GravityPad : MonoBehaviour
{
    [Header("Налаштування взаємодії")]
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _cooldown = 0.2f;

    [SerializeField] private bool _addPushOff = true;
    [SerializeField] private float _pushForce = 5f;

    [Header("Візуальні налаштування")]
    [SerializeField] private Renderer _padRenderer;
    [SerializeField] private Color _defaultColor = Color.cyan;
    [SerializeField] private Color _activeColor = Color.white;
    [SerializeField] private float _glowIntensity = 2f;
    [SerializeField] private float _bounceScale = 0.8f;
    [SerializeField] private float _animationDuration = 0.15f; 

    [Header("Ефекти")]
    [SerializeField] private ParticleSystem _activationEffect;
    [SerializeField] private AudioSource _activationSound;

    private Vector3 _originalScale;
    private float _lastActivationTime = -1f;

   
    private MaterialPropertyBlock _propBlock;
    private static readonly int _emissionColorID = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        _originalScale = transform.localScale;
        _propBlock = new MaterialPropertyBlock();

        SetGlowColor(_defaultColor, 0f); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
           
            Rigidbody playerRb = other.attachedRigidbody;

            if (playerRb != null)
            {
              
                PlayerController player = playerRb.GetComponent<PlayerController>();

                if (player != null)
                {
                    TryActivate(player, playerRb);
                }
            }
        }
    }

    private bool IsPlayer(Collider other)
    {
        return (_playerLayer.value & (1 << other.gameObject.layer)) != 0;
    }

    private void TryActivate(PlayerController player, Rigidbody playerRb)
    {
        if (Time.time - _lastActivationTime < _cooldown) return;
        _lastActivationTime = Time.time;

        ActivatePad(player, playerRb);
    }

    private void ActivatePad(PlayerController player, Rigidbody playerRb)
    {
      
        player.FlipGravity();

      
        if (_addPushOff && playerRb != null)
        {
          
            var velocity = playerRb.velocity;
            velocity.y = 0f;
            playerRb.velocity = velocity;

            playerRb.AddForce(transform.up * _pushForce, ForceMode.VelocityChange);
        }

       
        if (_activationEffect != null) _activationEffect.Play();
        if (_activationSound != null) _activationSound.Play();

      
        PlayPadAnimation();
    }

    private void PlayPadAnimation()
    {
        transform.DOKill();

       
        transform.DOScale(_originalScale * _bounceScale, _animationDuration)
                 .SetLoops(2, LoopType.Yoyo)
                 .SetEase(Ease.OutQuad)
                 .SetLink(gameObject);

       
        SetGlowColor(_activeColor, _glowIntensity);

        DOVirtual.DelayedCall(_animationDuration, () =>
        {
            SetGlowColor(_defaultColor, 0f);
        }).SetLink(gameObject);
    }

    private void SetGlowColor(Color color, float intensity)
    {
        if (_padRenderer != null)
        {
            _padRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(_emissionColorID, color * intensity);
            _padRenderer.SetPropertyBlock(_propBlock);
        }
    }
}