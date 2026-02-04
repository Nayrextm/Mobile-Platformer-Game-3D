using UnityEngine;
using System.Collections;

public class GravityPad : MonoBehaviour
{
    [Header("Interaction Settings")]
    public LayerMask playerLayer;
    public float cooldown = 0.2f; // Час перед повторним спрацюванням

    // Чи потрібно підштовхнути гравця від пада, щоб він не "залип" у ньому
    public bool addPushOff = true;
    public float pushForce = 5f;

    [Header("Visual Settings")]
    public Renderer padRenderer;
    public Color defaultColor = Color.cyan; // Блакитний для гравітації
    public Color activeColor = Color.white;
    public float glowIntensity = 2f;
    public float bounceScale = 0.8f;
    public float animationSpeed = 10f;

    [Header("Effects")]
    public ParticleSystem activationEffect;
    public AudioSource activationSound;

    private Vector3 originalScale;
    private Material padMaterial;
    private float lastActivationTime = -1f;

    private void Start()
    {
        originalScale = transform.localScale;

        if (padRenderer != null)
        {
            padMaterial = padRenderer.material;
            padMaterial.EnableKeyword("_EMISSION");
            padMaterial.SetColor("_EmissionColor", defaultColor * 0f); // Спочатку без світіння (або налаштуй інакше)
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Перевіряємо, чи це гравець
        if (IsPlayer(other))
        {
            // Шукаємо скрипт PlayerController на об'єкті
            PlayerController player = other.GetComponent<PlayerController>();
            // Якщо не знайшли на самому об'єкті, шукаємо в батьківських (корисно, якщо колайдер на Visual)
            if (player == null) player = other.GetComponentInParent<PlayerController>();

            if (player != null)
            {
                TryActivate(player);
            }
        }
    }

    private bool IsPlayer(Collider other)
    {
        return (playerLayer.value & (1 << other.gameObject.layer)) != 0;
    }

    private void TryActivate(PlayerController player)
    {
        // Перевірка кулдауну
        if (Time.time - lastActivationTime < cooldown) return;

        lastActivationTime = Time.time;

        ActivatePad(player);
    }

    private void ActivatePad(PlayerController player)
    {
        // 1. ГОЛОВНА ДІЯ: Міняємо гравітацію
        player.FlipGravity();

        // 2. Опціонально: Трішки відштовхуємо гравця від пада
        // Це допомагає уникнути ситуації, коли гравець застрягає в колайдері пада при зміні гравітації
        if (addPushOff)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Штовхаємо в напрямку, куди дивиться пад (зазвичай вгору transform.up)
                rb.AddForce(transform.up * pushForce, ForceMode.VelocityChange);
            }
        }

        // 3. Ефекти
        if (activationEffect != null) activationEffect.Play();
        if (activationSound != null) activationSound.Play();

        // 4. Анімація
        StartCoroutine(AnimatePadVisual());
    }

    private IEnumerator AnimatePadVisual()
    {
        // Стиснення
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * bounceScale, t);
            SetGlowColor(activeColor, glowIntensity);
            yield return null;
        }

        // Повернення
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            transform.localScale = Vector3.Lerp(originalScale * bounceScale, originalScale, t);
            SetGlowColor(defaultColor, 0f); // Повертаємо до тьмяного або стандартного кольору
            yield return null;
        }

        transform.localScale = originalScale;
    }

    private void SetGlowColor(Color color, float intensity)
    {
        if (padMaterial != null)
            padMaterial.SetColor("_EmissionColor", color * intensity);
    }
}