using UnityEngine;

public class PerspectivePortal : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Галочка є = вмикаємо 3D. Галочки нема = повертаємо 2D")]
    [SerializeField] private bool _enable3DView = true;

    
    private CameraFollow _cam;

    private void Awake()
    {
        
        _cam = FindObjectOfType<CameraFollow>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            if (_cam != null)
            {
                _cam.Set3DView(_enable3DView);
            }
        }
    }
}