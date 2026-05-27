using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class BackgroundFX : MonoBehaviour
{
    public enum TransitionType { VertexColorWave, GlobalMaterialFlash }

    public static BackgroundFX Instance { get; private set; }

    [Header("Налаштування Режиму")]
    [Tooltip("Vertex Color Wave — колір змінюється хвилею.\nGlobal Material Flash — весь фон змінюється синхронно.")]
    [SerializeField] private TransitionType _transitionType = TransitionType.VertexColorWave;

    [Header("Початковий стан фону")]
    [Tooltip("Колір фону, який буде увімкнено на самому початку рівня та після смерті")]
    [SerializeField] private Color _defaultColor = Color.white;

    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _mainModule;
    private ParticleSystemRenderer _particleRenderer;
    private Material _particleMaterial;

    private Coroutine _colorTransitionCoroutine;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        _particleSystem = GetComponent<ParticleSystem>();
        _mainModule = _particleSystem.main;
        _particleRenderer = GetComponent<ParticleSystemRenderer>();
    }

    private void Start()
    {
        ResetToDefault();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelReset += ResetToDefault;
        }
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelReset -= ResetToDefault;
        }
    }

    public void ChangeColorSmoothly(Color targetColor, float duration)
    {
        if (_colorTransitionCoroutine != null) StopCoroutine(_colorTransitionCoroutine);
        _colorTransitionCoroutine = StartCoroutine(ColorLerpCoroutine(targetColor, duration));
    }

    public void ResetToDefault()
    {
        if (_colorTransitionCoroutine != null) StopCoroutine(_colorTransitionCoroutine);

        Color colorWithAlpha = _defaultColor;
        colorWithAlpha.a = 0.6f;

        _mainModule.startColor = colorWithAlpha;

        if (_particleMaterial != null)
        {
            _particleMaterial.color = colorWithAlpha;
        }
    }

    private IEnumerator ColorLerpCoroutine(Color targetColor, float duration)
    {
        targetColor.a = 0.6f;
        float elapsed = 0f;

        switch (_transitionType)
        {
            case TransitionType.VertexColorWave:
                Color startColorWave = _mainModule.startColor.color;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    _mainModule.startColor = Color.Lerp(startColorWave, targetColor, elapsed / duration);
                    yield return null;
                }
                _mainModule.startColor = targetColor;
                break;

            case TransitionType.GlobalMaterialFlash:
                if (_particleMaterial == null) _particleMaterial = _particleRenderer.material;

                Color startColorFlash = _particleMaterial.color;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    _particleMaterial.color = Color.Lerp(startColorFlash, targetColor, elapsed / duration);
                    yield return null;
                }
                _particleMaterial.color = targetColor;
                break;
        }
    }
}