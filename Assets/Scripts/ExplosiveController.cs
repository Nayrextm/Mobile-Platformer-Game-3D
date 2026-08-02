using UnityEngine;
using DG.Tweening;

public class ExplosiveController : MonoBehaviour
{
    [Header("Об'єкти для знищення (Стіни, каміння)")]
    [SerializeField] private GameObject[] _targetObstacles;

    [Header("Власний колайдер")]
    [SerializeField] private Collider _dynamiteCollider;

    private ExplosiveVFX _vfxController;
    private bool _hasDetonated = false;

    private void Awake()
    {
        TryGetComponent(out _vfxController);
    }

    private void Start()
    {
        if (LevelManager.Instance != null) LevelManager.Instance.OnLevelReset += ResetExplosive;
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null) LevelManager.Instance.OnLevelReset -= ResetExplosive;
    }

    public void Detonate()
    {
        if (_hasDetonated) return;
        _hasDetonated = true;

        // МИТТЄВО вимикаємо фізику
        if (_dynamiteCollider != null) _dynamiteCollider.enabled = false;

        foreach (var obstacle in _targetObstacles)
        {
            if (obstacle != null)
            {
                Collider[] colliders = obstacle.GetComponentsInChildren<Collider>();
                foreach (var col in colliders) col.enabled = false;
            }
        }

        // Передаємо візуалу колбек (FinishDetonation) для виклику в кінці анімації
        if (_vfxController != null) _vfxController.PlayExplosionEffects(_targetObstacles, FinishDetonation);
        else FinishDetonation();
    }

    private void FinishDetonation()
    {
        foreach (var obstacle in _targetObstacles)
        {
            if (obstacle != null) obstacle.SetActive(false);
        }
    }

    private void ResetExplosive()
    {
        _hasDetonated = false;
        if (_dynamiteCollider != null) _dynamiteCollider.enabled = true;

        foreach (var obstacle in _targetObstacles)
        {
            if (obstacle != null)
            {
                obstacle.transform.DOKill(); // Зупиняємо анімацію зникнення
                obstacle.SetActive(true);
                obstacle.transform.localScale = Vector3.one;

                Collider[] colliders = obstacle.GetComponentsInChildren<Collider>();
                foreach (var col in colliders) col.enabled = true;
            }
        }

        if (_vfxController != null) _vfxController.ResetVisuals();
    }
}