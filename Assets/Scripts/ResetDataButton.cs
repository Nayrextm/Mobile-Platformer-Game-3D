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
            DatabaseManager.Instance.DeleteAllData();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}