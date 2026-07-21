//using UnityEngine;

//public class ModePortal : MonoBehaviour
//{
//    public enum GameMode
//    {
//        Cube,
//        Ball,
//        Spider,
//        Ghost
//    }

//    [Header("Налаштування")]
//    [SerializeField] private GameMode _targetMode;

//    [Header("Ефекти")]
//    [Tooltip("Наскільки далеко вперед від порталу з'явиться хвиля. Збільшіть, щоб відсунути її на гравця.")]
//    [SerializeField] private float _rippleOffset = 1.0f;

//    private string _modeString;

//    private void Awake()
//    {
//        _modeString = _targetMode.ToString();
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            if (other.attachedRigidbody != null)
//            {
//                PlayerController player = other.attachedRigidbody.GetComponent<PlayerController>();
//                if (player != null) player.SetMode(_modeString);

//                // --- АВТОМАТИЗОВАНІ ВІЗУАЛЬНІ ЕФЕКТИ ---
//                if (PoolManager.Instance != null)
//                {
//                    Vector3 rippleSpawnPos = transform.position + transform.forward * _rippleOffset;
//                    Quaternion fixedRotation = transform.rotation * Quaternion.Euler(0, 90, -90);

//                    // Спавнимо об'єкт ефекту з пулу
//                    GameObject rippleObj = PoolManager.Instance.SpawnFromPool("PortalRipple", rippleSpawnPos, fixedRotation);

//                    // Автоматично масштабуємо ефект під розмір мешу порталу
//                    if (rippleObj != null)
//                    {
//                        ParticleSystem ps = rippleObj.GetComponent<ParticleSystem>();
//                        if (ps != null)
//                        {
//                            var main = ps.main;
//                            // Знаходимо найбільшу сторону порталу (ширину або висоту)
//                            float maxPortalSize = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
//                            // Множимо на 1.5, щоб кругла хвиля гарантовано перекривала кути квадратного порталу
//                            main.startSize = maxPortalSize * 1.5f;
//                        }
//                    }
//                }

//                PlayerPortalFX fx = other.attachedRigidbody.GetComponent<PlayerPortalFX>();
//                if (fx != null) fx.PlayPortalTransition();
//            }
//        }
//    }
//}
using UnityEngine;

public class ModePortal : MonoBehaviour
{
    [Header("Налаштування")]
    // ЗМІНЕНО: Тепер використовуємо єдиний глобальний PlayerMode
    [SerializeField] private PlayerMode _targetMode;

    [Header("Ефекти")]
    [Tooltip("Наскільки далеко вперед від порталу з'явиться хвиля. Збільшіть, щоб відсунути її на гравця.")]
    [SerializeField] private float _rippleOffset = 1.0f;

    // ВИДАЛЕНО: метод Awake та _modeString більше не потрібні

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.attachedRigidbody != null)
        {
            // ОПТИМІЗАЦІЯ: Блискавичний пошук скрипта гравця
            if (other.attachedRigidbody.TryGetComponent(out PlayerController player))
            {
                // Передаємо режим напряму, без перетворення в текст!
                player.SetMode(_targetMode);
            }

            // --- АВТОМАТИЗОВАНІ ВІЗУАЛЬНІ ЕФЕКТИ ---
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

            // ОПТИМІЗАЦІЯ: Блискавичний пошук скрипта ефектів
            if (other.attachedRigidbody.TryGetComponent(out PlayerPortalFX fx))
            {
                fx.PlayPortalTransition();
            }
        }
    }
}