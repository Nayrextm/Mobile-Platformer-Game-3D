
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;
//using UnityEngine.EventSystems;

//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(LineRenderer))]
//public class PlayerController : MonoBehaviour
//{
//    [Header("Visuals")]
//    [SerializeField] private Transform _visualModel;
//    //Не використовується для обертання -> [SerializeField] private float _rotationSpeed = 360f;
//    [SerializeField] private TrailRenderer _trail;

//    [Header("Ghost Settings")]
//    [SerializeField] private float _ghostAlpha = 0.3f;
//    [SerializeField] private string _safeLayerName = "Ground";
//    [SerializeField] private string _deadlyLayerName = "Obstacle";
//    [SerializeField] private string _wallLayerName = "Wall";

//    [Header("Zipline Settings")]
//    [SerializeField] private float _ziplineBaseSpeed = 15f;
//    [SerializeField] private float _ziplineSnapSpeed = 20f;

//    [Header("Spider Settings")]
//    [SerializeField] private float _spiderBeamDuration = 0.1f;

//    [Header("Movement")]
//    [Tooltip("Кількість ДОДАТКОВИХ стрибків у повітрі. 0 = лише звичайний, 1 = подвійний стрибок.")]
//    [SerializeField] private int _extraAirJumps = 1;
//    [SerializeField] private float _forwardSpeed = 10f;
//    [SerializeField] private float _jumpForce = 12f;
//    [SerializeField] private float _jumpBufferTime = 0.15f;
//    [SerializeField] private float _coyoteTime = 0.1f;

//    [Header("Modes Settings")]
//    [SerializeField] private float _gravityForce = 30f;

//    [Header("Ground Check")]
//    [SerializeField] private LayerMask _groundMask;
//    [SerializeField] private float _groundCheckDistance = 0.6f;
//    [SerializeField] private Vector3 _groundCheckOffset = Vector3.zero;

//    [Header("Effects")]
//    [SerializeField] private ParticleSystem _jumpParticles;
//    [SerializeField] private ParticleSystem _deathParticles;
//    [SerializeField] private ParticleSystem _spiderTeleportParticles;
//    [SerializeField] private ParticleSystem _spiderLandParticles;

//    [Header("Audio SFX")]
//    [SerializeField] private AudioClip _jumpSfx;
//    [SerializeField] private AudioClip _deathSfx;
//    [SerializeField] private AudioClip _gravitySwitchSfx;


//    private Rigidbody _rb;
//    private Collider _myCollider;
//    private AudioSource _audioSource;
//    private LevelManager _levelManager;
//    private CameraFollow _cameraFollow;
//    private LineRenderer _spiderLine;
//    private Renderer[] _modelRenderers;


//    private int _playerLayer, _safeLayer, _deadlyLayer, _wallLayer;
//    private float _defaultSpeed;
//    private Vector3 _originalScale;

//    private float _gravityScale = 1f;
//    private int _jumpsLeft;
//    private float _jumpBufferCounter, _coyoteTimeCounter;

//    private bool _isDead = false;
//    private bool _isGrounded = false;
//    private bool _jumpRequested = false;
//    private bool _isHoldingInput = false; 


//    private bool _isGravityMode = false;
//    private bool _isSpiderMode = false;
//    private bool _isGhostMode = false;
//    private bool _isPhasing = false;
//    private bool _isZiplineMode = false;

//    private Transform _currentZipline;
//    private float _currentZiplineSpeedMod = 1f;


//    public float ForwardSpeed
//    {
//        get => _forwardSpeed;
//        set => _forwardSpeed = value;
//    }

//    private void Awake()
//    {
//        _rb = GetComponent<Rigidbody>();
//        _myCollider = GetComponent<Collider>();
//        _audioSource = GetComponent<AudioSource>();
//        _levelManager = FindObjectOfType<LevelManager>();
//        _cameraFollow = FindObjectOfType<CameraFollow>();
//        _spiderLine = GetComponent<LineRenderer>();

//        if (_spiderLine != null) _spiderLine.enabled = false;

//        _rb.interpolation = RigidbodyInterpolation.Interpolate;
//        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
//        _rb.useGravity = false;

//        if (_audioSource != null)
//        {
//            _audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
//        }

//        _defaultSpeed = _forwardSpeed;
//        _originalScale = transform.localScale;

//        if (_visualModel != null)
//            _modelRenderers = _visualModel.GetComponentsInChildren<Renderer>();

//        InitializeLayers();
//    }

//    private void InitializeLayers()
//    {
//        _playerLayer = gameObject.layer;
//        _safeLayer = LayerMask.NameToLayer(_safeLayerName);
//        _deadlyLayer = LayerMask.NameToLayer(_deadlyLayerName);
//        _wallLayer = LayerMask.NameToLayer(_wallLayerName);

