using UnityEngine;

public class GlassColorTrigger : MonoBehaviour
{
    [SerializeField] private Renderer _glassRenderer;

    [Header("Зміна текстури (Атлас)")]
    [Tooltip("Чи потрібно змінити візерунок скла при торканні?")]
    [SerializeField] private bool _changeTexture = false;

    [SerializeField] private Sprite _newGlassPattern;

    [Header("Зміна основного кольору")]
    [SerializeField] private bool _changeBaseColor = true;
    [SerializeField] private Color _newTintColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);

    [Header("Зміна окантовки (Highlight/Rim)")]
    [SerializeField] private bool _changeRimColor = true;
    [SerializeField] private Color _newRimColor = new Color(1f, 1f, 1f, 0.3f);

    private MaterialPropertyBlock _propBlock;

    private static readonly int ColorID = Shader.PropertyToID("_Color");
    private static readonly int RimColorID = Shader.PropertyToID("_RimColor");
    private static readonly int MainTexID = Shader.PropertyToID("_MainTex");
    private static readonly int MainTexST_ID = Shader.PropertyToID("_MainTex_ST");
    private static readonly int UseTexID = Shader.PropertyToID("_UseTex");

    private void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _glassRenderer != null)
        {
            _glassRenderer.GetPropertyBlock(_propBlock);

            if (_changeTexture && _newGlassPattern != null)
            {
                _propBlock.SetTexture(MainTexID, _newGlassPattern.texture);

                Rect spriteRect = _newGlassPattern.textureRect;
                float atlasWidth = _newGlassPattern.texture.width;
                float atlasHeight = _newGlassPattern.texture.height;

                // Вектор 4: X (Tiling X), Y (Tiling Y), Z (Offset X), W (Offset Y)
                Vector4 tilingAndOffset = new Vector4(
                    spriteRect.width / atlasWidth,    // Буде 32/96 = 0.333
                    spriteRect.height / atlasHeight,  // Буде 32/96 = 0.333
                    spriteRect.x / atlasWidth,        // Зміщення по горизонталі
                    spriteRect.y / atlasHeight        // Зміщення по вертикалі
                );

                _propBlock.SetVector(MainTexST_ID, tilingAndOffset);

                _propBlock.SetFloat(UseTexID, 1f);
            }

            if (_changeBaseColor) _propBlock.SetColor(ColorID, _newTintColor);
            if (_changeRimColor) _propBlock.SetColor(RimColorID, _newRimColor);

            _glassRenderer.SetPropertyBlock(_propBlock);
        }
    }
}