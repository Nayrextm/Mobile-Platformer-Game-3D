using UnityEngine;

public class ModePortal : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private PlayerMode _targetMode;

    [Header("Ефекти")]
    [Tooltip("Наскільки далеко вперед від порталу з'явиться хвиля. Збільшіть, щоб відсунути її на гравця.")]
    [SerializeField] private float _rippleOffset = 1.0f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.attachedRigidbody != null)
        {
            if (other.attachedRigidbody.TryGetComponent(out PlayerController player))
            {
                player.SetMode(_targetMode);
            }

            if (PoolManager.Instance != null)
            {
                Vector3 rippleSpawnPos = transform.position + transform.forward * _rippleOffset;
                Quaternion fixedRotation = transform.rotation * Quaternion.Euler(0, 90, -90);

                GameObject rippleObj = PoolManager.Instance.SpawnFromPool("PortalRipple", rippleSpawnPos, fixedRotation);

                if (rippleObj != null)
                {
                    if (rippleObj.TryGetComponent(out ParticleSystem ps))
                    {
                        var main = ps.main;
                        float maxPortalSize = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
                        main.startSize = maxPortalSize * 1.5f;
                    }
                }
            }

            if (other.attachedRigidbody.TryGetComponent(out PlayerPortalFX fx))
            {
                fx.PlayPortalTransition();
            }
        }
    }
}