using UnityEngine;
using UnityEngine.SceneManagement; 

[RequireComponent(typeof(Collider))]
public class CoinPickup : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private int _coinValue = 1;

    [Tooltip("Унікальний номер монетки на цьому рівні. Натисніть на три крапки компонента -> Автоматично призначити ID")]
    public int _uniqueID; 

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

   
    private string _globalID;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
        _renderers = GetComponentsInChildren<MeshRenderer>();

        _globalID = SceneManager.GetActiveScene().name + "_" + _uniqueID;
    }

    private void Start()
    {
        _startPos = transform.position;

        if (DatabaseManager.Instance != null && DatabaseManager.Instance.IsCoinAlreadyCollectedInDB(_globalID))
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
            DatabaseManager.Instance.CollectCoinImmediate(_globalID, _coinValue);
        }

        if (CoinUI.Instance != null) CoinUI.Instance.AddCoinAndDisplay();

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

#if UNITY_EDITOR
    [ContextMenu("Автоматично призначити ID всім монетам")]
    private void AutoAssignIDs()
    {
        CoinPickup[] allCoins = FindObjectsOfType<CoinPickup>();

        for (int i = 0; i < allCoins.Length; i++)
        {
            UnityEditor.Undo.RecordObject(allCoins[i], "Assign Coin IDs");

            allCoins[i]._uniqueID = i + 1;

            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(allCoins[i]);
        }

        Debug.Log($"Успішно пронумеровано {allCoins.Length} монет-префабів на сцені!");
    }
#endif
}