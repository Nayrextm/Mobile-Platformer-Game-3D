
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using DG.Tweening;
//#if UNITY_EDITOR
//using UnityEditor; 
//#endif

//public class StarCoin : MonoBehaviour
//{
//    [Header("Налаштування")]
//    [Tooltip("Унікальний ID монети на рівні (наприклад: 0, 1 або 2)")]
//    [SerializeField] private int coinID;

//    [Header("Візуалізація (DOTween)")]
//    [SerializeField] private float bobDuration = 1f;     
//    [SerializeField] private float bobHeight = 0.5f;     
//    [SerializeField] private float rotateDuration = 2f;  

//    [Header("Компоненти")]
//    [SerializeField] private GameObject visualModel; 
//    [SerializeField] private Collider myCollider;

//    [Header("Матеріали")]
//    [SerializeField] private Material ghostMaterial;

//    [Header("Ефекти")]
//    [SerializeField] private ParticleSystem pickupEffect;
//    [SerializeField] private AudioClip pickupSound;


//    private Vector3 startPos;
//    private Material normalMaterial;
//    private Renderer myRenderer;
//    private string currentSceneName;
//    private bool isPermanentlyCollected = false;
//    private bool isCollectedInRun = false;

//    public int CoinID => coinID;

//    private void Awake()
//    {

//        currentSceneName = SceneManager.GetActiveScene().name;


//        if (visualModel != null)
//        {
//            myRenderer = visualModel.GetComponent<Renderer>();
//            if (myRenderer != null)
//            {
//                normalMaterial = myRenderer.sharedMaterial;
//            }
//        }
//    }

//    private void Start()
//    {
//        startPos = transform.position;


//        if (LevelManager.Instance != null)
//        {
//            LevelManager.Instance.OnLevelReset += ResetCoin;
//        }

//        ResetCoin(); 


//        transform.DORotate(new Vector3(0, 360, 0), rotateDuration, RotateMode.FastBeyond360)
//                 .SetLoops(-1, LoopType.Incremental)
//                 .SetRelative()
//                 .SetEase(Ease.Linear)
//                 .SetLink(gameObject);


//        transform.DOMoveY(startPos.y + bobHeight, bobDuration)
//                 .SetLoops(-1, LoopType.Yoyo)
//                 .SetEase(Ease.InOutSine)
//                 .SetLink(gameObject);
//    }

//    private void OnDestroy()
//    {

//        if (LevelManager.Instance != null)
//        {
//            LevelManager.Instance.OnLevelReset -= ResetCoin;
//        }
//    }

//    private void ResetCoin()
//    {
//        isCollectedInRun = false;
//        CheckDatabase();

//        if (isPermanentlyCollected)
//        {
//            MakeGhost();
//        }
//        else
//        {
//            MakeNormal();
//        }
//    }

//    private void CheckDatabase()
//    {
//        if (DatabaseManager.Instance != null)
//        {

//            isPermanentlyCollected = DatabaseManager.Instance.IsStarCoinCollected(currentSceneName, coinID);
//        }
//    }

//    private void MakeGhost()
//    {
//        if (visualModel) visualModel.SetActive(true);
//        if (myCollider) myCollider.enabled = false; 
//        if (myRenderer != null && ghostMaterial != null)
//        {
//            myRenderer.sharedMaterial = ghostMaterial;
//        }
//    }

//    private void MakeNormal()
//    {
//        if (visualModel) visualModel.SetActive(true);
//        if (myCollider) myCollider.enabled = true; 

//        if (myRenderer != null && normalMaterial != null)
//        {
//            myRenderer.sharedMaterial = normalMaterial;
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (isCollectedInRun || isPermanentlyCollected) return;

//        if (other.CompareTag("Player"))
//        {
//            Collect();
//        }
//    }

//    private void Collect()
//    {
//        isCollectedInRun = true;

//        if (LevelManager.Instance != null)
//        {
//            LevelManager.Instance.CollectCoinTemp(coinID);
//        }

//        if (pickupEffect) Instantiate(pickupEffect, transform.position, Quaternion.identity);

//        if (pickupSound)
//        {
//            float volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
//            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
//        }


//        if (visualModel) visualModel.SetActive(false);
//        if (myCollider) myCollider.enabled = false;
//    }


