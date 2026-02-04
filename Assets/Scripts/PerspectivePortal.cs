using UnityEngine;

public class PerspectivePortal : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Галочка є = вмикаємо 3D. Галочки нема = повертаємо 2D")]
    public bool enable3DView = true;

    private void OnTriggerEnter(Collider other)
    {
        // Перевіряємо, чи це гравець
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>())
        {
            // Знаходимо камеру і перемикаємо режим
            CameraFollow cam = FindObjectOfType<CameraFollow>();
            if (cam != null)
            {
                cam.Set3DView(enable3DView);
            }
        }
    }
}