//        if (_wallLayer != -1) _groundMask |= (1 << _wallLayer);
//    }

//    private void Update()
//    {
//        if (_isDead) return;


//        GroundCheck();

//        HandleInput();
//        UpdateTimers();
//        ProcessJumpLogic();
//        HandleZiplineExit();
//    }

//    private void HandleInput()
//    {

//        _isHoldingInput = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || (Input.touchCount > 0);

//        bool isPressedDown = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ||
//                             (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

//        if (_isGhostMode)
//        {
//            SetPhasingState(_isHoldingInput);
//        }
//        else
//        {
//            if (_isPhasing) SetPhasingState(false);

//            if (isPressedDown)
//            {
//                if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
//                    _jumpBufferCounter = _jumpBufferTime;
//            }
//            else
//            {
//                _jumpBufferCounter -= Time.deltaTime;
//            }
//        }
//    }

//    private void UpdateTimers()
//    {
//        if (_isGrounded || _isZiplineMode)
//        {
//            _coyoteTimeCounter = _coyoteTime;
//            _jumpsLeft = _extraAirJumps;
//        }
//        else
//        {
//            _coyoteTimeCounter -= Time.deltaTime;
//        }
//    }

//    private void ProcessJumpLogic()
//    {
//        if (!_isGhostMode && !_isZiplineMode && _jumpBufferCounter > 0f)
//        {
//            if (_isSpiderMode && _isGrounded)
//            {
//                PerformSpiderTeleport();
//                _jumpBufferCounter = 0f;
//            }
//            else if (_isGravityMode && _isGrounded)
//            {
//                FlipGravity();
//                _jumpBufferCounter = 0f;
//            }
//            else if (!_isSpiderMode && !_isGravityMode && (_coyoteTimeCounter > 0f || _jumpsLeft > 0))
//            {
//                _jumpRequested = true;
//                if (_coyoteTimeCounter <= 0f) _jumpsLeft--;
//                _jumpBufferCounter = 0f;
//            }
//        }
//    }

//    private void HandleZiplineExit()
//    {
//        if (_isZiplineMode && !_isHoldingInput)
//        {
//            ExitZipline(false);
//        }
//    }

//    private void FixedUpdate()
//    {
//        CheckFrontCollision(); 

//        if (_isZiplineMode && _currentZipline != null)
//        {
//            ProcessZiplineMovement();
//            return;
//        }

//        if (!_rb.isKinematic)
//        {
//            ProcessStandardPhysics();
//        }
//    }

//    private void ProcessZiplineMovement()
//    {
//        Vector3 zipDirection = _currentZipline.right;
//        Vector3 zipOrigin = _currentZipline.position;
//        Vector3 playerDelta = transform.position - zipOrigin;
//        Vector3 projectedDelta = Vector3.Project(playerDelta, zipDirection);
//        Vector3 idealPositionOnLine = zipOrigin + projectedDelta;

//        float stepDistance = (_ziplineBaseSpeed * _currentZiplineSpeedMod) * Time.fixedDeltaTime;
//        Vector3 nextPosition = idealPositionOnLine + (zipDirection * stepDistance);
//        Vector3 smoothedPos = Vector3.Lerp(transform.position, nextPosition, Time.fixedDeltaTime * _ziplineSnapSpeed);

//        _rb.MovePosition(smoothedPos);

//        if (_visualModel != null)
//        {
//            Quaternion targetRot = Quaternion.LookRotation(zipDirection);
//            _visualModel.rotation = Quaternion.Lerp(_visualModel.rotation, targetRot, Time.fixedDeltaTime * 10f);
//        }
//    }

//    private void ProcessStandardPhysics()
//    {
//        Vector3 customGravity = Vector3.down * _gravityForce * _gravityScale;
//        _rb.AddForce(customGravity, ForceMode.Acceleration);

//        Vector3 currentVel = _rb.velocity;
//        currentVel.x = _forwardSpeed;

//        if (_jumpRequested)
//        {
//            currentVel.y = _jumpForce * _gravityScale;
//            if (_jumpParticles) _jumpParticles.Play();
//            if (_audioSource && _jumpSfx) _audioSource.PlayOneShot(_jumpSfx);
//            _jumpRequested = false;
//            _coyoteTimeCounter = 0f;
//        }

//        _rb.velocity = currentVel;
//    }



//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Zipline"))
//        {

//            if (_isHoldingInput) EnterZipline(other.transform);
//        }
//        else
//        {
//            int hitLayer = other.gameObject.layer;
//            bool isObstacle = (_deadlyLayer != -1 && hitLayer == _deadlyLayer) || other.CompareTag("Obstacle");

