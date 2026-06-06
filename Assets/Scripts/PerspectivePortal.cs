using UnityEngine;

public class PerspectivePortal : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Галочка є = вмикаємо 3D. Галочки нема = повертаємо 2D")]
    [SerializeField] private bool _enable3DView = true;

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
            if (_cam != null) _cam.Set3DView(_enable3DView);

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

            // Деформація гравця
            PlayerPortalFX fx = other.attachedRigidbody ? other.attachedRigidbody.GetComponent<PlayerPortalFX>() : other.GetComponent<PlayerPortalFX>();
            if (fx != null) fx.PlayPortalTransition();
        }
    }
}