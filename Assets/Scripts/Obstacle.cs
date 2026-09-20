using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public ParticleSystem hitParticles;
    public AudioClip hitSfx;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hitParticles) Instantiate(hitParticles, transform.position, Quaternion.identity);
            if (hitSfx)
            {
                AudioSource.PlayClipAtPoint(hitSfx, transform.position);
            }
        }
    }
}
