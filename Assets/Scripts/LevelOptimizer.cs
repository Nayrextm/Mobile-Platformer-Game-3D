using UnityEngine;
using System.Collections.Generic;

public class LevelOptimizer : MonoBehaviour
{
    public Transform playerCamera;
    public float viewDistance = 50f;
    public float checkInterval = 0.5f;

    private List<GameObject> allLevelObjects = new List<GameObject>();

    void Start()
    {
        GameObject env = GameObject.Find("Environment");
        if (env != null)
        {
            foreach (Transform child in env.transform)
            {
                // ВАЖЛИВО: Додаємо в список ТІЛЬКИ ті об'єкти, 
                // які активні в ієрархії на момент старту гри.
                if (child.gameObject.activeSelf)
                {
                    allLevelObjects.Add(child.gameObject);
                    child.gameObject.SetActive(false); // Тепер вимикаємо лише їх для подальшої появи
                }
            }
        }

        InvokeRepeating("OptimizeLevel", 0f, checkInterval);
    }

    void OptimizeLevel()
    {
        if (playerCamera == null) return;

        float camX = playerCamera.position.x;

        foreach (GameObject obj in allLevelObjects)
        {
            if (obj == null) continue;

            float distanceX = obj.transform.position.x - camX;

            // Об'єкт вмикається, якщо він попереду (до viewDistance) 
            // або за спиною не далі ніж на 15 одиниць
            if (distanceX > -15f && distanceX < viewDistance)
            {
                if (!obj.activeSelf) obj.SetActive(true);
            }
            else
            {
                if (obj.activeSelf) obj.SetActive(false);
            }
        }
    }
}