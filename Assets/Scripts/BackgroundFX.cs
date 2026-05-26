using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BackgroundFX : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _mainModule;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _mainModule = _particleSystem.main;
    }

    public void SetBackgroundColor(Color targetColor)
    {
        targetColor.a = 0.6f;

        _mainModule.startColor = targetColor;
    }

    [ContextMenu("Test Red Particles")]
    private void TestRedColor()
    {
        SetBackgroundColor(Color.red);
    }
}