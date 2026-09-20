using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float delay = 2.0f;

    void Start()
    {
        Destroy(gameObject, delay);
    }
}