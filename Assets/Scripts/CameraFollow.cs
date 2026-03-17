//using UnityEngine;

//[RequireComponent(typeof(Camera))]
//public class CameraFollow : MonoBehaviour
//{
//    [Header("Ціль")]
//    [SerializeField] private Transform _target;

//    [Header("Налаштування 2D (Side View)")]
//    [SerializeField] private Vector3 _offset2D = new Vector3(0, 1, -15);
//    [SerializeField] private Vector3 _rotation2D = new Vector3(0, 0, 0);
//    [SerializeField] private float _fov2D = 30f;

//    [Header("Налаштування 3D (Runner View)")]
//    [SerializeField] private Vector3 _offset3D = new Vector3(0, 4, -8);
//    [SerializeField] private Vector3 _rotation3D = new Vector3(20, 0, 0);
//    [SerializeField] private float _fov3D = 60f;

//    [Header("Налаштування Плавності")]
//    [SerializeField] private float _smoothTime = 0.1f;
//    [SerializeField] private float _rotationSpeed = 5f;
//    [SerializeField] private float _fovSpeed = 5f;

//    private Vector3 _currentVelocity;
//    private Vector3 _currentOffset;
//    private bool _is3DMode = false;
//    private Camera _cam;
//    private bool _isGravityFlipped = false;

//    private void Start()
//    {
//        _cam = GetComponent<Camera>();
//        _cam.orthographic = false;
//        _currentOffset = _offset2D;
//    }

//    private void LateUpdate()
//    {
//        if (_target == null) return;

//        Vector3 baseOffset = _is3DMode ? _offset3D : _offset2D;
//        Vector3 finalTargetOffset = baseOffset;

//        if (_isGravityFlipped)
//        {
//            finalTargetOffset.y = -baseOffset.y;
//        }

//        _currentOffset = Vector3.Lerp(_currentOffset, finalTargetOffset, _fovSpeed * Time.deltaTime);
//        Vector3 targetPosition = _target.position + _currentOffset;

//        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, _smoothTime);

//        Quaternion targetRot;

//        if (_is3DMode)
//        {
//            Vector3 targetEuler = _rotation3D;

//            if (_isGravityFlipped)
//            {
//                targetEuler.x = -_rotation3D.x;
//                targetEuler.z = 180f;
//            }
//            else
//            {
//                targetEuler.z = 0f;
//            }

//            targetRot = Quaternion.Euler(targetEuler);
//        }
//        else
//        {
//            Vector3 targetEuler = _rotation2D;
//            targetEuler.z = _isGravityFlipped ? 180f : 0f;
//            targetRot = Quaternion.Euler(targetEuler);
//        }

//        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);

//        float targetFOV = _is3DMode ? _fov3D : _fov2D;
//        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFOV, _fovSpeed * Time.deltaTime);
//    }

//    public void Set3DView(bool enable3D)
//    {
//        _is3DMode = enable3D;
//    }

//    public void SetGravityFlipped(bool flipped)
//    {
//        _isGravityFlipped = flipped;
//    }

//    public void ResetCamera()
//    {
//        _is3DMode = false;
//        _isGravityFlipped = false;

//        if (_target != null)
//        {
//            transform.position = _target.position + _offset2D;
//            transform.rotation = Quaternion.Euler(_rotation2D);
//            _currentOffset = _offset2D;
//        }
//    }
//}
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Ціль")]
    [SerializeField] private Transform _target;

    [Header("Налаштування 2D (Side View)")]
    [SerializeField] private Vector3 _offset2D = new Vector3(0, 1, -15);
    [SerializeField] private Vector3 _rotation2D = new Vector3(0, 0, 0);
    [SerializeField] private float _fov2D = 30f;
    [Tooltip("Початок туману в 2D (має бути далі, щоб бачити гори)")]
    [SerializeField] private float _fogStart2D = 40f;
    [SerializeField] private float _fogEnd2D = 120f;

    [Header("Налаштування 3D (Runner View)")]
    [SerializeField] private Vector3 _offset3D = new Vector3(0, 4, -8);
    [SerializeField] private Vector3 _rotation3D = new Vector3(20, 0, 0);
    [SerializeField] private float _fov3D = 60f;
    [Tooltip("Початок туману в 3D (має бути ближче, щоб ховати появу дороги)")]
    [SerializeField] private float _fogStart3D = 20f;
    [SerializeField] private float _fogEnd3D = 45f;

    [Header("Налаштування Плавності")]
    [SerializeField] private float _smoothTime = 0.1f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _fovSpeed = 5f;

    private Vector3 _currentVelocity;
    private Vector3 _currentOffset;
    private bool _is3DMode = false;
    private Camera _cam;
    private bool _isGravityFlipped = false;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _cam.orthographic = false;
        _currentOffset = _offset2D;

       
        RenderSettings.fog = true;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

       
        Vector3 baseOffset = _is3DMode ? _offset3D : _offset2D;
        Vector3 finalTargetOffset = baseOffset;

        if (_isGravityFlipped)
        {
            finalTargetOffset.y = -baseOffset.y;
        }

        _currentOffset = Vector3.Lerp(_currentOffset, finalTargetOffset, _fovSpeed * Time.deltaTime);
        Vector3 targetPosition = _target.position + _currentOffset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, _smoothTime);

        
        Quaternion targetRot;

        if (_is3DMode)
        {
            Vector3 targetEuler = _rotation3D;

            if (_isGravityFlipped)
            {
                targetEuler.x = -_rotation3D.x;
                targetEuler.z = 180f;
            }
            else
            {
                targetEuler.z = 0f;
            }

            targetRot = Quaternion.Euler(targetEuler);
        }
        else
        {
            Vector3 targetEuler = _rotation2D;
            targetEuler.z = _isGravityFlipped ? 180f : 0f;
            targetRot = Quaternion.Euler(targetEuler);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);

       
        float targetFOV = _is3DMode ? _fov3D : _fov2D;
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFOV, _fovSpeed * Time.deltaTime);

      
        float targetFogStart = _is3DMode ? _fogStart3D : _fogStart2D;
        float targetFogEnd = _is3DMode ? _fogEnd3D : _fogEnd2D;

        RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, targetFogStart, _fovSpeed * Time.deltaTime);
        RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, targetFogEnd, _fovSpeed * Time.deltaTime);
    }

    public void Set3DView(bool enable3D)
    {
        _is3DMode = enable3D;
    }

    public void SetGravityFlipped(bool flipped)
    {
        _isGravityFlipped = flipped;
    }

    public void ResetCamera()
    {
        _is3DMode = false;
        _isGravityFlipped = false;

        if (_target != null)
        {
            transform.position = _target.position + _offset2D;
            transform.rotation = Quaternion.Euler(_rotation2D);
            _currentOffset = _offset2D;
        }
    }
}