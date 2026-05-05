using UnityEngine;

public class MannequinAnimator : MonoBehaviour
{
    private bool _isAnimating = false;
    private Vector3 _startPosition;

    [Header("Налаштування польоту")]
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _radius = 1.5f;

    [Header("Посилання")]
    [SerializeField] private SkinApplier _mannequinApplier;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        if (_isAnimating)
        {
            float x = Mathf.Sin(Time.time * _speed) * _radius;
            float y = Mathf.Sin(Time.time * _speed * 2f) * (_radius / 2f);
            transform.position = _startPosition + new Vector3(x, y, 0);
        }
    }

    public void SetAnimationState(bool state)
    {
        _isAnimating = state;

        if (!state)
        {
            transform.position = _startPosition;

            if (_mannequinApplier != null)
            {
                _mannequinApplier.ClearTrailHistory();
            }
        }
    }
}