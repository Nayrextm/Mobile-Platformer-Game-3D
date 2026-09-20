using UnityEngine;
using DG.Tweening; 

public class BouncePad : MonoBehaviour
{
    [Header("Налаштування стрибка")]
    [SerializeField] private float bounceForce = 20f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float rayLength = 0.3f;
    [SerializeField] private bool useRaycast = true;
    [SerializeField] private float cooldown = 0.05f;

    [Header("Візуальні налаштування")]
    [SerializeField] private Renderer padRenderer;
    [SerializeField] private Color defaultColor = Color.yellow;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float bounceScale = 0.8f;   
    [SerializeField] private float animationDuration = 0.15f; 

    [Header("Ефекти")]
    [SerializeField] private ParticleSystem bounceEffect;
    [SerializeField] private AudioSource bounceSound;

    private Vector3 originalScale;
    private float lastBounceTime = -1f;

    
    private MaterialPropertyBlock propBlock;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        originalScale = transform.localScale;
        propBlock = new MaterialPropertyBlock();

        
        SetGlowColor(defaultColor, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
            TryBounce(other.attachedRigidbody);
    }

    private void FixedUpdate()
    {
        if (!useRaycast) return;

        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, rayLength, playerLayer))
        {
            TryBounce(hit.rigidbody);
        }
    }

    private bool IsPlayer(Collider other)
    {
        return (playerLayer.value & (1 << other.gameObject.layer)) != 0;
    }

    private void TryBounce(Rigidbody rb)
    {
        if (rb == null) return;

        if (Time.time - lastBounceTime < cooldown) return;
        lastBounceTime = Time.time;

        Bounce(rb);
    }

    private void Bounce(Rigidbody rb)
    {
       
        var velocity = rb.velocity;
        velocity.y = 0f;
        rb.velocity = velocity;

        rb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);

        if (bounceEffect != null) bounceEffect.Play();
        if (bounceSound != null) bounceSound.Play();

        PlayBounceAnimation();
    }

    private void PlayBounceAnimation()
    {
        
        transform.DOKill();

        transform.DOScale(originalScale * bounceScale, animationDuration)
                 .SetLoops(2, LoopType.Yoyo)
                 .SetEase(Ease.OutQuad)
                 .SetLink(gameObject);

        SetGlowColor(activeColor, glowIntensity);

        DOVirtual.DelayedCall(animationDuration, () =>
        {
            SetGlowColor(defaultColor, 0f);
        }).SetLink(gameObject);
    }

    private void SetGlowColor(Color color, float intensity)
    {
        if (padRenderer != null)
        {
            padRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor(EmissionColorID, color * intensity);
            padRenderer.SetPropertyBlock(propBlock);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.up * rayLength);
    }
}