//            if (isObstacle)
//            {
//                if (_isGhostMode && _isPhasing) return;
//                Die();
//            }
//        }
//    }

//    private void OnTriggerStay(Collider other)
//    {
//        if (other.CompareTag("Zipline") && !_isZiplineMode)
//        {
//            if (_isHoldingInput) EnterZipline(other.transform);
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (_isZiplineMode && other.CompareTag("Zipline") && other.transform == _currentZipline)
//        {
//            ExitZipline(false);
//        }
//    }

//    private void EnterZipline(Transform zipTransform)
//    {
//        _isZiplineMode = true;
//        _currentZipline = zipTransform;
//        _jumpBufferCounter = 0f;

//        ZiplineObject zipObj = zipTransform.GetComponent<ZiplineObject>();
//        _currentZiplineSpeedMod = (zipObj != null) ? zipObj.speedMultiplier : 1f;
//        _rb.velocity = zipTransform.right * (_ziplineBaseSpeed * _currentZiplineSpeedMod);
//    }

//    private void ExitZipline(bool jumpOut)
//    {
//        _isZiplineMode = false;
//        _currentZipline = null;

//        if (_visualModel != null) _visualModel.rotation = Quaternion.identity;

//        if (!jumpOut)
//        {
//            Vector3 exitVel = _rb.velocity;
//            exitVel.x = _forwardSpeed;
//            _rb.velocity = exitVel;
//        }
//    }

//    private void CheckFrontCollision()
//    {
//        if (_isDead || (_isGhostMode && _isPhasing) || _isZiplineMode) return;

//        float rayLength = 0.51f + (Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime);
//        Vector3 originTop = transform.position + Vector3.up * 0.4f;
//        Vector3 originBottom = transform.position + Vector3.up * -0.4f;

//        RaycastHit hit;
//        bool hasHit = Physics.Raycast(originTop, Vector3.right, out hit, rayLength, _groundMask | (1 << _deadlyLayer)) ||
//                      Physics.Raycast(originBottom, Vector3.right, out hit, rayLength, _groundMask | (1 << _deadlyLayer));

//        if (hasHit && hit.collider != null && !hit.collider.isTrigger)
//        {
//            Die();
//        }
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (_isDead || (_isGhostMode && _isPhasing) || _isZiplineMode) return;

//        foreach (ContactPoint contact in collision.contacts)
//        {
//            if (_gravityScale > 0 && contact.normal.y > 0.5f) return;
//            if (_gravityScale < 0 && contact.normal.y < -0.5f) return;

//            int hitLayer = collision.gameObject.layer;
//            if (hitLayer == _safeLayer || hitLayer == _wallLayer || hitLayer == _deadlyLayer)
//            {
//                if (contact.normal.x < -0.1f)
//                {
//                    Die();
//                    return;
//                }
//            }
//        }
//    }

//    private void SetPhasingState(bool active)
//    {
//        if (_isPhasing == active) return;
//        _isPhasing = active;

//        if (_modelRenderers != null)
//        {
//            foreach (var r in _modelRenderers)
//            {
//                foreach (var mat in r.materials)
//                {
//                    Color c = mat.color;
//                    c.a = active ? _ghostAlpha : 1f;
//                    mat.color = c;
//                }
//            }
//        }

//        if (_deadlyLayer != -1) Physics.IgnoreLayerCollision(_playerLayer, _deadlyLayer, active);
//        if (_wallLayer != -1) Physics.IgnoreLayerCollision(_playerLayer, _wallLayer, active);
//    }

//    public void Die()
//    {
//        if (_isDead) return;
//        _isDead = true;
//        _isZiplineMode = false;

//        LaneRunner3D runner = GetComponent<LaneRunner3D>();
//        if (runner != null) runner.enabled = false;

//        if (!_rb.isKinematic)
//        {
//            _rb.velocity = Vector3.zero;
//            _rb.angularVelocity = Vector3.zero;
//        }

//        _rb.isKinematic = true;
//        _rb.detectCollisions = false;

//        if (_myCollider != null) _myCollider.enabled = false;
//        if (_trail != null) _trail.emitting = false;

//        if (_deathParticles) Instantiate(_deathParticles, transform.position, Quaternion.identity);
//        if (_audioSource && _deathSfx) _audioSource.PlayOneShot(_deathSfx);

//        StartCoroutine(DeathAnimation());
//    }

//    private IEnumerator DeathAnimation()
//    {
//        float t = 0f;
//        Vector3 startScale = _visualModel != null ? _visualModel.localScale : transform.localScale;

