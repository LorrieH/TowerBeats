using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class TowerUpgradesAnimation : MonoBehaviour
{

    #region variables for animating
    [SerializeField] private Image m_Indicator;
    [SerializeField] private CanvasGroup m_SellButton;
    [SerializeField] private CanvasGroup m_UpgradeButton;
    [SerializeField] private Text m_TurretName;
    [SerializeField] private Text m_TurretLevel;
    [SerializeField] private Image m_Line;
    [SerializeField] private RectTransform m_DamageContainer;
    [SerializeField] private CanvasGroup m_TowerTargeting;

    private Sequence m_IndicatorSeq;

    public bool AnimatedIn { get; set; }
    #endregion

    /// <summary>
    /// Animation for enabling the tower menu
    /// </summary>
    /// <param name="callback">Callback to be called on completion</param>
    public void AnimateIn(Action callback = null)
    {
        if (AnimatedIn)
        {
            KillTweens();
        }

        ResetToDefault();
        SetIndicatorState(true);

        //Tower Targeting Animation
        m_TowerTargeting.transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo).SetId("TowerUpgradeAnimateIn");
        m_TowerTargeting.DOFade(1, 0.2f).SetId("TowerUpgradeAnimateIn");

        //Upgrade button animation
        m_UpgradeButton.transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo).SetId("TowerUpgradeAnimateIn");
        m_UpgradeButton.DOFade(1, 0.2f).SetId("TowerUpgradeAnimateIn");

        float upgradeButtonDelay = 0.1f;

        //Turret name animation
        m_TurretName.rectTransform.DOAnchorPosX(130, 0.33f, true).SetEase(Ease.OutExpo).SetDelay(upgradeButtonDelay).SetId("TowerUpgradeAnimateIn");
        m_TurretName.DOFade(1, 0.2f).SetDelay(upgradeButtonDelay + 0.13f).SetId("TowerUpgradeAnimateIn");

        float turretNameDelay = 0.15f;

        //Turret level animation
        m_TurretLevel.rectTransform.DOAnchorPosX(130, 0.33f, true).SetEase(Ease.OutExpo).SetDelay(upgradeButtonDelay + turretNameDelay).SetId("TowerUpgradeAnimateIn");
        m_TurretLevel.DOFade(1, 0.2f).SetDelay((upgradeButtonDelay + turretNameDelay) + 0.13f).SetId("TowerUpgradeAnimateIn");

        //Line animation
        m_Line.transform.DOScaleX(1, 0.33f).SetEase(Ease.OutExpo).SetDelay(upgradeButtonDelay).SetId("TowerUpgradeAnimateIn");
        m_Line.DOFade(1, 0.1f).SetDelay(upgradeButtonDelay).SetId("TowerUpgradeAnimateIn");

        //Sell button animation
        m_SellButton.transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo).SetDelay(0.1f).SetId("TowerUpgradeAnimateIn");
        m_SellButton.DOFade(1, 0.2f).SetDelay(0.1f).SetId("TowerUpgradeAnimateIn");

        //Damage container
        m_DamageContainer.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutExpo).SetDelay(upgradeButtonDelay + turretNameDelay).OnComplete(delegate { if (callback != null) callback(); AnimatedIn = true; }).SetId("TowerUpgradeAnimateIn");
    }

    /// <summary>
    /// Animation for disabling the tower menu
    /// </summary>
    /// <param name="callback">Callback to be called on completion</param>
    public void AnimateOut(Action callback = null)
    {
        if (!AnimatedIn)
        {
            KillTweens();
        }

        SetIndicatorState(false);

        //Damage container
        m_DamageContainer.DOAnchorPosY(35f, 0.5f).SetEase(Ease.InExpo).SetId("TowerUpgradeAnimateOut");

        //Line animation
        m_Line.transform.DOScaleX(0, 0.33f).SetEase(Ease.InExpo).SetDelay(0.1f).SetId("TowerUpgradeAnimateOut");
        m_Line.DOFade(0, 0.1f).SetDelay(0.23f).SetId("TowerUpgradeAnimateOut");

        //Turret name animation
        m_TurretName.rectTransform.DOAnchorPosX(200, 0.33f, true).SetEase(Ease.InExpo).SetDelay(0.1f).SetId("TowerUpgradeAnimateOut");
        m_TurretName.DOFade(0, 0.2f).SetDelay(0.1f + 0.13f).SetId("TowerUpgradeAnimateOut");

        //Turret level animation
        m_TurretLevel.rectTransform.DOAnchorPosX(200, 0.33f, true).SetEase(Ease.InExpo).SetDelay(0.15f).SetId("TowerUpgradeAnimateOut");
        m_TurretLevel.DOFade(0, 0.2f).SetDelay(0.15f + 0.13f).SetId("TowerUpgradeAnimateOut");

        //Sell button animation
        m_SellButton.transform.DOScale(0, 0.5f).SetEase(Ease.InExpo).SetDelay(0.2f).SetId("TowerUpgradeAnimateOut");
        m_SellButton.DOFade(0, 0.2f).SetDelay(0.4f).SetId("TowerUpgradeAnimateOut");

        //Upgrade button animation
        m_UpgradeButton.transform.DOScale(0, 0.5f).SetEase(Ease.InExpo).SetDelay(0.3f).OnComplete(delegate { if (gameObject.activeInHierarchy) { if (callback != null) callback(); AnimatedIn = false; }; }).SetId("TowerUpgradeAnimateOut");
        m_UpgradeButton.DOFade(0, 0.2f).SetDelay(0.5f).SetId("TowerUpgradeAnimateOut");

        //targeting button animation
        m_TowerTargeting.transform.DOScale(0, 0.5f).SetEase(Ease.InExpo).SetDelay(0.3f).SetId("TowerUpgradeAnimateOut");
        m_TowerTargeting.DOFade(0, 0.2f).SetDelay(0.5f).SetId("TowerUpgradeAnimateOut");
    }

    /// <summary>
    /// Reset the UI to its default values (colors, positions and scales)
    /// </summary>
    private void ResetToDefault()
    {
        //Indicator
        m_Indicator.color = new Color(m_Indicator.color.r, m_Indicator.color.g, m_Indicator.color.b, 0);
        m_Indicator.rectTransform.anchoredPosition = new Vector2(m_Indicator.rectTransform.anchoredPosition.x, 100);

        //targeting button
        m_TowerTargeting.transform.localScale = new Vector2(1.3f, 1.3f);
        m_TowerTargeting.alpha = 0;

        //Upgrade button
        m_UpgradeButton.transform.localScale = new Vector2(1.3f, 1.3f);
        m_UpgradeButton.alpha = 0;

        //Sell button
        m_SellButton.transform.localScale = new Vector2(1.3f, 1.3f);
        m_SellButton.alpha = 0;

        //Turret name
        m_TurretName.rectTransform.anchoredPosition = new Vector2(200, m_TurretName.rectTransform.anchoredPosition.y);
        m_TurretName.color = new Color(m_TurretName.color.r, m_TurretName.color.g, m_TurretName.color.b, 0);

        //Turret level
        m_TurretLevel.rectTransform.anchoredPosition = new Vector2(200, m_TurretLevel.rectTransform.anchoredPosition.y);
        m_TurretLevel.color = new Color(m_TurretLevel.color.r, m_TurretLevel.color.g, m_TurretLevel.color.b, 0);

        //Line
        m_Line.color = new Color(m_Line.color.r, m_Line.color.g, m_Line.color.b, 0);
        m_Line.transform.localScale = new Vector3(0, 1, 1);

        //Damage container
        m_DamageContainer.anchoredPosition = new Vector2(m_DamageContainer.anchoredPosition.x, 35f);
    }

    /// <summary>
    /// Kill tweens to avoid bugs
    /// </summary>
    private void KillTweens()
    {
        DOTween.Kill("TowerUpgradeAnimateIn");
        DOTween.Kill("TowerUpgradeAnimateOut");
    }

    private void OnEnable()
    {
        KillTweens();
    }

    private void OnDisable()
    {
        KillTweens();
    }

    /// <summary>
    /// Animation for the indicator
    /// </summary>
    /// <param name="active"></param>
    public void SetIndicatorState(bool active)
    {
        if(m_IndicatorSeq != null)
            m_IndicatorSeq.Kill();

        m_IndicatorSeq = DOTween.Sequence(); //Sets the sequence to a new sequence

        if (active)
        {
            m_IndicatorSeq.Append(m_Indicator.rectTransform.DOAnchorPosY(0, 0.33f, true).SetEase(Ease.OutExpo));
            m_IndicatorSeq.Join(m_Indicator.DOFade(1, 0.2f));
            m_IndicatorSeq.Append(m_Indicator.rectTransform.DOAnchorPosY(30, 1f, true).SetEase(Ease.InOutQuad).SetLoops(int.MaxValue, LoopType.Yoyo));
        }
        else
        {
            m_IndicatorSeq.Append(m_Indicator.rectTransform.DOAnchorPosY(100, 0.33f, true).SetEase(Ease.InExpo));
            m_IndicatorSeq.Join(m_Indicator.DOFade(0, 0.2f).SetDelay(0.13f));
        }

    }
}