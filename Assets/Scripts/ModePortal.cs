using UnityEngine;

public class ModePortal : MonoBehaviour
{
    public enum GameMode
    {
        Cube,
        Ball,
        Spider,
        Ghost
    }

    [Header("Налаштування")]
    [SerializeField] private GameMode _targetMode;

    [Header("Ефекти")]
    [Tooltip("Наскільки далеко вперед від порталу з'явиться хвиля. Збільшіть, щоб відсунути її на гравця.")]
    [SerializeField] private float _rippleOffset = 1.0f;

    private string _modeString;

    private void Awake()
    {
        _modeString = _targetMode.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.attachedRigidbody != null)
            {
                PlayerController player = other.attachedRigidbody.GetComponent<PlayerController>();
                if (player != null) player.SetMode(_modeString);

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

                PlayerPortalFX fx = other.attachedRigidbody.GetComponent<PlayerPortalFX>();
                if (fx != null) fx.PlayPortalTransition();
            }
        }
    }
}