//#if UNITY_EDITOR

//    private void OnDrawGizmos()
//    {

//        GUIStyle style = new GUIStyle();
//        style.normal.textColor = Color.yellow; 
//        style.fontSize = 24;                   
//        style.fontStyle = FontStyle.Bold;
//        style.alignment = TextAnchor.MiddleCenter;

//        Vector3 labelPosition = transform.position + Vector3.up * 1.5f;

//        Handles.Label(labelPosition, $"ID: {coinID}", style);
//    }
//#endif
//}
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StarCoin : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Унікальний ID монети на рівні (наприклад: 0, 1 або 2)")]
    [SerializeField] private int _coinID;

    [Header("Візуалізація (DOTween)")]
    [SerializeField] private float _bobDuration = 1f;
    [SerializeField] private float _bobHeight = 0.5f;
    [SerializeField] private float _rotateDuration = 2f;

    [Header("Компоненти")]
    [SerializeField] private GameObject _visualModel;
    [SerializeField] private Collider _myCollider;

    [Header("Матеріали")]
    [SerializeField] private Material _ghostMaterial;

    [Header("Ефекти")]
    [SerializeField] private GameObject _pickupEffect;

    [SerializeField] private string _poolTag = "Spark";
    [SerializeField] private AudioClip _pickupSound;

    private Vector3 _startPos;
    private Material _normalMaterial;
    private Renderer _myRenderer;
    private string _currentSceneName;
    private bool _isPermanentlyCollected = false;
    private bool _isCollectedInRun = false;

    public int CoinID => _coinID;

    private void Awake()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;

        if (_visualModel != null)
        {
            _myRenderer = _visualModel.GetComponent<Renderer>();
            if (_myRenderer != null)
            {
                _normalMaterial = _myRenderer.sharedMaterial;
            }
        }
    }

    private void Start()
    {
        _startPos = transform.position;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelReset += ResetCoin;
        }

        ResetCoin();

        transform.DORotate(new Vector3(0, 360, 0), _rotateDuration, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Incremental)
                 .SetRelative()
                 .SetEase(Ease.Linear)
                 .SetLink(gameObject);

        transform.DOMoveY(_startPos.y + _bobHeight, _bobDuration)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine)
                 .SetLink(gameObject);
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelReset -= ResetCoin;
        }
    }

    private void ResetCoin()
    {
        _isCollectedInRun = false;
        CheckDatabase();

        if (_isPermanentlyCollected)
        {
            MakeGhost();
        }
        else
        {
            MakeNormal();
        }
    }

    private void CheckDatabase()
    {
        if (DatabaseManager.Instance != null)
        {
            _isPermanentlyCollected = DatabaseManager.Instance.IsStarCoinCollected(_currentSceneName, _coinID);
        }
    }

    private void MakeGhost()
    {
        if (_visualModel) _visualModel.SetActive(true);
        if (_myCollider) _myCollider.enabled = false;
        if (_myRenderer != null && _ghostMaterial != null)
        {
            _myRenderer.sharedMaterial = _ghostMaterial;
        }
    }

    private void MakeNormal()
    {
        if (_visualModel) _visualModel.SetActive(true);
        if (_myCollider) _myCollider.enabled = true;

        if (_myRenderer != null && _normalMaterial != null)
        {
            _myRenderer.sharedMaterial = _normalMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isCollectedInRun || _isPermanentlyCollected) return;

        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        _isCollectedInRun = true;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.CollectCoinTemp(_coinID);
        }

      
        if (_pickupEffect != null)
        {
            if (PoolManager.Instance != null)
            {
                PoolManager.Instance.SpawnFromPool(_poolTag, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(_pickupEffect, transform.position, Quaternion.identity);
            }
        }
        

        if (_pickupSound)
        {
            float volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            AudioSource.PlayClipAtPoint(_pickupSound, transform.position, volume);
        }

        if (_visualModel) _visualModel.SetActive(false);
        if (_myCollider) _myCollider.enabled = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.yellow;
        style.fontSize = 24;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;

        Vector3 labelPosition = transform.position + Vector3.up * 1.5f;

        Handles.Label(labelPosition, $"ID: {_coinID}", style);
    }
#endif
}