using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Credits : MonoBehaviour
{
    [SerializeField] private float m_ShowingTime;
    [Space]
    [SerializeField] private ScrollRect m_ScrollRect;
    [SerializeField] private Button m_BackButton;

    private void OnEnable()
    {
        ShowCredits();
    }

    /// <summary>
    /// Shows the credits
    /// </summary>
    private void ShowCredits()
    {
        SetCreditsDefaultState();
        m_ScrollRect.DOVerticalNormalizedPos(0, m_ShowingTime).SetEase(Ease.Linear).SetId("Credits");
        m_BackButton.transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo).SetDelay(m_ShowingTime - 7f).SetId("Credits");
    }

    /// <summary>
    /// Resets the credits screen so it starts at the top again
    /// </summary>
    private void SetCreditsDefaultState()
    {
        DOTween.Kill("Credits");
        m_ScrollRect.verticalNormalizedPosition = 1;
        m_BackButton.gameObject.transform.localScale = Vector3.zero;
    }
}