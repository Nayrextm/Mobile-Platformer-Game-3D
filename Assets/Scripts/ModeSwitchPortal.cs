
using UnityEngine;

public enum PortalType
{
    ExitTo2D,
    Enter3DLane,
    EnterMinecart
}

public class ModeSwitchPortal : MonoBehaviour
{
    [Header("Налаштування Порталу")]
    [Tooltip("УВАГА: Режим 'EnterMinecart' працюватиме ТІЛЬКИ якщо на гравцю висить скрипт 'MinecartRider'.")]
    [SerializeField] private PortalType _portalType = PortalType.Enter3DLane;

    [Header("Ефекти")]
    [SerializeField] private float _rippleOffset = 1.0f;

    private CameraFollow _cam;

    private void Awake()
    {
        _cam = FindObjectOfType<CameraFollow>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.attachedRigidbody != null)
        {
            Rigidbody rb = other.attachedRigidbody;

            other.attachedRigidbody.TryGetComponent(out PlayerController standardMove);
            other.attachedRigidbody.TryGetComponent(out LaneRunner3D laneMove);
            other.attachedRigidbody.TryGetComponent(out MinecartRider cartRider);

            switch (_portalType)
            {
                case PortalType.Enter3DLane:
                    if (standardMove) standardMove.enabled = false;
                    if (cartRider) cartRider.DismountCart();

                    if (laneMove)
                    {
                        laneMove.enabled = true;
                        if (standardMove) laneMove.ForwardSpeed = standardMove.ForwardSpeed;
                    }

                    Setup3DPhysics(rb, other.transform);
                    if (_cam) _cam.Set3DView(true);
                    break;

                case PortalType.EnterMinecart:
                    if (standardMove) standardMove.enabled = false;

                    if (cartRider)
                    {
                        float currentSpeed = standardMove ? standardMove.ForwardSpeed : 15f;
                        cartRider.MountCart(currentSpeed);
                    }

                    Setup3DPhysics(rb, other.transform);
                    if (_cam) _cam.Set3DView(true);
                    break;

                case PortalType.ExitTo2D:
                    if (cartRider) cartRider.DismountCart();
                    else if (laneMove && laneMove.enabled) laneMove.DisableAndSnapToCenter();
                    else if (laneMove) laneMove.enabled = false;

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
                        standardMove.SetMode(PlayerMode.Cube);
                    }
                    if (_cam) _cam.Set3DView(false);
                    break;
            }

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

            if (other.attachedRigidbody.TryGetComponent(out PlayerPortalFX fx))
            {
                fx.PlayPortalTransition();
            }
        }
    }

    private void Setup3DPhysics(Rigidbody rb, Transform playerTransform)
    {
        if (rb.velocity.y > 0)
        {
            Vector3 fixedVel = rb.velocity;
            fixedVel.y = 0f;
            rb.velocity = fixedVel;
        }
        playerTransform.rotation = Quaternion.identity;
    }
}