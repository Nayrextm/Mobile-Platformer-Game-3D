using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(LaneRunner3D))]
public class MinecartRider : MonoBehaviour
{
    [Header("Старт рівня")]
    [SerializeField] private bool _startInCart = false;
    [SerializeField] private float _startCartSpeed = 15f;

    [Header("Динаміка та Прискорення")]
    [SerializeField] private float _boostMultiplier = 1.5f;
    [SerializeField] private float _boostDuration = 2.5f;
    [SerializeField] private float _accelerationSmoothness = 5f;
    [SerializeField] private float _boostFovExpansion = 15f;

    [Header("Анімації (Процедурні + DOTween)")]
    [SerializeField] private Vector3 _switchWobbleAngles = new Vector3(15f, 0f, 0f);
    [SerializeField] private Vector3 _boostTiltAngles = new Vector3(0f, 0f, 8f);
    [SerializeField] private Vector3 _boostSwayAngles = new Vector3(4f, 0f, 0f);
    [SerializeField] private float _boostSwaySpeed = 12f;

    [Header("Візуал Вагонетки (Холдер)")]
    [SerializeField] private GameObject _minecartModel;
    [SerializeField] private ParticleSystem _wheelSparks;
    [Tooltip("Іскри, що вилітають тільки при старті та кінці розгону")]
    [SerializeField] private ParticleSystem _boostTransitionSparks;
    [Tooltip("Система частинок для ефекту вітру при розгоні")]
    [SerializeField] private ParticleSystem _speedWindParticles;

    [Header("Звук")]
    [SerializeField] private AudioSource _cartAudioSource;
    [SerializeField] private AudioClip _rollingLoopSfx;
    [SerializeField] private AudioClip _boostSfx;

    private LaneRunner3D _laneRunner;
    private bool _isRiding = false;
    private float _baseSpeed;
    private float _targetSpeed;
    private float _boostTimer = 0f;

    private Vector3 _originalModelPos;
    private Quaternion _originalModelRot;
    private bool _isBoosting = false;

    private Vector3 _currentPitch = Vector3.zero;
    private Vector3 _currentSway = Vector3.zero;
    private Vector3 _wobbleOffset = Vector3.zero;
    private float _swayTime = 0f;

    private void Awake()
    {
        _laneRunner = GetComponent<LaneRunner3D>();

        if (_minecartModel != null)
        {
            _originalModelPos = _minecartModel.transform.localPosition;
            _originalModelRot = _minecartModel.transform.localRotation;
            _minecartModel.SetActive(false);
        }

        if (_speedWindParticles != null) _speedWindParticles.Stop();
    }

