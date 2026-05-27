using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ParticleColorTrigger : MonoBehaviour
{
    [Header("Налаштування Порталу/Зони")]
    [Tooltip("У який колір перефарбувати фон?")]
    [SerializeField] private Color _targetColor = Color.cyan;

    [Tooltip("Як швидко змінити колір (в секундах)?")]
    [SerializeField] private float _transitionDuration = 1.0f;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (BackgroundFX.Instance != null)
            {
                BackgroundFX.Instance.ChangeColorSmoothly(_targetColor, _transitionDuration);
            }
        }
    }
}