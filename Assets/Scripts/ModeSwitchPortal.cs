using UnityEngine;

public class ModeSwitchPortal : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Постав галочку, якщо це вхід у 3D тунель. Прибери, якщо це вихід у 2D.")]
    [SerializeField] private bool _enterLaneMode = true;

    
    private CameraFollow _cam;

    private void Awake()
    {
       
        _cam = FindObjectOfType<CameraFollow>();
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            PlayerController standardMove = other.GetComponent<PlayerController>();
            LaneRunner3D laneMove = other.GetComponent<LaneRunner3D>();

            if (_enterLaneMode)
            {
                
                if (standardMove) standardMove.enabled = false;

                
                if (laneMove)
                {
                    laneMove.enabled = true;
                   
                    if (standardMove) laneMove.ForwardSpeed = standardMove.ForwardSpeed;
                }

              
                if (_cam) _cam.Set3DView(true);

               
                if (rb != null && rb.velocity.y > 0)
                {
                    Vector3 fixedVel = rb.velocity;
                    fixedVel.y = 0f;
                    rb.velocity = fixedVel;
                }

                
                other.transform.rotation = Quaternion.identity;
            }
            else
            {
                

               
                if (laneMove) laneMove.enabled = false;

               
                if (rb)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = false;
                }

              
                Vector3 flatPos = other.transform.position;
                flatPos.z = 0f;
                other.transform.position = flatPos;

               
                other.transform.rotation = Quaternion.identity;

              
                if (standardMove)
                {
                    standardMove.enabled = true;
                    standardMove.SetMode("Cube");
                }

               
                if (_cam) _cam.Set3DView(false);
            }
        }
    }
}