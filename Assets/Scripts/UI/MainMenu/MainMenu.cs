using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform[] m_Buttons;
    [SerializeField] private Credits m_Credits;
    [SerializeField] private RectTransform m_PlatformContainer;
    [SerializeField] private CanvasGroup m_PlatformAlpha;

    #endregion

    #region Monobehaviour Functions

    /// <summary>
    /// Gets called when the GameObject initializes
    /// </summary>
    private void Start()
    {
        // Play the background music if it isn't playing already
        if (!SoundManager.s_Instance.IsSoundPlaying(SoundNames.BACKGROUND_MUSIC))
            SoundManager.s_Instance.PlaySound(SoundNames.BACKGROUND_MUSIC, true);

        // Show the Main Menu buttons
        ShowMainButtons();

        // Show the Don Diablo Avatar
        ShowDonDiablo();
    }

    #endregion

    #region Sceneloading

    /// <summary>
    /// Load a Scene (Calls the Sceneloader Instance)
    /// </summary>
    /// <param name="scene">Scene name</param>
    public void LoadScene(string scene)
    {
        // Load the scene
        Sceneloader.s_Instance.LoadScene(scene);

        // Play button click sound
        SoundManager.s_Instance.PlaySound(SoundNames.BUTTON_CLICK);
    }

    #endregion

    #region Credits

    /// <summary>
    /// Show the Credits
    /// </summary>
    public void ShowCredits()
    {
        // Hide the buttons
        StartCoroutine(AnimateButtons(false));

        // Set the credits active
        m_Credits.gameObject.SetActive(true);

        // Play button click sound
        SoundManager.s_Instance.PlaySound(SoundNames.BUTTON_CLICK);
    }

    /// <summary>
    /// Hide the Credits
    /// </summary>
    public void HideCredits()
    {
        // Show the Main Menu buttons
        ShowMainButtons();

        // Hide the Credits
        m_Credits.gameObject.SetActive(false);

        // Play button click sound
        SoundManager.s_Instance.PlaySound(SoundNames.BUTTON_CLICK);
    }

    #endregion

    #region Quitting

    /// <summary>
    /// Quit the Game
    /// </summary>
    public void QuitGame()
    {
        // Play button click sound
        SoundManager.s_Instance.PlaySound(SoundNames.BUTTON_CLICK);

        // Quit the Application
        Application.Quit();
    }

    #endregion

    #region Don Diablo Avatar

    /// <summary>
    /// Sets the default state of Don Diablo's avatar
    /// </summary>
    private void SetDonDiabloDefaultState()
    {
        // Reset to default position
        m_PlatformContainer.anchoredPosition = new Vector2(-25f, -400f);

        // Hide with alpha on 0
        m_PlatformAlpha.alpha = 0;
    }

    /// <summary>
    /// Shows Don Diablo's Avatar
    /// </summary>
    private void ShowDonDiablo()
    {
        // Set the default state
        SetDonDiabloDefaultState();

        // Animate Don Diablo's avatar
        m_PlatformContainer.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutExpo);
        m_PlatformAlpha.DOFade(1, 0.33f);
    }

    #endregion

    #region Main Menu Buttons

    /// <summary>
    /// Show the Main Menu Buttons
    /// </summary>
    public void ShowMainButtons()
    {
        SetButtonsDefaultState();
        StartCoroutine(AnimateButtons(true));
    }

    /// <summary>
    /// Set the default state of the Main Menu Buttons
    /// </summary>
    private void SetButtonsDefaultState()
    {
        for (int i = 0; i < m_Buttons.Length; i++)
            m_Buttons[i].localScale = Vector2.zero;
    }

    /// <summary>
    /// Animate the Main Menu Buttons
    /// </summary>
    /// <param name="animateIn">True == Animate in, false == animate out</param>
    /// <returns></returns>
    private IEnumerator AnimateButtons(bool animateIn)
    {
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            m_Buttons[i].DOScale((animateIn ? 1 : 0), 0.5f).SetEase((animateIn ? Ease.OutExpo : Ease.InExpo));
            yield return new WaitForSeconds(0.05f);
        }
    }

    #endregion
}
