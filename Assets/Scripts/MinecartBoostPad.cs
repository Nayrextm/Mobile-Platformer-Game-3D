using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MinecartBoostPad : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.attachedRigidbody != null && other.attachedRigidbody.TryGetComponent(out MinecartRider cart))
        {
            cart.TriggerDownhillBoost();
        }
    }
}