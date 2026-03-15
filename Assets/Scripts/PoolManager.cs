//using System.Collections.Generic;
//using UnityEngine;


//[System.Serializable]
//public class Pool
//{
//    public string tag;           
//    public GameObject prefab;    
//    public int size;             
//}

//public class PoolManager : MonoBehaviour
//{

//    public static PoolManager Instance { get; private set; }

//    [Header("Налаштування пулів")]
//    [SerializeField] private List<Pool> _pools;


//    private Dictionary<string, Queue<GameObject>> _poolDictionary;

//    private void Awake()
//    {

//        if (Instance == null) Instance = this;
//        else Destroy(gameObject);
//    }

//    private void Start()
//    {
//        _poolDictionary = new Dictionary<string, Queue<GameObject>>();


//        foreach (Pool pool in _pools)
//        {
//            Queue<GameObject> objectPool = new Queue<GameObject>();


//            for (int i = 0; i < pool.size; i++)
//            {
//                GameObject obj = Instantiate(pool.prefab);
//                obj.SetActive(false); 
//                obj.transform.SetParent(transform); 
//                objectPool.Enqueue(obj);
//            }


//            _poolDictionary.Add(pool.tag, objectPool);
//        }
//    }



//    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
//    {

//        if (!_poolDictionary.ContainsKey(tag))
//        {
//            Debug.LogWarning("Pool з тегом " + tag + " не знайдено!");
//            return null;
//        }

//        GameObject objectToSpawn = _poolDictionary[tag].Dequeue();


//        objectToSpawn.SetActive(true);
//        objectToSpawn.transform.position = position;
//        objectToSpawn.transform.rotation = rotation;


//        _poolDictionary[tag].Enqueue(objectToSpawn);

//        return objectToSpawn;
//    }

//    public void HideAllActiveEffects()
//    {

//        foreach (Transform child in transform)
//        {

//            if (child.gameObject.activeSelf)
//            {
//                child.gameObject.SetActive(false);
//            }
//        }
//    }
//}
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public int size;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [Header("Налаштування пулів")]
    [SerializeField] private List<Pool> _pools;

    private Dictionary<string, Queue<GameObject>> _poolDictionary;

    private void Awake()
    {
      
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; 
        }

       
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in _pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                objectPool.Enqueue(obj);
            }

            _poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool з тегом " + tag + " не знайдено!");
            return null;
        }

       
        GameObject objectToSpawn = _poolDictionary[tag].Dequeue();

       
        objectToSpawn.SetActive(false);

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

       
        _poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

   
    public void HideAllActiveEffects()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}