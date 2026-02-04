using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float delay = 2.0f; // Час життя ефекту в секундах

    void Start()
    {
        // Ця команда каже Unity: "Знищи цей об'єкт через 'delay' секунд"
        Destroy(gameObject, delay);
    }
}