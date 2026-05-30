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
    [Tooltip("Чи хочемо ми змінювати колір глобального туману на цьому рівні?")]
    [SerializeField] private bool _enableFogChange = true;

    [Tooltip("Початковий колір туману при старті рівня та після смерті")]
    [SerializeField] private Color _defaultFogColor = Color.gray;

    // Компоненти частинок
    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _mainModule;
    private ParticleSystemRenderer _particleRenderer;
    private Material _particleMaterial;

    // Компоненти скайбоксу
    private Material _skyboxInstance;
    private int _skyColorPropertyID;

    private Coroutine _environmentTransitionCoroutine;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        _particleSystem = GetComponent<ParticleSystem>();
        _mainModule = _particleSystem.main;
        _particleRenderer = GetComponent<ParticleSystemRenderer>();

        _skyColorPropertyID = Shader.PropertyToID(_skyColorPropertyName);

        if (_enableSkyboxChange && RenderSettings.skybox != null)
        {
            _skyboxInstance = new Material(RenderSettings.skybox);
            RenderSettings.skybox = _skyboxInstance;
        }
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

    /// <summary>
    /// Головний метод зміни всього оточення (викликається з тригерів)
    /// </summary>
    public void ChangeEnvironmentSmoothly(Color targetParticle, Color targetSkybox, Color targetFog, float duration)
    {
        if (_environmentTransitionCoroutine != null) StopCoroutine(_environmentTransitionCoroutine);
        _environmentTransitionCoroutine = StartCoroutine(EnvironmentLerpCoroutine(targetParticle, targetSkybox, targetFog, duration));
    }

    /// <summary>
    /// Автоматичне скидання до дефолтного стану при рестарті
    /// </summary>
    public void ResetToDefault()
    {
        if (_environmentTransitionCoroutine != null) StopCoroutine(_environmentTransitionCoroutine);

        // 1. Скидання частинок
        Color particleColorWithAlpha = _defaultParticleColor;
        particleColorWithAlpha.a = 0.6f;
        _mainModule.startColor = particleColorWithAlpha;
        if (_particleMaterial != null) _particleMaterial.color = particleColorWithAlpha;

        // 2. Скидання скайбоксу
        if (_enableSkyboxChange && _skyboxInstance != null)
        {
            _skyboxInstance.SetColor(_skyColorPropertyID, _defaultSkyboxColor);
        }

        // 3. Скидання туману
        if (_enableFogChange)
        {
            RenderSettings.fogColor = _defaultFogColor;
        }
    }

    private IEnumerator EnvironmentLerpCoroutine(Color targetParticle, Color targetSkybox, Color targetFog, float duration)
    {
        targetParticle.a = 0.6f;
        float elapsed = 0f;

        // Кешуємо прапорці для чищення коду від спагетті
        bool isWave = (_transitionType == TransitionType.VertexColorWave);
        bool hasSkybox = (_enableSkyboxChange && _skyboxInstance != null);
        bool hasFog = _enableFogChange;

        if (!isWave && _particleMaterial == null) _particleMaterial = _particleRenderer.material;

        // Запам'ятовуємо стартові кольори
        Color startSkyColor = hasSkybox ? _skyboxInstance.GetColor(_skyColorPropertyID) : Color.black;
        Color startParticleColor = isWave ? _mainModule.startColor.color : _particleMaterial.color;
        Color startFogColor = hasFog ? RenderSettings.fogColor : Color.gray;

        // Цикл плавної зміни
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Лерп Неба
            if (hasSkybox)
                _skyboxInstance.SetColor(_skyColorPropertyID, Color.Lerp(startSkyColor, targetSkybox, t));

            // Лерп Туману
            if (hasFog)
                RenderSettings.fogColor = Color.Lerp(startFogColor, targetFog, t);

            // Лерп Частинок
            if (isWave)
                _mainModule.startColor = Color.Lerp(startParticleColor, targetParticle, t);
            else
                _particleMaterial.color = Color.Lerp(startParticleColor, targetParticle, t);

            yield return null;
        }

        // Залізобетонна фіксація фінальних кольорів після завершення циклу
        if (hasSkybox) _skyboxInstance.SetColor(_skyColorPropertyID, targetSkybox);
        if (hasFog) RenderSettings.fogColor = targetFog;

        if (isWave) _mainModule.startColor = targetParticle;
        else _particleMaterial.color = targetParticle;
    }
}