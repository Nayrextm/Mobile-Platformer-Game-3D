using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkinSelectorMenu : MonoBehaviour
{
    public enum SelectorMode { Skin, PlayerColor, TrailColor }

    [Header("Що обираємо в цьому меню?")]
    [SerializeField] private SelectorMode _mode;

    [Header("Бази Даних")]
    [SerializeField] private SkinDatabase _skinDatabase;
    [SerializeField] private ColorDatabase _colorDatabase;

    [Header("UI Елементи")]
    [SerializeField] private TMP_Text _skinNameText;     
    [SerializeField] private Image _colorPreviewImage;    
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private Button _selectButton;
    [SerializeField] private TMP_Text _selectButtonText;

    private int _currentIndex = 0;
    private int _currentSavedID = 0; 

    private void Start()
    {
        if (_leftButton != null) _leftButton.onClick.AddListener(PreviousItem);
        if (_rightButton != null) _rightButton.onClick.AddListener(NextItem);
        if (_selectButton != null) _selectButton.onClick.AddListener(SelectItem);

        if (DatabaseManager.Instance != null)
        {
            switch (_mode)
            {
                case SelectorMode.Skin:
                    _currentSavedID = DatabaseManager.Instance.GetSelectedSkinID();
                    _currentIndex = _skinDatabase.allSkins.FindIndex(s => s.skinID == _currentSavedID);
                    break;
                case SelectorMode.PlayerColor:
                    _currentSavedID = DatabaseManager.Instance.GetSelectedColorIndex();
                    _currentIndex = _currentSavedID;
                    break;
                case SelectorMode.TrailColor:
                    _currentSavedID = DatabaseManager.Instance.GetSelectedTrailColorIndex();
                    _currentIndex = _currentSavedID;
                    break;
            }

            if (_currentIndex == -1) _currentIndex = 0;
        }

        UpdateUI();
    }

    private void NextItem()
    {
        _currentIndex++;
        if (_currentIndex >= GetMaxCount()) _currentIndex = 0;
        UpdateUI();
    }

    private void PreviousItem()
    {
        _currentIndex--;
        if (_currentIndex < 0) _currentIndex = GetMaxCount() - 1;
        UpdateUI();
    }

    private int GetMaxCount()
    {
        if (_mode == SelectorMode.Skin) return _skinDatabase != null ? _skinDatabase.allSkins.Count : 0;
        return _colorDatabase != null ? _colorDatabase.Count : 0;
    }

    private void UpdateUI()
    {
        if (GetMaxCount() == 0) return;

        int checkingID = 0; 
        
        if (_mode == SelectorMode.Skin)
        {
            SkinData currentSkin = _skinDatabase.allSkins[_currentIndex];
            checkingID = currentSkin.skinID;

            if (_skinNameText != null) { _skinNameText.gameObject.SetActive(true); _skinNameText.text = currentSkin.skinDisplayName; }
            if (_colorPreviewImage != null) _colorPreviewImage.gameObject.SetActive(false);
        }
        else
        {
            checkingID = _currentIndex; 
            Color currentColor = _colorDatabase.GetColorByIndex(_currentIndex);

            if (_skinNameText != null) _skinNameText.gameObject.SetActive(false);
            if (_colorPreviewImage != null) { _colorPreviewImage.gameObject.SetActive(true); _colorPreviewImage.color = currentColor; }
        }
       
        if (_selectButtonText != null)
        {
            if (checkingID == _currentSavedID)
            {
                _selectButtonText.text = "EQUIPPED";
                _selectButton.interactable = false;
            }
            else
            {
                _selectButtonText.text = "SELECT";
                _selectButton.interactable = true;
            }
        }
    }

    private void SelectItem()
    {
        if (GetMaxCount() == 0 || DatabaseManager.Instance == null) return;

        if (_mode == SelectorMode.Skin)
        {
            SkinData skinToSelect = _skinDatabase.allSkins[_currentIndex];
            DatabaseManager.Instance.SaveSelectedSkinID(skinToSelect.skinID);
            _currentSavedID = skinToSelect.skinID;
        }
        else if (_mode == SelectorMode.PlayerColor)
        {
            DatabaseManager.Instance.SaveSelectedColorIndex(_currentIndex);
            _currentSavedID = _currentIndex;
        }
        else if (_mode == SelectorMode.TrailColor)
        {
            DatabaseManager.Instance.SaveSelectedTrailColorIndex(_currentIndex);
            _currentSavedID = _currentIndex;
        }

        UpdateUI();
    }
}