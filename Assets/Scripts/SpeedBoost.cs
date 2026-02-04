using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpeedBoost : MonoBehaviour
{
    [Header("Boost Settings")]
    public float speedMultiplier = 1.5f;

    [Header("Glow Settings")]
    public Renderer boostRenderer;
    public Color glowColor = Color.cyan;
    public float glowIntensity = 2f;
    public float glowSpeed = 8f;

    private Material boostMaterial;
    private float targetGlow = 0f;
    private float currentGlow = 0f;

    private void Start()
    {
        if (boostRenderer != null)
        {
            boostMaterial = boostRenderer.material;
            boostMaterial.EnableKeyword("_EMISSION");
            boostMaterial.SetColor("_EmissionColor", Color.black);
        }
    }

    private void Update()
    {
        if (boostMaterial == null) return;

        currentGlow = Mathf.Lerp(currentGlow, targetGlow, Time.deltaTime * glowSpeed);
        boostMaterial.SetColor("_EmissionColor", glowColor * currentGlow);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        targetGlow = glowIntensity;

        var player = other.GetComponent<PlayerController>();
        if (player != null)
            player.forwardSpeed *= speedMultiplier;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        targetGlow = 0f;
    }
}
