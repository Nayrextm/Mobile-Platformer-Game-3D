using System.Collections;
using UnityEngine;

public class PadController : MonoBehaviour
{
    [Header("Speed Settings")]
    public float targetSpeed = 12f;        
    public bool isSlowPad = false;         
    public float cooldown = 0.1f;         

    [Header("Visual Settings")]
    public Renderer padRenderer;
    public Color defaultColor = Color.yellow;
    public Color activeColor = Color.white;
    public float glowIntensity = 2f;
    public float hitScale = 0.8f;
    public float animSpeed = 10f;

    [Header("Effects (optional)")]
    public ParticleSystem activateFX;
    public AudioSource activateSound;

    private Material padMat;
    private Vector3 originalScale;
    private float lastActivation = -10f;

    void Start()
    {
        originalScale = transform.localScale;

        if (padRenderer != null)
        {
            padMat = padRenderer.material;
            padMat.EnableKeyword("_EMISSION");
            padMat.SetColor("_EmissionColor", defaultColor * 0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            if (Time.time - lastActivation < cooldown) return;
            lastActivation = Time.time;

            if (isSlowPad)
                player.ForwardSpeed = targetSpeed; 
            else
                player.ForwardSpeed = targetSpeed; 

            if (activateFX != null) activateFX.Play();
            if (activateSound != null) activateSound.Play();

            StartCoroutine(ActivateVisual());
        }
    }

    IEnumerator ActivateVisual()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * animSpeed;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * hitScale, t);
            SetGlow(activeColor, glowIntensity);
            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * animSpeed;
            transform.localScale = Vector3.Lerp(originalScale * hitScale, Vector3.zero, t);
            SetGlow(defaultColor, Mathf.Lerp(glowIntensity, 0f, t));
            yield return null;
        }

        gameObject.SetActive(false);
    }

    void SetGlow(Color color, float intensity)
    {
        if (padMat != null)
            padMat.SetColor("_EmissionColor", color * intensity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, Vector3.up * 0.4f);
    }
}

