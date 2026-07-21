//using UnityEngine;

//public class ModeSwitchPortal : MonoBehaviour
//{
//    [Header("Налаштування")]
//    [Tooltip("Постав галочку, якщо це вхід у 3D тунель. Прибери, якщо це вихід у 2D.")]
//    [SerializeField] private bool _enterLaneMode = true;

//    [Header("Ефекти")]
//    [Tooltip("Наскільки далеко вперед від порталу з'явиться хвиля. Збільшіть, щоб відсунути її на гравця.")]
//    [SerializeField] private float _rippleOffset = 1.0f;

//    private CameraFollow _cam;

//    private void Awake()
//    {
//        _cam = FindObjectOfType<CameraFollow>();
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            Rigidbody rb = other.GetComponent<Rigidbody>();
//            PlayerController standardMove = other.GetComponent<PlayerController>();
//            LaneRunner3D laneMove = other.GetComponent<LaneRunner3D>();

//            // Базова логіка перемикання
//            if (_enterLaneMode)
//            {
//                if (standardMove) standardMove.enabled = false;
//                if (laneMove)
//                {
//                    laneMove.enabled = true;
//                    if (standardMove) laneMove.ForwardSpeed = standardMove.ForwardSpeed;
//                }
//                if (_cam) _cam.Set3DView(true);
//                if (rb != null && rb.velocity.y > 0)
//                {
//                    Vector3 fixedVel = rb.velocity;
//                    fixedVel.y = 0f;
//                    rb.velocity = fixedVel;
//                }
//                other.transform.rotation = Quaternion.identity;
//            }
//            else
//            {
//                // --- ЧИСТИЙ ТА ОПТИМІЗОВАНИЙ ВИХІД З РЕЖИМУ ---
//                if (laneMove && laneMove.enabled)
//                {
//                    // Запускаємо наш розумний метод вирівнювання на трасі
//                    laneMove.DisableAndSnapToCenter();
//                }
//                else if (laneMove)
//                {
//                    laneMove.enabled = false;
//                }

//                if (rb)
//                {
//                    rb.interpolation = RigidbodyInterpolation.None;
//                    rb.velocity = Vector3.zero;
//                    rb.angularVelocity = Vector3.zero;
//                    rb.isKinematic = false;

//                    // ВИДАЛЕНО жорстку прив'язку flatPos.z = 0f, яка ламала координати!
//                    rb.rotation = Quaternion.identity;

//                    Physics.SyncTransforms();
//                    rb.interpolation = RigidbodyInterpolation.Interpolate;
//                }

//                if (standardMove)
//                {
//                    standardMove.enabled = true;
//                    standardMove.SetMode("Cube");
//                }
//                if (_cam) _cam.Set3DView(false);
//            }

//            // --- АВТОМАТИЗОВАНІ ВІЗУАЛЬНІ ЕФЕКТИ ---
//            if (PoolManager.Instance != null)
//            {
//                Vector3 rippleSpawnPos = transform.position + transform.forward * _rippleOffset;
//                Quaternion fixedRotation = transform.rotation * Quaternion.Euler(0, 90, -90);

//                GameObject rippleObj = PoolManager.Instance.SpawnFromPool("PortalRipple", rippleSpawnPos, fixedRotation);

//                if (rippleObj != null)
//                {
//                    ParticleSystem ps = rippleObj.GetComponent<ParticleSystem>();
//                    if (ps != null)
//                    {
//                        var main = ps.main;
//                        float maxPortalSize = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
//                        main.startSize = maxPortalSize * 1.5f;
//                    }
//                }
//            }

//            PlayerPortalFX fx = other.attachedRigidbody ? other.attachedRigidbody.GetComponent<PlayerPortalFX>() : other.GetComponent<PlayerPortalFX>();
//            if (fx != null) fx.PlayPortalTransition();
//        }
//    }
//}
using UnityEngine;

public class ModeSwitchPortal : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Постав галочку, якщо це вхід у 3D тунель. Прибери, якщо це вихід у 2D.")]
    [SerializeField] private bool _enterLaneMode = true;

    [Header("Ефекти")]
    [Tooltip("Наскільки далеко вперед від порталу з'явиться хвиля. Збільшіть, щоб відсунути її на гравця.")]
    [SerializeField] private float _rippleOffset = 1.0f;

    private CameraFollow _cam;

    private void Awake()
    {
        _cam = FindObjectOfType<CameraFollow>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Використовуємо TryGetComponent для оптимізації
            if (other.attachedRigidbody != null)
            {
                Rigidbody rb = other.attachedRigidbody;

                // Намагаємося отримати посилання на контролери
                other.attachedRigidbody.TryGetComponent(out PlayerController standardMove);
                other.attachedRigidbody.TryGetComponent(out LaneRunner3D laneMove);

                // Базова логіка перемикання
                if (_enterLaneMode)
                {
                    if (standardMove) standardMove.enabled = false;
                    if (laneMove)
                    {
                        laneMove.enabled = true;
                        if (standardMove) laneMove.ForwardSpeed = standardMove.ForwardSpeed;
                    }
                    if (_cam) _cam.Set3DView(true);

                    if (rb.velocity.y > 0)
                    {
                        Vector3 fixedVel = rb.velocity;
                        fixedVel.y = 0f;
                        rb.velocity = fixedVel;
                    }
                    other.transform.rotation = Quaternion.identity;
                }
                else
                {
                    // --- ЧИСТИЙ ТА ОПТИМІЗОВАНИЙ ВИХІД З РЕЖИМУ ---
                    if (laneMove && laneMove.enabled)
                    {
                        laneMove.DisableAndSnapToCenter();
                    }
                    else if (laneMove)
                    {
                        laneMove.enabled = false;
                    }

                    rb.interpolation = RigidbodyInterpolation.None;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = false;
                    rb.rotation = Quaternion.identity;

                    Physics.SyncTransforms();
                    rb.interpolation = RigidbodyInterpolation.Interpolate;

                    if (standardMove)
                    {
                        standardMove.enabled = true;
                        // ФІКС: Передаємо enum замість рядка "Cube"
                        standardMove.SetMode(PlayerMode.Cube);
                    }
                    if (_cam) _cam.Set3DView(false);
                }

                // --- АВТОМАТИЗОВАНІ ВІЗУАЛЬНІ ЕФЕКТИ ---
                if (PoolManager.Instance != null)
                {
                    Vector3 rippleSpawnPos = transform.position + transform.forward * _rippleOffset;
                    Quaternion fixedRotation = transform.rotation * Quaternion.Euler(0, 90, -90);

                    GameObject rippleObj = PoolManager.Instance.SpawnFromPool("PortalRipple", rippleSpawnPos, fixedRotation);

                    if (rippleObj != null && rippleObj.TryGetComponent(out ParticleSystem ps))
                    {
                        var main = ps.main;
                        float maxPortalSize = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
                        main.startSize = maxPortalSize * 1.5f;
                    }
                }

                // Використовуємо TryGetComponent
                if (other.attachedRigidbody.TryGetComponent(out PlayerPortalFX fx))
                {
                    fx.PlayPortalTransition();
                }
            }
        }
    }
}