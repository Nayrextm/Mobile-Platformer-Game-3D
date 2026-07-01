using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class UIDualColorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Складові кнопки")]
    [SerializeField] private Image frontImage;
    [SerializeField] private Image shadowImage;

    [Header("Кольори Основні (Front)")]
    [SerializeField] private Color frontNormal = Color.white;
    [SerializeField] private Color frontHover = new Color(0.9f, 0.9f, 0.9f, 1f);
    [SerializeField] private Color frontPressed = Color.gray;

    [Header("Кольори Тіні (Shadow)")]
    [SerializeField] private Color shadowNormal = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color shadowHover = new Color(0.6f, 0.6f, 0.6f, 1f);
    [SerializeField] private Color shadowPressed = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("Налаштування анімації (Масштаб)")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float pressedScale = 0.9f;
    [SerializeField] private float animDuration = 0.2f;

    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;

        // Встановлюємо стартові кольори
        if (frontImage != null) frontImage.color = frontNormal;
        if (shadowImage != null) shadowImage.color = shadowNormal;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(_originalScale * hoverScale, animDuration).SetEase(Ease.OutBack);
        if (frontImage != null) frontImage.DOColor(frontHover, animDuration);
        if (shadowImage != null) shadowImage.DOColor(shadowHover, animDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(_originalScale, animDuration).SetEase(Ease.OutQuad);
        if (frontImage != null) frontImage.DOColor(frontNormal, animDuration);
        if (shadowImage != null) shadowImage.DOColor(shadowNormal, animDuration);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(_originalScale * pressedScale, animDuration * 0.5f).SetEase(Ease.OutQuad);
        if (frontImage != null) frontImage.DOColor(frontPressed, animDuration * 0.5f);
        if (shadowImage != null) shadowImage.DOColor(shadowPressed, animDuration * 0.5f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(_originalScale * hoverScale, animDuration).SetEase(Ease.OutElastic);
        if (frontImage != null) frontImage.DOColor(frontHover, animDuration);
        if (shadowImage != null) shadowImage.DOColor(shadowHover, animDuration);
    }

    private void OnDisable()
    {
        transform.DOKill();
        if (frontImage != null) frontImage.DOKill();
        if (shadowImage != null) shadowImage.DOKill();

        transform.localScale = _originalScale;
        if (frontImage != null) frontImage.color = frontNormal;
        if (shadowImage != null) shadowImage.color = shadowNormal;
    }
}