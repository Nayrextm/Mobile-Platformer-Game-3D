using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelOptimizer : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private Transform _interactablesParent; 

    [Header("Зона видимості")]
    [SerializeField] private float _viewDistanceForward = 50f;
    [SerializeField] private float _viewDistanceBackward = 15f;
    [SerializeField] private float _checkInterval = 0.5f;

    private readonly List<GameObject> _levelObjects = new List<GameObject>();

    private void Start()
    {
        if (_interactablesParent == null || _playerCamera == null)
        {
            Debug.LogError("Оптимізатор: Не призначено Камеру або Interactables Parent!");
            return;
        }

      
        foreach (Transform child in _interactablesParent)
        {
            _levelObjects.Add(child.gameObject);
            
            CheckDistanceAndToggle(child.gameObject, _playerCamera.position.x);
        }

        StartCoroutine(OptimizationRoutine());
    }

    private IEnumerator OptimizationRoutine()
    {
       
        WaitForSeconds wait = new WaitForSeconds(_checkInterval);

        while (true)
        {
            yield return wait;
            OptimizeLevel();
        }
    }

    private void OptimizeLevel()
    {
        float camX = _playerCamera.position.x;

       
        for (int i = 0; i < _levelObjects.Count; i++)
        {
            GameObject obj = _levelObjects[i];

           
            if (obj == null) continue;

            CheckDistanceAndToggle(obj, camX);
        }
    }

    private void CheckDistanceAndToggle(GameObject obj, float camX)
    {
        float distanceX = obj.transform.position.x - camX;

        bool shouldBeActive = distanceX > -_viewDistanceBackward && distanceX < _viewDistanceForward;

       
        if (obj.activeSelf != shouldBeActive)
        {
            obj.SetActive(shouldBeActive);
        }
    }
}