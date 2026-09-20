using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelStatsDisplay : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private string _levelID;

    [Header("UI Елементи")]
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _attemptsText;
    [SerializeField] private TMP_Text _statusText;

    [Header("Тексти Статусу")]
    [SerializeField] private string _textIfCompleted = "COMPLETED";
    [SerializeField] private string _textIfNotCompleted = "NOT COMPLETED";

    [Header("Кольори")]
    [SerializeField] private Color _colorCompleted = Color.green;
    [SerializeField] private Color _colorNotCompleted = Color.white;

    [Header("Іконки Алмазів")]
    [SerializeField] private Image _coin1Image;
    [SerializeField] private Image _coin2Image;
    [SerializeField] private Image _coin3Image;

    [Header("Спрайти")]
    [SerializeField] private Sprite _collectedSprite;
    [SerializeField] private Sprite _missingSprite;

    private void Start()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        if (DatabaseManager.Instance == null) return;

        var stats = DatabaseManager.Instance.GetLevelData(_levelID);

        if (stats != null)
        {
            double t = stats.TotalTime;

            if (_timeText)
            {
                TimeSpan time = TimeSpan.FromSeconds(t);
                _timeText.text = $"TIME: {time.Minutes:D2}:{time.Seconds:D2}";
            }
            if (_attemptsText) _attemptsText.text = $"ATTEMPTS: {stats.TotalAttempts}";

            SetCoinState(_coin1Image, stats.StarCoin1);
            SetCoinState(_coin2Image, stats.StarCoin2);
            SetCoinState(_coin3Image, stats.StarCoin3);

            if (_statusText != null)
            {
                if (stats.IsCompleted)
                {
                    _statusText.text = _textIfCompleted;
                    _statusText.color = _colorCompleted;
                }
                else
                {
                    _statusText.text = _textIfNotCompleted;
                    _statusText.color = _colorNotCompleted;
                }
            }
        }
        else
        {
            if (_timeText) _timeText.text = "TIME: 00:00";
            if (_attemptsText) _attemptsText.text = "ATTEMPTS: 0";

            SetCoinState(_coin1Image, false);
            SetCoinState(_coin2Image, false);
            SetCoinState(_coin3Image, false);

            if (_statusText != null)
            {
                _statusText.text = _textIfNotCompleted;
                _statusText.color = _colorNotCompleted;
            }
        }
    }

    private void SetCoinState(Image coinImg, bool collected)
    {
        if (coinImg != null)
        {
            coinImg.sprite = collected ? _collectedSprite : _missingSprite;
            coinImg.color = Color.white;
        }
    }

    public void SetLevelID(string newLevelID)
    {
        _levelID = newLevelID;
        UpdateStats(); 
    }
}