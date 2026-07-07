using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DirectionTrigger : MonoBehaviour
{
    [Header("Новий напрямок руху")]
    [Tooltip("Приклад: X=1, Z=0 (вправо). X=0, Z=1 (вперед). X=-1, Z=0 (вліво)")]
    [SerializeField] private Vector3 _newMoveDirection = Vector3.forward;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Кажемо гравцеві змінити напрямок, а камера сама відреагує на це
            if (other.attachedRigidbody.TryGetComponent(out PlayerController player))
            {
                player.SetMoveDirection(_newMoveDirection);
            }
        }
    }
}