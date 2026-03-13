//using UnityEngine;

//public class LevelStats : MonoBehaviour
//{
//    public static LevelStats Instance;

//    public int attempts = 1;      // перша спроба при старті рівня
//    public float time = 0f;       // скільки секунд триває спроба
//    public bool levelFinished = false;

//    private void Awake()
//    {
//        // робимо Singleton (тільки один у сцені)
//        if (Instance == null)
//            Instance = this;
//        else
//            Destroy(gameObject);
//    }

//    private void Update()
//    {
//        // якщо рівень не завершено — рахуємо час
//        if (!levelFinished)
//        {
//            time += Time.deltaTime;
//        }
//    }

//    public void AddAttempt()
//    {
//        attempts++;
//        time = 0f;  // час нової спроби починається з нуля
//    }

//    public void Finish()
//    {
//        levelFinished = true;
//    }
//}
using UnityEngine;

public class LevelStats : MonoBehaviour
{
    public static LevelStats Instance { get; private set; }

    public int Attempts { get; private set; } = 1;
    public float TimeElapsed { get; private set; } = 0f;
    public bool LevelFinished { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!LevelFinished)
        {
            TimeElapsed += Time.deltaTime;
        }
    }

    public void AddAttempt()
    {
        Attempts++;
        TimeElapsed = 0f;
    }

    public void Finish()
    {
        LevelFinished = true;
    }
}
