using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZiplineObject : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Множник швидкості (1 = стандартна, 1.5 = швидше)")]
    [SerializeField] private float _speedMultiplier = 1.0f;

    public float SpeedMultiplier => _speedMultiplier;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        if (!gameObject.CompareTag("Zipline"))
        {
            gameObject.tag = "Zipline";
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(Vector3.zero, Vector3.right * 3f);
        Gizmos.DrawSphere(Vector3.right * 3f, 0.2f);
    }
}