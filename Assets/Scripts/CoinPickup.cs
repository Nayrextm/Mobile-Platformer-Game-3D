
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using DG.Tweening;

//public class CoinPickup : MonoBehaviour
//{
//    [Header("Налаштування")]
//    [SerializeField] private int coinValue = 1;

//    [Header("Візуалізація (Левітація та Обертання)")]
//    [SerializeField] private float rotateDuration = 2f;
//    [SerializeField] private float bobDuration = 1f;
//    [SerializeField] private float bobHeight = 0.5f;

//    [Header("Ефекти")]
//    [SerializeField] private GameObject pickupEffect;
//    [SerializeField] private AudioClip pickupSound;

//    private Vector3 startPos;
//    private bool isCollected = false;
//    private string myID;

//    void Start()
//    {
//        startPos = transform.position;


//        string posString = $"{startPos.x:F2}_{startPos.y:F2}_{startPos.z:F2}";
//        myID = $"{SceneManager.GetActiveScene().name}_{posString}";


//        if (DatabaseManager.Instance != null && DatabaseManager.Instance.IsCoinCollected(myID))
//        {
//            gameObject.SetActive(false);
//            isCollected = true;
//            return;
//        }


//        transform.DORotate(new Vector3(0, 360, 0), rotateDuration, RotateMode.FastBeyond360)
//                 .SetLoops(-1, LoopType.Restart)
//                 .SetRelative()
//                 .SetEase(Ease.Linear)
//                 .SetLink(gameObject);


//        transform.DOMoveY(startPos.y + bobHeight, bobDuration)
//                 .SetLoops(-1, LoopType.Yoyo)
//                 .SetEase(Ease.InOutSine)
//                 .SetLink(gameObject);
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player") && !isCollected)
//        {
//            Collect();
//        }
//    }

//    void Collect()
//    {
//        isCollected = true;

//        if (DatabaseManager.Instance != null)
//        {
//            DatabaseManager.Instance.AddCoins(coinValue);
//            DatabaseManager.Instance.MarkCoinAsCollected(myID);
//        }

//        if (CoinUI.Instance != null) CoinUI.Instance.UpdateDisplay();

//        if (pickupEffect != null) Instantiate(pickupEffect, transform.position, Quaternion.identity);

//        if (pickupSound != null)
//        {
//            float volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
//        }


//        transform.DOKill();

//        gameObject.SetActive(false);
//    }
//}
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class CoinPickup : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private int _coinValue = 1;

    [Header("Візуалізація")]
    [SerializeField] private float _rotateDuration = 2f;
    [SerializeField] private float _bobDuration = 1f;
    [SerializeField] private float _bobHeight = 0.5f;
    [SerializeField] private GameObject _visualModel;

    [Header("Ефекти")]
    [SerializeField] private GameObject _pickupEffect;
    [SerializeField] private AudioClip _pickupSound;

    private Vector3 _startPos;
    private bool _isCollected = false;
    private string _myID;
    private Collider _collider;
    private float _sfxVolume;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;

        
        _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    private void Start()
    {
        _startPos = transform.position;

      
        _myID = $"{SceneManager.GetActiveScene().name}_{_startPos.x:F1}_{_startPos.y:F1}_{_startPos.z:F1}";

        if (DatabaseManager.Instance != null && DatabaseManager.Instance.IsCoinCollected(_myID))
        {
            gameObject.SetActive(false);
            _isCollected = true;
            return;
        }

        AnimateCoin();
    }

    private void AnimateCoin()
    {
       
        if (_visualModel != null)
        {
            _visualModel.transform.DORotate(new Vector3(0, 360, 0), _rotateDuration, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetRelative()
                .SetEase(Ease.Linear)
                .SetLink(_visualModel);

            _visualModel.transform.DOMoveY(_startPos.y + _bobHeight, _bobDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetLink(_visualModel);
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
            DatabaseManager.Instance.AddCoins(_coinValue);
            DatabaseManager.Instance.MarkCoinAsCollected(_myID);
        }

      
        if (CoinUI.Instance != null) CoinUI.Instance.UpdateDisplay();

        if (_pickupEffect != null)
        {
            Instantiate(_pickupEffect, transform.position, Quaternion.identity);
        }

        if (_pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(_pickupSound, transform.position, _sfxVolume);
        }

       
        if (_visualModel != null) _visualModel.SetActive(false);
        _collider.enabled = false;
    }
}