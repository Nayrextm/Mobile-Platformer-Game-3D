
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResetDataButton : MonoBehaviour
{
    private Button myButton;

    void Start()
    {
        myButton = GetComponent<Button>();
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnResetClick);
        }
    }

    void OnResetClick()
    {
        if (DatabaseManager.Instance != null)
        {
            // 1. Видаляємо з бази
            DatabaseManager.Instance.DeleteAllData();

            // 2. Перезавантажуємо сцену
            // Це змусить скрипти StarCoin.cs запуститися заново.
            // Вони перевірять базу -> побачать, що запису немає -> покажуть алмази знову.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}