    private void Start()
    {
        if (LevelManager.Instance != null) LevelManager.Instance.OnLevelReset += DismountCart;
        if (_startInCart) MountCart(_startCartSpeed);
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null) LevelManager.Instance.OnLevelReset -= DismountCart;
        DOTween.Kill("Wobble");
    }

    private void Update()
    {
        if (!_isRiding || !_laneRunner.enabled) return;

        CheckLaneSwitchInput();

        if (_boostTimer > 0f)
        {
            _boostTimer -= Time.deltaTime;
            if (_boostTimer <= 0f) EndBoost();
        }

        if (Mathf.Abs(_laneRunner.ForwardSpeed - _targetSpeed) > 0.05f)
        {
            _laneRunner.ForwardSpeed = Mathf.Lerp(_laneRunner.ForwardSpeed, _targetSpeed, Time.deltaTime * _accelerationSmoothness);
        }

        if (_minecartModel != null)
        {
            Vector3 targetPitch = _isBoosting ? _boostTiltAngles : Vector3.zero;
            _currentPitch = Vector3.Lerp(_currentPitch, targetPitch, Time.deltaTime * 6f);

            if (_isBoosting)
            {
                _swayTime += Time.deltaTime * _boostSwaySpeed;
                _currentSway = _boostSwayAngles * Mathf.Sin(_swayTime);
            }
            else
            {
                _currentSway = Vector3.Lerp(_currentSway, Vector3.zero, Time.deltaTime * 6f);
                _swayTime = 0f;
            }

            _minecartModel.transform.localRotation = Quaternion.Euler(_originalModelRot.eulerAngles + _currentPitch + _currentSway + _wobbleOffset);
        }
    }

    private void CheckLaneSwitchInput()
    {
        bool isTap = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        if (isTap && _minecartModel != null)
        {
            DOTween.Kill("Wobble");
            float randomDir = Random.value > 0.5f ? 1f : -1f;
            Vector3 targetWobble = _switchWobbleAngles * randomDir;

            Sequence boatWobble = DOTween.Sequence().SetId("Wobble");
            boatWobble.Append(DOTween.To(() => _wobbleOffset, x => _wobbleOffset = x, targetWobble, 0.12f).SetEase(Ease.OutQuad))
                      .Append(DOTween.To(() => _wobbleOffset, x => _wobbleOffset = x, -targetWobble * 0.6f, 0.25f).SetEase(Ease.InOutSine))
                      .Append(DOTween.To(() => _wobbleOffset, x => _wobbleOffset = x, targetWobble * 0.25f, 0.2f).SetEase(Ease.InOutSine))
                      .Append(DOTween.To(() => _wobbleOffset, x => _wobbleOffset = x, Vector3.zero, 0.15f).SetEase(Ease.InOutSine))
                      .SetLink(gameObject);
        }
    }

    public void MountCart(float forwardSpeed)
    {
        _isRiding = true;
        _isBoosting = false;
        _baseSpeed = forwardSpeed;
        _targetSpeed = forwardSpeed;
        _boostTimer = 0f;

        _wobbleOffset = Vector3.zero;
        _currentPitch = Vector3.zero;
        _currentSway = Vector3.zero;

        _laneRunner.enabled = true;
        _laneRunner.ForwardSpeed = forwardSpeed;

        if (_minecartModel != null)
        {
            _minecartModel.SetActive(true);
            _minecartModel.transform.localPosition = _originalModelPos;
            _minecartModel.transform.localRotation = _originalModelRot;
        }

        if (_wheelSparks != null) _wheelSparks.Play();

        if (_cartAudioSource != null && _rollingLoopSfx != null)
        {
            _cartAudioSource.clip = _rollingLoopSfx;
            _cartAudioSource.loop = true;
            _cartAudioSource.Play();
        }
    }

    public void DismountCart()
    {
        _isRiding = false;

        if (_laneRunner != null)
        {
            _laneRunner.ForwardSpeed = _baseSpeed;
            if (_laneRunner.enabled) _laneRunner.DisableAndSnapToCenter();
        }

        if (_minecartModel != null)
        {
            DOTween.Kill("Wobble");
            _minecartModel.transform.localPosition = _originalModelPos;
            _minecartModel.transform.localRotation = _originalModelRot;
            _minecartModel.SetActive(false);
        }

        if (_wheelSparks != null) _wheelSparks.Stop();
        if (_cartAudioSource != null) _cartAudioSource.Stop();
        if (CameraFollow.Instance != null) CameraFollow.Instance.FovModifier = 0f;

        if (_speedWindParticles != null) _speedWindParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void TriggerDownhillBoost()
    {
        if (!_isRiding) return;

        _isBoosting = true;
        _targetSpeed = _baseSpeed * _boostMultiplier;
        _boostTimer = _boostDuration;

        if (CameraFollow.Instance != null) CameraFollow.Instance.FovModifier = _boostFovExpansion;
        if (_cartAudioSource != null && _boostSfx != null) _cartAudioSource.PlayOneShot(_boostSfx);

        if (_speedWindParticles != null) _speedWindParticles.Play();

        if (_boostTransitionSparks != null) _boostTransitionSparks.Play();
    }

    private void EndBoost()
    {
        _isBoosting = false;
        _targetSpeed = _baseSpeed;
        if (CameraFollow.Instance != null) CameraFollow.Instance.FovModifier = 0f;

        if (_speedWindParticles != null) _speedWindParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (_boostTransitionSparks != null) _boostTransitionSparks.Play();
    }
}