//        while (t < 0.3f)
//        {
//            t += Time.unscaledDeltaTime;
//            Vector3 newScale = Vector3.Lerp(startScale, Vector3.zero, t / 0.3f);
//            if (_visualModel != null) _visualModel.localScale = newScale; else transform.localScale = newScale;
//            yield return null;
//        }

//        if (_visualModel != null) _visualModel.localScale = Vector3.zero; else transform.localScale = Vector3.zero;
//        if (_levelManager) _levelManager.PlayerDied(this);
//    }

//    public void RespawnAt(Transform spawnPoint)
//    {
//        if (spawnPoint == null) return;

//        _isDead = false;
//        _jumpRequested = false;
//        _jumpBufferCounter = 0f;
//        _coyoteTimeCounter = 0f;
//        _forwardSpeed = _defaultSpeed;

//        if (_myCollider != null) _myCollider.enabled = false;
//        _rb.isKinematic = true;
//        _rb.detectCollisions = false;

//        transform.position = spawnPoint.position;
//        transform.rotation = spawnPoint.rotation;

//        if (_visualModel != null)
//        {
//            _visualModel.localScale = Vector3.one;
//            _visualModel.localRotation = Quaternion.identity;
//        }
//        else
//        {
//            transform.localScale = _originalScale;
//        }

//        if (_trail != null) { _trail.Clear(); _trail.emitting = true; }
//        if (_spiderLine != null) _spiderLine.enabled = false;

//        Physics.SyncTransforms();

//        LaneRunner3D runnerScript = GetComponent<LaneRunner3D>();
//        if (runnerScript != null) runnerScript.enabled = false;

//        this.enabled = true;

//        if (_cameraFollow)
//        {
//            _cameraFollow.SetGravityFlipped(false);
//            _cameraFollow.Set3DView(false);
//        }

//        _rb.constraints = RigidbodyConstraints.FreezeRotation;

//        if (_myCollider != null) _myCollider.enabled = true;
//        _rb.isKinematic = false;
//        _rb.detectCollisions = true;
//        _rb.velocity = Vector3.zero;
//        _rb.angularVelocity = Vector3.zero;

//        SetMode("Cube");
//        _gravityScale = 1f;
//        _jumpsLeft = _extraAirJumps;
//        _isZiplineMode = false;

//        Vector3 flatPos = transform.position;
//        flatPos.z = 0f;
//        transform.position = flatPos;
//    }

//    private void PerformSpiderTeleport()
//    {
//        Vector3 searchDirection = _gravityScale > 0 ? Vector3.up : Vector3.down;
//        RaycastHit hit;

//        if (Physics.Raycast(transform.position, searchDirection, out hit, Mathf.Infinity, _groundMask))
//        {
//            if (_audioSource && _gravitySwitchSfx) _audioSource.PlayOneShot(_gravitySwitchSfx);
//            if (_spiderTeleportParticles) Instantiate(_spiderTeleportParticles, transform.position, Quaternion.identity);

//            Vector3 startPos = transform.position;
//            float playerHeightOffset = 0.5f;
//            Vector3 targetPosition = hit.point + (searchDirection * -1 * playerHeightOffset);
//            Vector3 finalPos = new Vector3(transform.position.x, targetPosition.y, transform.position.z);

//            _rb.interpolation = RigidbodyInterpolation.None;
//            transform.position = finalPos;
//            Physics.SyncTransforms();
//            _rb.interpolation = RigidbodyInterpolation.Interpolate;

//            if (_trail != null) _trail.Clear();
//            if (_spiderLandParticles) Instantiate(_spiderLandParticles, finalPos, Quaternion.LookRotation(hit.normal));

//            StartCoroutine(DrawSpiderBeam(startPos, finalPos));

//            FlipGravity();

//            _rb.velocity = new Vector3(_rb.velocity.x, 0, 0);
//        }
//    }

//    private IEnumerator DrawSpiderBeam(Vector3 start, Vector3 end)
//    {
//        if (_spiderLine != null)
//        {
//            _spiderLine.enabled = true;
//            _spiderLine.SetPosition(0, start);
//            _spiderLine.SetPosition(1, end);
//            yield return new WaitForSeconds(_spiderBeamDuration);
//            _spiderLine.enabled = false;
//        }
//    }

//    public void FlipGravity()
//    {
//        _gravityScale *= -1;

//        if (_cameraFollow != null)
//            _cameraFollow.SetGravityFlipped(_gravityScale < 0);
//    }

//    public void SetMode(string modeName)
//    {
//        _isGravityMode = false; _isSpiderMode = false; _isGhostMode = false; SetPhasingState(false);
//        if (_visualModel != null) _visualModel.localScale = Vector3.one;

