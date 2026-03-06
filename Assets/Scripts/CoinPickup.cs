
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CoinPickup : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private int coinValue = 1;

    [Header("Візуалізація (Левітація та Обертання)")]
    [SerializeField] private float rotateDuration = 2f;
    [SerializeField] private float bobDuration = 1f;
    [SerializeField] private float bobHeight = 0.5f;

    [Header("Ефекти")]
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private AudioClip pickupSound;

    private Vector3 startPos;
    private bool isCollected = false;
    private string myID;

    void Start()
    {
        startPos = transform.position;

       
        string posString = $"{startPos.x:F2}_{startPos.y:F2}_{startPos.z:F2}";
        myID = $"{SceneManager.GetActiveScene().name}_{posString}";

        
        if (DatabaseManager.Instance != null && DatabaseManager.Instance.IsCoinCollected(myID))
        {
            gameObject.SetActive(false);
            isCollected = true;
            return;
        }

       
        transform.DORotate(new Vector3(0, 360, 0), rotateDuration, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Restart)
                 .SetRelative()
                 .SetEase(Ease.Linear)
                 .SetLink(gameObject);

       
        transform.DOMoveY(startPos.y + bobHeight, bobDuration)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine)
                 .SetLink(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Collect();
        }
    }

    void Collect()
    {
        isCollected = true;

        if (DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.AddCoins(coinValue);
            DatabaseManager.Instance.MarkCoinAsCollected(myID);
        }

        if (CoinUI.Instance != null) CoinUI.Instance.UpdateDisplay();

        if (pickupEffect != null) Instantiate(pickupEffect, transform.position, Quaternion.identity);

        if (pickupSound != null)
        {
            float volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }

       
        transform.DOKill();

        gameObject.SetActive(false);
    }
}