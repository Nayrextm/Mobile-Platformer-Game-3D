using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class BackgroundFX : MonoBehaviour
{
    private enum TransitionType { VertexColorWave, GlobalMaterialFlash }

    public static BackgroundFX Instance { get; private set; }

    [Header("--- НАЛАШТУВАННЯ ЧАСТИНОК ---")]
    [SerializeField] private TransitionType _transitionType = TransitionType.VertexColorWave;
    [SerializeField] private Color _defaultParticleColor = Color.white;

    [Header("--- НАЛАШТУВАННЯ СКАЙБОКСУ (НЕБА) ---")]
    [SerializeField] private bool _enableSkyboxChange = true;
    [SerializeField] private string _skyColorPropertyName = "_SkyColor";
    [ColorUsage(true, true)][SerializeField] private Color _defaultSkyboxColor = Color.black;

    [Header("--- НАЛАШТУВАННЯ ТУМАНУ ---")]
    [SerializeField] private bool _enableFogChange = true;
    [SerializeField] private Color _defaultFogColor = Color.gray;

    [Header("--- НАЛАШТУВАННЯ МАТЕРІАЛУ ПЛАТФОРМ ---")]
    [Tooltip("Чи хочемо ми змінювати колір платформ?")]
    [SerializeField] private bool _enablePlatformChange = true;

    [Tooltip("Перетягніть сюди матеріал ваших платформ (напр. Bake_map_BlockWhiteOut)")]
    [SerializeField] private Material _platformMaterial;

    [Tooltip("Ім'я параметра кольору (для URP Lit це зазвичай _BaseColor)")]
    [SerializeField] private string _platformColorPropertyName = "_BaseColor";

    [Tooltip("Колір платформ при старті рівня")]
    [SerializeField] private Color _defaultPlatformColor = Color.white;

    // Внутрішні компоненти
    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _mainModule;
    private ParticleSystemRenderer _particleRenderer;
    private Material _particleMaterial;
    private Material _skyboxInstance;

    // Закешовані ID шейдерів
    private int _skyColorPropertyID;
    private int _platformColorPropertyID;

    // Збереження оригінального кольору файлу матеріалу (для редактора)
    private Color _originalAssetPlatformColor;

    private Coroutine _envTransitionCoroutine;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        _particleSystem = GetComponent<ParticleSystem>();
        _mainModule = _particleSystem.main;
        _particleRenderer = GetComponent<ParticleSystemRenderer>();

        _skyColorPropertyID = Shader.PropertyToID(_skyColorPropertyName);
        _platformColorPropertyID = Shader.PropertyToID(_platformColorPropertyName);

        if (_enableSkyboxChange && RenderSettings.skybox != null)
        {
            _skyboxInstance = new Material(RenderSettings.skybox);
            RenderSettings.skybox = _skyboxInstance;
        }

        // Зберігаємо початковий колір матеріалу з файлів проєкту
        if (_enablePlatformChange && _platformMaterial != null)
        {
            _originalAssetPlatformColor = _platformMaterial.GetColor(_platformColorPropertyID);
        }
    }

    private void Start()
    {
        ResetToDefault();
        if (LevelManager.Instance != null) LevelManager.Instance.OnLevelReset += ResetToDefault;
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null) LevelManager.Instance.OnLevelReset -= ResetToDefault;
    }

    // ВАЖЛИВО: Захист для редактора Unity! 
    // Повертаємо матеріал у початковий стан при зупинці гри.
    private void OnApplicationQuit()
    {
        if (_enablePlatformChange && _platformMaterial != null)
        {
            _platformMaterial.SetColor(_platformColorPropertyID, _originalAssetPlatformColor);
        }
    }

    /// <summary>
    /// Головний метод зміни (тепер приймає 4 кольори)
    /// </summary>
    public void ChangeEnvironmentSmoothly(Color targetParticle, Color targetSkybox, Color targetFog, Color targetPlatform, float duration)
    {
        if (_envTransitionCoroutine != null) StopCoroutine(_envTransitionCoroutine);
        _envTransitionCoroutine = StartCoroutine(EnvironmentLerpCoroutine(targetParticle, targetSkybox, targetFog, targetPlatform, duration));
    }

    public void ResetToDefault()
    {
        if (_envTransitionCoroutine != null) StopCoroutine(_envTransitionCoroutine);

        Color particleColorWithAlpha = _defaultParticleColor;
        particleColorWithAlpha.a = 0.6f;
        _mainModule.startColor = particleColorWithAlpha;

        if (_particleMaterial != null) _particleMaterial.color = particleColorWithAlpha;
        if (_enableSkyboxChange && _skyboxInstance != null) _skyboxInstance.SetColor(_skyColorPropertyID, _defaultSkyboxColor);
        if (_enableFogChange) RenderSettings.fogColor = _defaultFogColor;

        // Скидання платформ до локального дефолту рівня
        if (_enablePlatformChange && _platformMaterial != null)
            _platformMaterial.SetColor(_platformColorPropertyID, _defaultPlatformColor);
    }

    private IEnumerator EnvironmentLerpCoroutine(Color targetParticle, Color targetSkybox, Color targetFog, Color targetPlatform, float duration)
    {
        targetParticle.a = 0.6f;
        float elapsed = 0f;

        bool isWave = (_transitionType == TransitionType.VertexColorWave);
        bool hasSkybox = (_enableSkyboxChange && _skyboxInstance != null);
        bool hasFog = _enableFogChange;
        bool hasPlatform = (_enablePlatformChange && _platformMaterial != null);

        if (!isWave && _particleMaterial == null) _particleMaterial = _particleRenderer.material;

        Color startSky = hasSkybox ? _skyboxInstance.GetColor(_skyColorPropertyID) : Color.black;
        Color startParticle = isWave ? _mainModule.startColor.color : _particleMaterial.color;
        Color startFog = hasFog ? RenderSettings.fogColor : Color.gray;
        Color startPlatform = hasPlatform ? _platformMaterial.GetColor(_platformColorPropertyID) : Color.white;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (hasSkybox) _skyboxInstance.SetColor(_skyColorPropertyID, Color.Lerp(startSky, targetSkybox, t));
            if (hasFog) RenderSettings.fogColor = Color.Lerp(startFog, targetFog, t);
            if (hasPlatform) _platformMaterial.SetColor(_platformColorPropertyID, Color.Lerp(startPlatform, targetPlatform, t));

            if (isWave) _mainModule.startColor = Color.Lerp(startParticle, targetParticle, t);
            else _particleMaterial.color = Color.Lerp(startParticle, targetParticle, t);

            yield return null;
        }

        if (hasSkybox) _skyboxInstance.SetColor(_skyColorPropertyID, targetSkybox);
        if (hasFog) RenderSettings.fogColor = targetFog;
        if (hasPlatform) _platformMaterial.SetColor(_platformColorPropertyID, targetPlatform);

        if (isWave) _mainModule.startColor = targetParticle;
        else _particleMaterial.color = targetParticle;
    }
}