
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using DG.Tweening;

//[RequireComponent(typeof(Collider))]
//public class CoinPickup : MonoBehaviour
//{
//    [Header("Налаштування")]
//    [SerializeField] private int _coinValue = 1;

//    [Header("Візуалізація")]
//    [SerializeField] private float _rotateDuration = 2f;
//    [SerializeField] private float _bobDuration = 1f;
//    [SerializeField] private float _bobHeight = 0.5f;
//    [SerializeField] private GameObject _visualModel;

//    [Header("Ефекти")]
//    [SerializeField] private GameObject _pickupEffect;

//    [SerializeField] private string _poolTag = "Spark";
//    [SerializeField] private AudioClip _pickupSound;

//    private Vector3 _startPos;
//    private bool _isCollected = false;
//    private string _myID;
//    private Collider _collider;
//    private float _sfxVolume;

//    private void Awake()
//    {
//        _collider = GetComponent<Collider>();
//        _collider.isTrigger = true;

//        _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
//    }

//    private void Start()
//    {
//        _startPos = transform.position;

//        _myID = $"{SceneManager.GetActiveScene().name}_{_startPos.x:F1}_{_startPos.y:F1}_{_startPos.z:F1}";

//        if (DatabaseManager.Instance != null && DatabaseManager.Instance.IsCoinCollected(_myID))
//        {
//            gameObject.SetActive(false);
//            _isCollected = true;
//            return;
//        }

//        AnimateCoin();
//    }

//    private void AnimateCoin()
//    {
//        if (_visualModel != null)
//        {
//            _visualModel.transform.DORotate(new Vector3(0, 360, 0), _rotateDuration, RotateMode.FastBeyond360)
//                .SetLoops(-1, LoopType.Restart)
//                .SetRelative()
//                .SetEase(Ease.Linear)
//                .SetLink(_visualModel);

//            _visualModel.transform.DOMoveY(_startPos.y + _bobHeight, _bobDuration)
//                .SetLoops(-1, LoopType.Yoyo)
//                .SetEase(Ease.InOutSine)
//                .SetLink(_visualModel);
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!_isCollected && other.CompareTag("Player"))
//        {
//            Collect();
//        }
//    }

//    private void Collect()
//    {
//        _isCollected = true;

//        if (DatabaseManager.Instance != null)
//        {
//            DatabaseManager.Instance.AddCoins(_coinValue);
//            DatabaseManager.Instance.MarkCoinAsCollected(_myID);
//        }

//        if (CoinUI.Instance != null) CoinUI.Instance.UpdateDisplay();


//        if (_pickupEffect != null)
//        {
//            if (PoolManager.Instance != null)
//            {
//                PoolManager.Instance.SpawnFromPool(_poolTag, transform.position, Quaternion.identity);
//            }
//            else
//            {

//                Instantiate(_pickupEffect, transform.position, Quaternion.identity);
//            }
//        }


//        if (_pickupSound != null)
//        {
//            AudioSource.PlayClipAtPoint(_pickupSound, transform.position, _sfxVolume);
//        }

//        if (_visualModel != null) _visualModel.SetActive(false);
//        _collider.enabled = false;
//    }
//}
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CoinPickup : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private int _coinValue = 1;
    [Tooltip("Унікальний номер монетки. Кожна монетка на рівні повинна мати свою цифру (1, 2, 3...)")]
    [SerializeField] private int _uniqueID;

    [Header("Візуалізація (Без DOTween)")]
    [SerializeField] private float _rotateSpeed = 150f;
    [SerializeField] private float _bobSpeed = 2f;
    [SerializeField] private float _bobHeight = 0.5f;
    [SerializeField] private GameObject _visualModel;

    [Header("Ефекти")]
    [SerializeField] private string _poolTag = "Spark";
    [SerializeField] private AudioClip _pickupSound;

    private Vector3 _startPos;
    private Collider _collider;

   
    private bool _isCollected = false;

   
    private MeshRenderer[] _renderers;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;

       
        _renderers = GetComponentsInChildren<MeshRenderer>();
    }

    private void Start()
    {
        _startPos = transform.position;

        if (DatabaseManager.Instance != null && DatabaseManager.Instance.IsCoinAlreadyCollectedInDB(_uniqueID))
        {
            _isCollected = true;
            HideCoin(); 
            return;
        }
    }

    private void Update()
    {
      
        if (_isCollected) return;

        if (_visualModel != null)
        {
            _visualModel.transform.Rotate(0, 0, _rotateSpeed * Time.deltaTime);

            float newY = _startPos.y + (Mathf.Sin(Time.time * _bobSpeed) * _bobHeight);
            _visualModel.transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isCollected && other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        _isCollected = true;

        if (DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.CollectCoinImmediate(_uniqueID, _coinValue);
        }

        if (CoinUI.Instance != null) CoinUI.Instance.UpdateDisplay();

        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.SpawnFromPool(_poolTag, transform.position, Quaternion.identity);
        }

        if (_pickupSound != null)
        {
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
            AudioSource.PlayClipAtPoint(_pickupSound, transform.position, sfxVol);
        }

        HideCoin();
    }

    private void HideCoin()
    {
        foreach (var renderer in _renderers)
        {
            renderer.enabled = false;
        }

        if (_collider != null) _collider.enabled = false;

        this.enabled = false;
    }
}