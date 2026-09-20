using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanelFader : MonoBehaviour
{
    [Tooltip("Тривалість анімації появи/зникнення в секундах")]
    [SerializeField] private float _fadeDuration = 0.3f;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _canvasGroup.DOKill();
        _canvasGroup.alpha = 0f;

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = false;

        _canvasGroup.DOFade(1f, _fadeDuration)
            .SetLink(gameObject)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                _canvasGroup.interactable = true;
            });
    }

    public void Hide()
    {
        _canvasGroup.DOKill();

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        _canvasGroup.DOFade(0f, _fadeDuration)
            .SetLink(gameObject)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    private void OnDestroy()
    {
        _canvasGroup.DOKill();
    }
}