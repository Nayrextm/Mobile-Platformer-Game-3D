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
            Rigidbody rb = other.GetComponent<Rigidbody>();
            PlayerController standardMove = other.GetComponent<PlayerController>();
            LaneRunner3D laneMove = other.GetComponent<LaneRunner3D>();

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
                if (rb != null && rb.velocity.y > 0)
                {
                    Vector3 fixedVel = rb.velocity;
                    fixedVel.y = 0f;
                    rb.velocity = fixedVel;
                }
                other.transform.rotation = Quaternion.identity;
            }
            else
            {
                if (laneMove) laneMove.enabled = false;
                if (rb)
                {
                    rb.interpolation = RigidbodyInterpolation.None;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = false;
                    Vector3 flatPos = rb.position;
                    flatPos.z = 0f;
                    rb.position = flatPos;
                    rb.rotation = Quaternion.identity;
                    Physics.SyncTransforms();
                    rb.interpolation = RigidbodyInterpolation.Interpolate;
                }
                if (standardMove)
                {
                    standardMove.enabled = true;
                    standardMove.SetMode("Cube");
                }
                if (_cam) _cam.Set3DView(false);
            }

            // --- АВТОМАТИЗОВАНІ ВІЗУАЛЬНІ ЕФЕКТИ ---
            if (PoolManager.Instance != null)
            {
                Vector3 rippleSpawnPos = transform.position + transform.forward * _rippleOffset;
                Quaternion fixedRotation = transform.rotation * Quaternion.Euler(0, 90, -90);

                // Спавнимо об'єкт ефекту з пулу
                GameObject rippleObj = PoolManager.Instance.SpawnFromPool("PortalRipple", rippleSpawnPos, fixedRotation);

                // Автоматично масштабуємо ефект під розмір мешу порталу
                if (rippleObj != null)
                {
                    ParticleSystem ps = rippleObj.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        var main = ps.main;
                        // Знаходимо найбільшу сторону порталу (ширину або висоту)
                        float maxPortalSize = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
                        // Множимо на 1.5, щоб кругла хвиля гарантовано перекривала кути квадратного порталу
                        main.startSize = maxPortalSize * 1.5f;
                    }
                }
            }

            PlayerPortalFX fx = other.attachedRigidbody ? other.attachedRigidbody.GetComponent<PlayerPortalFX>() : other.GetComponent<PlayerPortalFX>();
            if (fx != null) fx.PlayPortalTransition();
        }
    }
}