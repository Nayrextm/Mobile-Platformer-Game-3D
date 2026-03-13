
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GhostFade : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 2.0f; 
    [SerializeField] private bool _fadeOut = true;   

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

   
    private static readonly int _baseColorID = Shader.PropertyToID("_BaseColor");

    private float _startAlpha = 1f;
    private float _timer = 0f;
    private Color _currentColor;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();

        if (_renderer != null && _renderer.sharedMaterial != null)
        {
            
            if (_renderer.sharedMaterial.HasProperty(_baseColorID))
            {
                _currentColor = _renderer.sharedMaterial.GetColor(_baseColorID);
                _startAlpha = _currentColor.a;
            }
        }

        
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        if (_fadeOut && _renderer != null)
        {
            _timer += Time.deltaTime;
            float progress = _timer / _lifeTime;

            
            _currentColor.a = Mathf.Lerp(_startAlpha, 0f, progress);

           
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(_baseColorID, _currentColor);
            _renderer.SetPropertyBlock(_propBlock);
        }
    }
}