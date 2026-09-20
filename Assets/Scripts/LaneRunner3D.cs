using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class LaneRunner3D : MonoBehaviour
{
    [Header("Налаштування руху")]
    [SerializeField] private float _forwardSpeed = 15f;
    [SerializeField] private float _laneDistance = 3.0f;
    [SerializeField] private float _extraGravity = 30f;

    [Header("Ефекти (Object Pool)")]
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private string _ghostPoolTag = "Ghost";

    [SerializeField] private GameObject _teleportEffect;
    [SerializeField] private string _teleportPoolTag = "Spark";
    [SerializeField] private AudioClip _teleportSound;

    private readonly int[] _laneSequence = { 0, 1, 0, -1 };
    private int _sequenceIndex = 0;

    private Rigidbody _rb;
    private PlayerController _mainController;
    private AudioSource _audioSource;

    private bool _isDead = false;
    private int _obstacleLayer;

    public float ForwardSpeed
    {
        get => _forwardSpeed;
        set => _forwardSpeed = value;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mainController = GetComponent<PlayerController>();
        _audioSource = GetComponent<AudioSource>();

        _obstacleLayer = LayerMask.NameToLayer("Obstacle");

        enabled = false;
    }

    private void OnEnable()
    {
        ResetRun();
    }

    private void OnDisable()
    {
        if (_rb != null)
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void ResetRun()
    {
        _isDead = false;
        _sequenceIndex = 0;

        if (_rb != null)
        {
            _rb.isKinematic = false;
            _rb.velocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private void Update()
    {
        if (_isDead)
        {
            if (_rb != null && !_rb.isKinematic)
            {
                ResetRun();
            }
            return;
        }

        bool isTap = Input.GetButtonDown("Jump") ||
                     Input.GetMouseButtonDown(0) ||
                     (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        if (isTap)
        {
            SwitchLane();
        }
    }

    private void FixedUpdate()
    {
        if (_isDead) return;

        float currentYVelocity = _rb.velocity.y - (_extraGravity * Time.fixedDeltaTime);

        Vector3 forwardDir = _mainController.MoveDirection;

        _rb.velocity = new Vector3(forwardDir.x * _forwardSpeed, currentYVelocity, forwardDir.z * _forwardSpeed);
    }

    private void SwitchLane()
    {
        Vector3 oldPosition = transform.position;

        int oldLane = _laneSequence[_sequenceIndex];
        _sequenceIndex++;
        if (_sequenceIndex >= _laneSequence.Length) _sequenceIndex = 0;
        int newLane = _laneSequence[_sequenceIndex];

        int laneDiff = newLane - oldLane;

        Vector3 rightDir = Vector3.Cross(Vector3.up, _mainController.MoveDirection).normalized;

        Vector3 newPosition = oldPosition + (rightDir * (laneDiff * _laneDistance));

        if (_ghostPrefab != null)
        {
            GameObject ghost = (PoolManager.Instance != null)
                ? PoolManager.Instance.SpawnFromPool(_ghostPoolTag, oldPosition, transform.rotation)
                : Instantiate(_ghostPrefab, oldPosition, transform.rotation);

            if (ghost != null) ghost.transform.localScale = transform.localScale;
        }

        _rb.interpolation = RigidbodyInterpolation.None;
        transform.position = newPosition;
        _rb.position = newPosition;
        Physics.SyncTransforms();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (_teleportSound && _audioSource) _audioSource.PlayOneShot(_teleportSound);

        if (_teleportEffect != null)
        {
            GameObject teleportObj = (PoolManager.Instance != null)
                ? PoolManager.Instance.SpawnFromPool(_teleportPoolTag, newPosition, Quaternion.identity)
                : Instantiate(_teleportEffect, newPosition, Quaternion.identity);

            if (teleportObj != null && teleportObj.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
            {
                ps.Emit(10);
            }
        }
    }

    public void DisableAndSnapToCenter()
    {
        int currentLaneOffset = _laneSequence[_sequenceIndex];

        if (currentLaneOffset != 0 && _mainController != null)
        {
            float offsetAmount = currentLaneOffset * _laneDistance;

            Vector3 rightDir = Vector3.Cross(Vector3.up, _mainController.MoveDirection).normalized;

            _rb.interpolation = RigidbodyInterpolation.None;
            transform.position -= (rightDir * offsetAmount);
            _rb.position = transform.position;
            Physics.SyncTransforms();
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        _sequenceIndex = 0;
        enabled = false;
    }

    private void OnCollisionEnter(Collision collision) { CheckDamage(collision.gameObject); }
    private void OnTriggerEnter(Collider other) { CheckDamage(other.gameObject); }

    private void CheckDamage(GameObject obj)
    {
        if (_isDead) return;

        if (obj.CompareTag("Obstacle") || obj.layer == _obstacleLayer)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (_isDead) return;
        _isDead = true;

        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;

        if (_mainController != null)
        {
            _mainController.Die();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}