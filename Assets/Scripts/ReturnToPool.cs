//using UnityEngine;
//using System.Collections;


//[RequireComponent(typeof(ParticleSystem))]
//public class ReturnToPool : MonoBehaviour
//{
//    private ParticleSystem _particleSystem;

//    private void Awake()
//    {

//        _particleSystem = GetComponent<ParticleSystem>();
//    }


//    private void OnEnable()
//    {
//        StartCoroutine(DeactivateAfterDelay());
//    }

//    private IEnumerator DeactivateAfterDelay()
//    {

//        float waitTime = _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax;

//        yield return new WaitForSecondsRealtime(waitTime);


//        gameObject.SetActive(false);
//    }
//}
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ReturnToPool : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private float _lifeTime;
    private float _timer;

    private void Awake()
    {
       
        _particleSystem = GetComponent<ParticleSystem>();

        if (_particleSystem != null)
        {
            _lifeTime = _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax;
        }
    }

    private void OnEnable()
    {
      
        _timer = _lifeTime;

        
        if (_particleSystem != null)
        {
            _particleSystem.Play(true);
        }
    }

    private void OnDisable()
    {
       
        if (_particleSystem != null)
        {
            _particleSystem.Clear(true);
        }
    }

    private void Update()
    {
      
        _timer -= Time.unscaledDeltaTime;

        
        if (_timer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}