//        if (modeName == "Cube")
//        {
//            _gravityScale = 1f;
//            if (_cameraFollow) _cameraFollow.SetGravityFlipped(false);
//        }
//        else if (modeName == "Ball") _isGravityMode = true;
//        else if (modeName == "Spider") _isSpiderMode = true;
//        else if (modeName == "Ghost") _isGhostMode = true;
//    }

//    public void SetSFXVolume(float volume) { if (_audioSource != null) _audioSource.volume = volume; }

//    private void GroundCheck()
//    {
//        RaycastHit hit;
//        Vector3 origin = transform.position + _groundCheckOffset;
//        Vector3 checkDirection = _gravityScale > 0 ? Vector3.down : Vector3.up;
//        _isGrounded = Physics.Raycast(origin, checkDirection, out hit, _groundCheckDistance, _groundMask);
//    }

//    public void Win()
//    {
//        _rb.velocity = Vector3.zero;
//        _rb.isKinematic = true;
//        if (_myCollider != null) _myCollider.enabled = false;
//        enabled = false;
//    }

//    public void ResetJumpsFromPad() { _jumpsLeft = _extraAirJumps; }
//}
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Transform _visualModel;
    [SerializeField] private TrailRenderer _trail;

    [Header("Ghost Settings")]
    [SerializeField] private float _ghostAlpha = 0.3f;
    [SerializeField] private string _safeLayerName = "Ground";
    [SerializeField] private string _deadlyLayerName = "Obstacle";
    [SerializeField] private string _wallLayerName = "Wall";

    [Header("Zipline Settings")]
    [SerializeField] private float _ziplineBaseSpeed = 15f;
    [SerializeField] private float _ziplineSnapSpeed = 20f;

    [Header("Spider Settings")]
    [SerializeField] private float _spiderBeamDuration = 0.1f;

    [Header("Movement")]
    [Tooltip("Кількість ДОДАТКОВИХ стрибків у повітрі. 0 = лише звичайний, 1 = подвійний стрибок.")]
    [SerializeField] private int _extraAirJumps = 1;
    [SerializeField] private float _forwardSpeed = 10f;
    [SerializeField] private float _jumpForce = 12f;
    [SerializeField] private float _jumpBufferTime = 0.15f;
    [SerializeField] private float _coyoteTime = 0.1f;

    [Header("Modes Settings")]
    [SerializeField] private float _gravityForce = 30f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundCheckDistance = 0.6f;
    [SerializeField] private Vector3 _groundCheckOffset = Vector3.zero;

    [Header("Effects (Object Pool)")]
    [SerializeField] private GameObject _jumpParticles;
    [SerializeField] private string _jumpPoolTag = "JumpDust";

    [SerializeField] private GameObject _deathParticles;
    [SerializeField] private string _deathPoolTag = "DeathExplosion";

    [SerializeField] private GameObject _spiderTeleportParticles;
    [SerializeField] private string _spiderTeleportPoolTag = "Spark";

    [SerializeField] private GameObject _spiderLandParticles;
    [SerializeField] private string _spiderLandPoolTag = "SpiderLand";

    [Header("Audio SFX")]
    [SerializeField] private AudioClip _jumpSfx;
    [SerializeField] private AudioClip _deathSfx;
    [SerializeField] private AudioClip _gravitySwitchSfx;


    private Rigidbody _rb;
    private Collider _myCollider;
    private AudioSource _audioSource;
    private LevelManager _levelManager;
    private CameraFollow _cameraFollow;
    private LineRenderer _spiderLine;
    private Renderer[] _modelRenderers;

    private int _playerLayer, _safeLayer, _deadlyLayer, _wallLayer;
    private float _defaultSpeed;
    private Vector3 _originalScale;

    private float _gravityScale = 1f;
    private int _jumpsLeft;
    private float _jumpBufferCounter, _coyoteTimeCounter;

    private bool _isDead = false;
    private bool _isGrounded = false;
    private bool _jumpRequested = false;
    private bool _isHoldingInput = false;

    private bool _isGravityMode = false;
    private bool _isSpiderMode = false;
    private bool _isGhostMode = false;
    private bool _isPhasing = false;
    private bool _isZiplineMode = false;

    private Transform _currentZipline;
    private float _currentZiplineSpeedMod = 1f;

    public float ForwardSpeed
    {
        get => _forwardSpeed;
        set => _forwardSpeed = value;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _myCollider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();
        _levelManager = FindObjectOfType<LevelManager>();
        _cameraFollow = FindObjectOfType<CameraFollow>();
        _spiderLine = GetComponent<LineRenderer>();

        if (_spiderLine != null) _spiderLine.enabled = false;

        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rb.useGravity = false;

        if (_audioSource != null)
        {
            _audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }

        _defaultSpeed = _forwardSpeed;
        _originalScale = transform.localScale;

        if (_visualModel != null)
            _modelRenderers = _visualModel.GetComponentsInChildren<Renderer>();

        InitializeLayers();
    }

    private void InitializeLayers()
    {
        _playerLayer = gameObject.layer;
        _safeLayer = LayerMask.NameToLayer(_safeLayerName);
        _deadlyLayer = LayerMask.NameToLayer(_deadlyLayerName);
        _wallLayer = LayerMask.NameToLayer(_wallLayerName);

        if (_wallLayer != -1) _groundMask |= (1 << _wallLayer);
    }

    private void Update()
    {
        if (_isDead) return;

        GroundCheck();
        HandleInput();
        UpdateTimers();
        ProcessJumpLogic();
        HandleZiplineExit();
    }

    private void HandleInput()
    {
        _isHoldingInput = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || (Input.touchCount > 0);

        bool isPressedDown = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ||
                             (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        if (_isGhostMode)
        {
            SetPhasingState(_isHoldingInput);
        }
        else
        {
            if (_isPhasing) SetPhasingState(false);

            if (isPressedDown)
            {
                if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
                    _jumpBufferCounter = _jumpBufferTime;
            }
            else
            {
                _jumpBufferCounter -= Time.deltaTime;
            }
        }
    }

    private void UpdateTimers()
    {
        if (_isGrounded || _isZiplineMode)
        {
            _coyoteTimeCounter = _coyoteTime;
            _jumpsLeft = _extraAirJumps;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void ProcessJumpLogic()
    {
        if (!_isGhostMode && !_isZiplineMode && _jumpBufferCounter > 0f)
        {
            if (_isSpiderMode && _isGrounded)
            {
                PerformSpiderTeleport();
                _jumpBufferCounter = 0f;
            }
            else if (_isGravityMode && _isGrounded)
            {
                FlipGravity();
                _jumpBufferCounter = 0f;
            }
            else if (!_isSpiderMode && !_isGravityMode && (_coyoteTimeCounter > 0f || _jumpsLeft > 0))
            {
                _jumpRequested = true;
                if (_coyoteTimeCounter <= 0f) _jumpsLeft--;
                _jumpBufferCounter = 0f;
            }
        }
    }

    private void HandleZiplineExit()
    {
        if (_isZiplineMode && !_isHoldingInput)
        {
            ExitZipline(false);
        }
    }

    private void FixedUpdate()
    {
        if (_isZiplineMode && _currentZipline != null)
        {
            ProcessZiplineMovement();
            return;
        }

        if (!_rb.isKinematic)
        {
            ProcessStandardPhysics();
        }
    }

    private void ProcessZiplineMovement()
    {
        Vector3 zipDirection = _currentZipline.right;
        Vector3 zipOrigin = _currentZipline.position;

        Vector3 playerDelta = _rb.position - zipOrigin;
        Vector3 projectedDelta = Vector3.Project(playerDelta, zipDirection);
        Vector3 exactPositionOnLine = zipOrigin + projectedDelta;

        Vector3 snapVector = exactPositionOnLine - _rb.position;
        Vector3 targetVelocity = (zipDirection * _ziplineBaseSpeed * _currentZiplineSpeedMod) + (snapVector * _ziplineSnapSpeed);

        _rb.velocity = targetVelocity;

        if (_visualModel != null)
        {
            Quaternion targetRot = Quaternion.LookRotation(zipDirection);
            _visualModel.rotation = Quaternion.Lerp(_visualModel.rotation, targetRot, Time.fixedDeltaTime * 15f);
        }
    }

    private void ProcessStandardPhysics()
    {
        Vector3 customGravity = Vector3.down * _gravityForce * _gravityScale;
        _rb.AddForce(customGravity, ForceMode.Acceleration);

        Vector3 currentVel = _rb.velocity;
        currentVel.x = _forwardSpeed;

        if (_jumpRequested)
        {
            currentVel.y = _jumpForce * _gravityScale;

            if (_jumpParticles != null)
            {
                if (PoolManager.Instance != null)
                    PoolManager.Instance.SpawnFromPool(_jumpPoolTag, transform.position, Quaternion.identity);
                else
                    Instantiate(_jumpParticles, transform.position, Quaternion.identity);
            }

            if (_audioSource && _jumpSfx) _audioSource.PlayOneShot(_jumpSfx);
            _jumpRequested = false;
            _coyoteTimeCounter = 0f;
        }

        _rb.velocity = currentVel;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zipline"))
        {
            if (_isHoldingInput) EnterZipline(other.transform);
        }
        else
        {
            int hitLayer = other.gameObject.layer;
            bool isObstacle = (_deadlyLayer != -1 && hitLayer == _deadlyLayer) || other.CompareTag("Obstacle");

            if (isObstacle)
            {
                if (_isGhostMode && _isPhasing) return;
                Die();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Zipline") && !_isZiplineMode)
        {
            if (_isHoldingInput) EnterZipline(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isZiplineMode && other.CompareTag("Zipline") && other.transform == _currentZipline)
        {
            ExitZipline(false);
        }
    }

    private void EnterZipline(Transform zipTransform)
    {
        _isZiplineMode = true;
        _currentZipline = zipTransform;
        _jumpBufferCounter = 0f;

        ZiplineObject zipObj = zipTransform.GetComponent<ZiplineObject>();
        _currentZiplineSpeedMod = (zipObj != null) ? zipObj.SpeedMultiplier : 1f;

    }

    private void ExitZipline(bool jumpOut)
    {
        _isZiplineMode = false;
        _currentZipline = null;

        if (_visualModel != null) _visualModel.rotation = Quaternion.identity;

        if (!jumpOut)
        {
            Vector3 exitVel = _rb.velocity;
            exitVel.x = _forwardSpeed;
            _rb.velocity = exitVel;
        }
    }


    private void OnCollisionEnter(Collision collision) { ProcessPhysicalHit(collision); }
    private void OnCollisionStay(Collision collision) { ProcessPhysicalHit(collision); }

    private void ProcessPhysicalHit(Collision collision)
    {
        if (_isDead || (_isGhostMode && _isPhasing) || _isZiplineMode) return;

        int hitLayer = collision.gameObject.layer;

        
        if (hitLayer == _deadlyLayer || collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
            return;
        }

       
        if (hitLayer == _safeLayer || hitLayer == _wallLayer)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                
                if (contact.normal.x < -0.1f)
                {
                    float hitHeightDiff = contact.point.y - transform.position.y;

                    
                    if (_gravityScale > 0 && hitHeightDiff > -0.3f)
                    {
                       
                        Die();
                        return;
                    }
                    else if (_gravityScale < 0 && hitHeightDiff < 0.3f)
                    {
                        
                        Die();
                        return;
                    }
                }
            }
        }
    }

    private void SetPhasingState(bool active)
    {
        if (_isPhasing == active) return;
        _isPhasing = active;

        if (_modelRenderers != null)
        {
            foreach (var r in _modelRenderers)
            {
                foreach (var mat in r.materials)
                {
                    Color c = mat.color;
                    c.a = active ? _ghostAlpha : 1f;
                    mat.color = c;
                }
            }
        }

        if (_deadlyLayer != -1) Physics.IgnoreLayerCollision(_playerLayer, _deadlyLayer, active);
        if (_wallLayer != -1) Physics.IgnoreLayerCollision(_playerLayer, _wallLayer, active);
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        _isZiplineMode = false;

        LaneRunner3D runner = GetComponent<LaneRunner3D>();
        if (runner != null) runner.enabled = false;

        if (!_rb.isKinematic)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        _rb.isKinematic = true;
        _rb.detectCollisions = false;

        if (_myCollider != null) _myCollider.enabled = false;
        if (_trail != null) _trail.emitting = false;

        if (_deathParticles != null)
        {
            if (PoolManager.Instance != null)
                PoolManager.Instance.SpawnFromPool(_deathPoolTag, transform.position, Quaternion.identity);
            else
                Instantiate(_deathParticles, transform.position, Quaternion.identity);
        }

        if (_audioSource && _deathSfx) _audioSource.PlayOneShot(_deathSfx);

        StartCoroutine(DeathAnimation());
    }

    private IEnumerator DeathAnimation()
    {
        float t = 0f;
        Vector3 startScale = _visualModel != null ? _visualModel.localScale : transform.localScale;

        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            Vector3 newScale = Vector3.Lerp(startScale, Vector3.zero, t / 0.3f);
            if (_visualModel != null) _visualModel.localScale = newScale; else transform.localScale = newScale;
            yield return null;
        }

        if (_visualModel != null) _visualModel.localScale = Vector3.zero; else transform.localScale = Vector3.zero;
        if (_levelManager) _levelManager.PlayerDied(this);
    }

    public void RespawnAt(Transform spawnPoint)
    {
        if (spawnPoint == null) return;

        _isDead = false;
        _jumpRequested = false;
        _jumpBufferCounter = 0f;
        _coyoteTimeCounter = 0f;
        _forwardSpeed = _defaultSpeed;

        if (_myCollider != null) _myCollider.enabled = false;
        _rb.isKinematic = true;
        _rb.detectCollisions = false;

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        if (_visualModel != null)
        {
            _visualModel.localScale = Vector3.one;
            _visualModel.localRotation = Quaternion.identity;
        }
        else
        {
            transform.localScale = _originalScale;
        }

        if (_trail != null) { _trail.Clear(); _trail.emitting = true; }
        if (_spiderLine != null) _spiderLine.enabled = false;

        Physics.SyncTransforms();

        LaneRunner3D runnerScript = GetComponent<LaneRunner3D>();
        if (runnerScript != null) runnerScript.enabled = false;

        this.enabled = true;

        if (_cameraFollow)
        {
            _cameraFollow.SetGravityFlipped(false);
            _cameraFollow.Set3DView(false);
        }

        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (_myCollider != null) _myCollider.enabled = true;
        _rb.isKinematic = false;
        _rb.detectCollisions = true;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        SetMode("Cube");
        _gravityScale = 1f;
        _jumpsLeft = _extraAirJumps;
        _isZiplineMode = false;

        Vector3 flatPos = transform.position;
        flatPos.z = 0f;
        transform.position = flatPos;
    }

    private void PerformSpiderTeleport()
    {
        Vector3 searchDirection = _gravityScale > 0 ? Vector3.up : Vector3.down;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, searchDirection, out hit, Mathf.Infinity, _groundMask))
        {
            if (_audioSource && _gravitySwitchSfx) _audioSource.PlayOneShot(_gravitySwitchSfx);

            if (_spiderTeleportParticles != null)
            {
                if (PoolManager.Instance != null)
                    PoolManager.Instance.SpawnFromPool(_spiderTeleportPoolTag, transform.position, Quaternion.identity);
                else
                    Instantiate(_spiderTeleportParticles, transform.position, Quaternion.identity);
            }

            Vector3 startPos = transform.position;
            float playerHeightOffset = 0.5f;
            Vector3 targetPosition = hit.point + (searchDirection * -1 * playerHeightOffset);
            Vector3 finalPos = new Vector3(transform.position.x, targetPosition.y, transform.position.z);

            _rb.interpolation = RigidbodyInterpolation.None;
            transform.position = finalPos;
            Physics.SyncTransforms();
            _rb.interpolation = RigidbodyInterpolation.Interpolate;

            if (_trail != null) _trail.Clear();

            if (_spiderLandParticles != null)
            {
                if (PoolManager.Instance != null)
                    PoolManager.Instance.SpawnFromPool(_spiderLandPoolTag, finalPos, Quaternion.LookRotation(hit.normal));
                else
                    Instantiate(_spiderLandParticles, finalPos, Quaternion.LookRotation(hit.normal));
            }

            StartCoroutine(DrawSpiderBeam(startPos, finalPos));
            FlipGravity();
            _rb.velocity = new Vector3(_rb.velocity.x, 0, 0);
        }
    }

    private IEnumerator DrawSpiderBeam(Vector3 start, Vector3 end)
    {
        if (_spiderLine != null)
        {
            _spiderLine.enabled = true;
            _spiderLine.SetPosition(0, start);
            _spiderLine.SetPosition(1, end);
            yield return new WaitForSeconds(_spiderBeamDuration);
            _spiderLine.enabled = false;
        }
    }

    public void FlipGravity()
    {
        _gravityScale *= -1;
        if (_cameraFollow != null) _cameraFollow.SetGravityFlipped(_gravityScale < 0);
    }

    public void SetMode(string modeName)
    {
        _isGravityMode = false; _isSpiderMode = false; _isGhostMode = false; SetPhasingState(false);
        if (_visualModel != null) _visualModel.localScale = Vector3.one;

        if (modeName == "Cube")
        {
            _gravityScale = 1f;
            if (_cameraFollow) _cameraFollow.SetGravityFlipped(false);
        }
        else if (modeName == "Ball") _isGravityMode = true;
        else if (modeName == "Spider") _isSpiderMode = true;
        else if (modeName == "Ghost") _isGhostMode = true;
    }

    public void SetSFXVolume(float volume) { if (_audioSource != null) _audioSource.volume = volume; }

    private void GroundCheck()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + _groundCheckOffset;
        Vector3 checkDirection = _gravityScale > 0 ? Vector3.down : Vector3.up;
        _isGrounded = Physics.Raycast(origin, checkDirection, out hit, _groundCheckDistance, _groundMask);
    }

    public void Win()
    {
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
        if (_myCollider != null) _myCollider.enabled = false;
        enabled = false;
    }

    public void ResetJumpsFromPad() { _jumpsLeft = _extraAirJumps; }
}