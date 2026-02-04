using UnityEngine;

public class GhostFade : MonoBehaviour
{
    public float lifeTime = 2.0f; // Скільки секунд живе привид
    public bool fadeOut = true;   // Чи треба плавно зникати

    private Material _matInstance;
    private float _startAlpha;
    private float _timer;

    void Start()
    {
        // Отримуємо матеріал
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            _matInstance = rend.material;
            // Запам'ятовуємо початкову прозорість
            if (_matInstance.HasProperty("_BaseColor"))
                _startAlpha = _matInstance.GetColor("_BaseColor").a;
            else
                _startAlpha = 1f; // Якщо шейдер стандартний
        }

        // Запуск таймера знищення (гарантія, що об'єкт видалиться)
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (fadeOut && _matInstance != null)
        {
            _timer += Time.deltaTime;
            // Рахуємо нову прозорість (від стартової до 0)
            float progress = _timer / lifeTime;
            float currentAlpha = Mathf.Lerp(_startAlpha, 0f, progress);

            // Застосовуємо колір
            Color c = _matInstance.GetColor("_BaseColor");
            c.a = currentAlpha;
            _matInstance.SetColor("_BaseColor", c);
        }
    }
}