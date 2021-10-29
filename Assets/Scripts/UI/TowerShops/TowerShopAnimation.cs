using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class TowerShopAnimation : MonoBehaviour
{
    /// <summary>
    /// Indicator for the tower.
    /// </summary>
    [SerializeField] private Image m_Indicator;

    /// <summary>
    /// Canvas group of the buttons.
    /// </summary>
    [SerializeField] private CanvasGroup[] m_Buttons;

    /// <summary>
    /// Animate in?
    /// </summary>
    public bool AnimatedIn { get; set; }

    /// <summary>
    /// Animation for enabling the tower shop menu
    /// </summary>
    /// <param name="callback">Callback to be called on completion</param>
    public void AnimateIn(Action callback = null)
    {
        if (AnimatedIn)
        {
            KillTweens();
        }

        // Reset animating items to their default state
        ResetToDefault();
        
        // Show the indicator
        m_Indicator.rectTransform.DOAnchorPosY(-65, 0.33f).SetEase(Ease.OutExpo).SetId("TowerShopAnimateIn");
        m_Indicator.transform.DOScale(1, 0.33f).SetEase(Ease.OutExpo).SetId("TowerShopAnimateIn");
        m_Indicator.DOFade(1, 0.2f).SetId("TowerShopAnimateIn");

        // Show the buttons
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            m_Buttons[i].DOFade(1, 0.2f).SetDelay((0.15f * i) - (0.05f * i));
            if (i == m_Buttons.Length - 1)
                m_Buttons[i].transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo).SetDelay((0.15f * i) - (0.05f * i)).OnComplete(delegate { if (callback != null) callback(); AnimatedIn = true; }).SetId("TowerShopAnimateIn");
            else
                m_Buttons[i].transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo).SetDelay((0.15f * i) - (0.05f * i)).SetId("TowerShopAnimateIn");
        }
    }

    /// <summary>
    /// Animation for disabling the tower shop menu
    /// </summary>
    /// <param name="callback">Callback to be called on completion</param>
    public void AnimateOut(Action callback = null)
    {
        if (!AnimatedIn)
        {
            KillTweens();
        }

        // Hide the buttons
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            m_Buttons[i].DOFade(0, 0.2f).SetDelay(0.13f).SetId("TowerShopAnimateOut");
            m_Buttons[i].transform.DOScale(0.5f, 0.33f).SetEase(Ease.InExpo).SetId("TowerShopAnimateOut");
        }

        // Hide the indicator
        m_Indicator.rectTransform.DOAnchorPosY(-65f * 2f, 0.33f).SetEase(Ease.InExpo).OnComplete(delegate { if (gameObject.activeInHierarchy) { if (callback != null) callback(); AnimatedIn = false; }; }).SetId("TowerShopAnimateOut");
        m_Indicator.DOFade(0, 0.2f).SetDelay(0.13f).SetId("TowerShopAnimateOut");
    }

    /// <summary>
    /// Resets the UI to its default values (colors, position and scale)
    /// </summary>
    private void ResetToDefault()
    {
        m_Indicator.color = new Color(m_Indicator.color.r, m_Indicator.color.g, m_Indicator.color.b, 0);
        m_Indicator.rectTransform.DOAnchorPosY(65, 0, true);
        m_Indicator.transform.localScale = Vector2.zero;

        for (int i = 0; i < m_Buttons.Length; i++)
        {
            m_Buttons[i].alpha = 0;
            m_Buttons[i].transform.localScale = new Vector2(1.3f, 1.3f);
        }
    }

    /// <summary>
    /// Kills animation tweens to avoid bugs
    /// </summary>
    private void KillTweens()
    {
        DOTween.Kill("TowerShopAnimateIn");
        DOTween.Kill("TowerShopAnimateOut");
    }

    private void OnEnable()
    {
        KillTweens();
    }

    private void OnDisable()
    {
        KillTweens();
    }
}