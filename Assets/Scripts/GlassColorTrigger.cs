//using UnityEngine;

//public class GlassColorTrigger : MonoBehaviour
//{
//    [SerializeField] private Renderer _glassRenderer;

//    [Header("Зміна текстури")]
//    [Tooltip("Чи потрібно змінити текстуру скла при торканні?")]
//    [SerializeField] private bool _changeTexture = false;
//    [SerializeField] private Texture2D _newTexture;

//    [Header("Зміна основного кольору")]
//    [Tooltip("Чи потрібно змінювати базовий колір скла?")]
//    [SerializeField] private bool _changeBaseColor = true;
//    [SerializeField] private Color _newTintColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);

//    [Header("Зміна окантовки (Highlight/Rim)")]
//    [Tooltip("Чи потрібно змінювати колір глянцю на краях?")]
//    [SerializeField] private bool _changeRimColor = true;
//    [SerializeField] private Color _newRimColor = new Color(1f, 1f, 1f, 0.3f);

//    private MaterialPropertyBlock _propBlock;

//    private static readonly int ColorID = Shader.PropertyToID("_Color");
//    private static readonly int RimColorID = Shader.PropertyToID("_RimColor");
//    private static readonly int MainTexID = Shader.PropertyToID("_MainTex");

//    private void Awake()
//    {
//        _propBlock = new MaterialPropertyBlock();
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player") && _glassRenderer != null)
//        {
//            _glassRenderer.GetPropertyBlock(_propBlock);

//            if (_changeTexture && _newTexture != null)
//            {
//                _propBlock.SetTexture(MainTexID, _newTexture);
//            }

//            if (_changeBaseColor)
//            {
//                _propBlock.SetColor(ColorID, _newTintColor);
//            }

//            if (_changeRimColor)
//            {
//                _propBlock.SetColor(RimColorID, _newRimColor);
//            }

//            _glassRenderer.SetPropertyBlock(_propBlock);
//        }
//    }
//}
using UnityEngine;

public class GlassColorTrigger : MonoBehaviour
{
    [SerializeField] private Renderer _glassRenderer;

    [Header("Зміна текстури (Атлас)")]
    [Tooltip("Чи потрібно змінити візерунок скла при торканні?")]
    [SerializeField] private bool _changeTexture = false;

    // ТЕПЕР ТУТ СПРАЙТ! Перетягуйте сюди нарізані шматочки 32x32 з вашого атласу
    [SerializeField] private Sprite _newGlassPattern;

    [Header("Зміна основного кольору")]
    [SerializeField] private bool _changeBaseColor = true;
    [SerializeField] private Color _newTintColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);

    [Header("Зміна окантовки (Highlight/Rim)")]
    [SerializeField] private bool _changeRimColor = true;
    [SerializeField] private Color _newRimColor = new Color(1f, 1f, 1f, 0.3f);

    private MaterialPropertyBlock _propBlock;

    // Кешуємо всі ID для блискавичної роботи
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

            // --- БЛОК АТЛАСУ ТА СПРАЙТІВ ---
            if (_changeTexture && _newGlassPattern != null)
            {
                // 1. Передаємо в шейдер весь файл (картинку 96x96)
                _propBlock.SetTexture(MainTexID, _newGlassPattern.texture);

                // 2. Вираховуємо точні координати вибраного візерунка
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

                // 3. Відправляємо ці 4 цифри у відеокарту
                _propBlock.SetVector(MainTexST_ID, tilingAndOffset);

                // 4. Примусово вмикаємо галочку текстури в шейдері (1f = true)
                _propBlock.SetFloat(UseTexID, 1f);
            }

            // --- БЛОК КОЛЬОРІВ ---
            if (_changeBaseColor) _propBlock.SetColor(ColorID, _newTintColor);
            if (_changeRimColor) _propBlock.SetColor(RimColorID, _newRimColor);

            // Застосовуємо всі зміни за один крок
            _glassRenderer.SetPropertyBlock(_propBlock);
        }
    }
}