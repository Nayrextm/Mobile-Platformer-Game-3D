//using UnityEngine;
//using UnityEngine.SceneManagement;

//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(AudioSource))]
//public class LaneRunner3D : MonoBehaviour
//{
//    [Header("Налаштування руху")]
//    [SerializeField] private float _forwardSpeed = 15f;
//    [SerializeField] private float _laneDistance = 3.0f; 
//    [SerializeField] private float _extraGravity = 30f;  

//    [Header("Ефекти")]
//    [SerializeField] private GameObject _ghostPrefab;
//    [SerializeField] private ParticleSystem _teleportEffect;
//    [SerializeField] private AudioClip _teleportSound;


//    private readonly int[] _laneSequence = { 0, 1, 0, -1 };
//    private int _sequenceIndex = 0;

//    private Rigidbody _rb;
//    private PlayerController _mainController;
//    private AudioSource _audioSource;

//    private bool _isDead = false;
//    private int _obstacleLayer;

//    public float ForwardSpeed
//    {
//        get => _forwardSpeed;
//        set => _forwardSpeed = value;
//    }

//    private void Awake()
//    {
//        _rb = GetComponent<Rigidbody>();
//        _mainController = GetComponent<PlayerController>();
//        _audioSource = GetComponent<AudioSource>();


//        _obstacleLayer = LayerMask.NameToLayer("Obstacle");


//        enabled = false;
//    }

//    private void OnEnable()
//    {
//        ResetRun();
//    }

//    private void OnDisable()
//    {
//        if (_rb != null)
//        {
//            _rb.constraints = RigidbodyConstraints.FreezeRotation;
//        }

//        if (_teleportEffect != null)
//        {
//            _teleportEffect.Stop();
//            _teleportEffect.Clear();
//        }
//    }

//    public void ResetRun()
//    {
//        _isDead = false;
//        _sequenceIndex = 0;

//        if (_rb != null)
//        {
//            _rb.isKinematic = false;
//            _rb.velocity = Vector3.zero;
//            _rb.constraints = RigidbodyConstraints.FreezeRotation;

//            SnapToLane(_laneSequence[_sequenceIndex]);
//        }
//    }

//    private void Update()
//    {
//        if (_isDead)
//        {

//            if (_rb != null && !_rb.isKinematic)
//            {
//                ResetRun();
//            }
//            return;
//        }


//        bool isTap = Input.GetButtonDown("Jump") ||
//                     Input.GetMouseButtonDown(0) ||
//                     (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

//        if (isTap)
//        {
//            SwitchLane();
//        }
//    }

//    private void FixedUpdate()
//    {
//        if (_isDead) return;


//        float currentYVelocity = _rb.velocity.y;
//        currentYVelocity -= _extraGravity * Time.fixedDeltaTime;


//        Vector3 targetVelocity = new Vector3(_forwardSpeed, currentYVelocity, 0);
//        _rb.velocity = targetVelocity;


//        Vector3 currentPos = _rb.position;
//        float targetZ = _laneSequence[_sequenceIndex] * _laneDistance;

//        Vector3 newPos = new Vector3(currentPos.x, currentPos.y, targetZ);
//        _rb.MovePosition(newPos);
//    }

//    private void SwitchLane()
//    {
//        Vector3 oldPosition = transform.position;

//        _sequenceIndex++;
//        if (_sequenceIndex >= _laneSequence.Length) _sequenceIndex = 0;

//        float targetZ = _laneSequence[_sequenceIndex] * _laneDistance;
//        Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, targetZ);


//        if (_ghostPrefab != null)
//        {
//            GameObject ghost = Instantiate(_ghostPrefab, oldPosition, transform.rotation);
//            ghost.transform.localScale = transform.localScale;


//        }


//        transform.position = newPosition;
//        _rb.position = newPosition;

//        if (_teleportSound && _audioSource) _audioSource.PlayOneShot(_teleportSound);

//        if (_teleportEffect != null)
//        {
//            ParticleSystem newEffect = Instantiate(_teleportEffect, transform.position, Quaternion.identity);
//            newEffect.Emit(30);
//            Destroy(newEffect.gameObject, 1.5f); 
//        }
//    }

//    private void SnapToLane(int laneIndex)
//    {
//        Vector3 pos = transform.position;
//        pos.z = laneIndex * _laneDistance;
//        transform.position = pos;
//    }

//    private void OnCollisionEnter(Collision collision) { CheckDamage(collision.gameObject); }
//    private void OnTriggerEnter(Collider other) { CheckDamage(other.gameObject); }

//    private void CheckDamage(GameObject obj)
//    {
//        if (_isDead) return;


//        if (obj.CompareTag("Obstacle") || obj.layer == _obstacleLayer)
//        {
//            HandleDeath();
//        }
//    }

//    private void HandleDeath()
//    {
//        if (_isDead) return;
//        _isDead = true;

//        _rb.velocity = Vector3.zero;
//        _rb.isKinematic = true;

//        if (_mainController != null)
//        {
//            _mainController.Die();
//        }
//        else
//        {
//            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//        }
//    }
//}
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

            SnapToLane(_laneSequence[_sequenceIndex]);
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

        float currentYVelocity = _rb.velocity.y;
        currentYVelocity -= _extraGravity * Time.fixedDeltaTime;

        Vector3 currentPos = _rb.position;
        float targetZ = _laneSequence[_sequenceIndex] * _laneDistance;

        float currentZVelocity = (targetZ - currentPos.z) * 10f;

        Vector3 targetVelocity = new Vector3(_forwardSpeed, currentYVelocity, currentZVelocity);

        _rb.velocity = targetVelocity;
    }

    private void SwitchLane()
    {
        Vector3 oldPosition = transform.position;

        _sequenceIndex++;
        if (_sequenceIndex >= _laneSequence.Length) _sequenceIndex = 0;

        float targetZ = _laneSequence[_sequenceIndex] * _laneDistance;
        Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, targetZ);


        if (_ghostPrefab != null)
        {
            GameObject ghost = null;
            if (PoolManager.Instance != null)
            {
                ghost = PoolManager.Instance.SpawnFromPool(_ghostPoolTag, oldPosition, transform.rotation);
            }
            else
            {
                ghost = Instantiate(_ghostPrefab, oldPosition, transform.rotation);
            }


            if (ghost != null)
            {
                ghost.transform.localScale = transform.localScale;
            }
        }

        transform.position = newPosition;
        _rb.position = newPosition;

        if (_teleportSound && _audioSource) _audioSource.PlayOneShot(_teleportSound);


        if (_teleportEffect != null)
        {
            GameObject teleportObj = null;
            if (PoolManager.Instance != null)
            {
                teleportObj = PoolManager.Instance.SpawnFromPool(_teleportPoolTag, transform.position, Quaternion.identity);
            }
            else
            {
                teleportObj = Instantiate(_teleportEffect, transform.position, Quaternion.identity);
            }


            if (teleportObj != null && teleportObj.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
            {
                ps.Emit(30);
            }
        }
    }

    private void SnapToLane(int laneIndex)
    {
        Vector3 pos = transform.position;
        pos.z = laneIndex * _laneDistance;
        transform.position = pos;
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