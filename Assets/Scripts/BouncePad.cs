using UnityEngine;
using System.Collections;

public class BouncePad : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceForce = 20f;
    public LayerMask playerLayer;
    public float rayLength = 0.3f;
    public bool useRaycast = true;
    public float cooldown = 0.05f;

    [Header("Visual Settings")]
    public Renderer padRenderer;
    public Color defaultColor = Color.yellow;
    public Color activeColor = Color.white;
    public float glowIntensity = 2f;
    public float bounceScale = 0.8f;
    public float animationSpeed = 10f;

    [Header("Effects (optional)")]
    public ParticleSystem bounceEffect;
    public AudioSource bounceSound;

    private Vector3 originalScale;
    private Material padMaterial;
    private float lastBounceTime = -1f;

    private void Start()
    {
        // Save original size
        originalScale = transform.localScale;

        // Setup material/emission
        if (padRenderer != null)
        {
            padMaterial = padRenderer.material;
            padMaterial.EnableKeyword("_EMISSION");
            padMaterial.SetColor("_EmissionColor", defaultColor * 0f);
        }
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

        // Cooldown check
        if (Time.time - lastBounceTime < cooldown) return;

        lastBounceTime = Time.time;

        Bounce(rb);
    }

    private void Bounce(Rigidbody rb)
    {
        // Remove downward/upward velocity
        var velocity = rb.velocity;
        velocity.y = 0f;
        rb.velocity = velocity;

        rb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);

        if (bounceEffect != null) bounceEffect.Play();
        if (bounceSound != null) bounceSound.Play();

        StartCoroutine(BounceVisual());
    }

    private IEnumerator BounceVisual()
    {
        // Compress
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * bounceScale, t);
            SetGlowColor(activeColor, glowIntensity);
            yield return null;
        }

        // Return scale
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            transform.localScale = Vector3.Lerp(originalScale * bounceScale, originalScale, t);
            SetGlowColor(defaultColor, 0f);
            yield return null;
        }
    }

    private void SetGlowColor(Color color, float intensity)
    {
        if (padMaterial != null)
            padMaterial.SetColor("_EmissionColor", color * intensity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.up * rayLength);
    }
}
