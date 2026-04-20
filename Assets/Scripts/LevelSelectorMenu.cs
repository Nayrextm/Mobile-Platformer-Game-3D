using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelSelectorMenu : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public string displayName; 
        public string sceneName;   
        public string difficulty;  
    }

    [Header("Список рівнів")]
    [SerializeField] private LevelData[] _levels;

    [Header("UI Елементи")]
    [SerializeField] private TMP_Text _levelNameText; 
    [SerializeField] private TMP_Text _difficultyText; 
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private Button _playButton; 

    [Header("Скрипти")]
    [SerializeField] private LevelStatsDisplay _statsDisplay;
    [SerializeField] private MainMenuController _mainMenu;

    private int _currentIndex = 0;

    private void Start()
    {
        if (_leftButton != null) _leftButton.onClick.AddListener(PreviousLevel);
        if (_rightButton != null) _rightButton.onClick.AddListener(NextLevel);
        if (_playButton != null) _playButton.onClick.AddListener(PlaySelectedLevel);

        UpdateUI();
    }

    private void NextLevel()
    {
        _currentIndex++;
        if (_currentIndex >= _levels.Length) _currentIndex = 0;
        UpdateUI();
    }

    private void PreviousLevel()
    {
        _currentIndex--;
        if (_currentIndex < 0) _currentIndex = _levels.Length - 1;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_levels.Length == 0) return;

        LevelData currentLevel = _levels[_currentIndex];

        
        if (_levelNameText != null) _levelNameText.text = currentLevel.displayName;
        if (_difficultyText != null) _difficultyText.text = "DIFFICULTY: " + currentLevel.difficulty;

        if (_statsDisplay != null)
        {
            _statsDisplay.SetLevelID(currentLevel.sceneName);
        }
    }

    private void PlaySelectedLevel()
    {
        if (_mainMenu != null && _levels.Length > 0)
        {
            _mainMenu.LoadLevelScene(_levels[_currentIndex].sceneName);
        }
    }
}