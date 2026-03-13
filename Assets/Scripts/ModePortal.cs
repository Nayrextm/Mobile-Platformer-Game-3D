using UnityEngine;

public class ModePortal : MonoBehaviour
{
    public enum GameMode
    {
        Cube,   
        Ball,   
        Spider, 
        Ghost   
    }

    [Header("Налаштування")]
    [SerializeField] private GameMode _targetMode;
    [SerializeField] private AudioClip _portalSound;

   
    private string _modeString;

    private void Awake()
    {
       
        _modeString = _targetMode.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
           
            if (other.attachedRigidbody != null)
            {
                PlayerController player = other.attachedRigidbody.GetComponent<PlayerController>();

                if (player != null)
                {
                    
                    player.SetMode(_modeString);

                   
                    if (_portalSound != null)
                    {
                        AudioSource.PlayClipAtPoint(_portalSound, transform.position);
                    }
                }
            }
        }
    }
}