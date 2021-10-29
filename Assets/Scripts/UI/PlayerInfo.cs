using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerInfo : MonoBehaviour
{
    #region Variables

    [SerializeField] private Text m_Coins;
    [SerializeField] private Image m_Lives;
    [SerializeField] private Text m_SongText;
    [SerializeField] private Text m_PreparationTimer;

    #endregion

    #region Monobehaviour Functions

    private void Awake()
    {
        // Add the listeners
        AddListeners(true);

        // Show the healthbar animation
        ShowHealthbarSequence();
    }

    private void OnDestroy()
    {
        // Remove the listeners
        AddListeners(false);
    }

    #endregion

    #region Listeners

    /// <summary>
    /// Adds or removes the Listeners
    /// </summary>
    /// <param name="add">Add the Listeners?</param>
    private void AddListeners(bool add)
    {
        if(add)
        {
            PlayerData.s_OnUpdateCoins += UpdateCoinsUI;
            PlayerData.s_OnUpdateLives += UpdateLivesUI;
            SongManager.s_OnChangeSong += UpdateSongUI;
            GameManager.s_OnPreparationTimeUpdated += UpdatePreparationTime;
        }
        else
        {
            PlayerData.s_OnUpdateCoins -= UpdateCoinsUI;
            PlayerData.s_OnUpdateLives -= UpdateLivesUI;
            SongManager.s_OnChangeSong -= UpdateSongUI;
            GameManager.s_OnPreparationTimeUpdated -= UpdatePreparationTime;
        }
    }

    #endregion

    #region Animations

    /// <summary>
    /// Shows the healthbar startup sequence animation
    /// </summary>
    private void ShowHealthbarSequence()
    {
        // Sets the default fill amount for the animation
        m_Lives.fillAmount = 0;

        // Fills up the bar at the start
        Sequence healthBarStartSequence = DOTween.Sequence();
        healthBarStartSequence.AppendInterval(0.3f);
        healthBarStartSequence.Append(m_Lives.DOFillAmount(1, 3f)).SetEase(Ease.Linear);
        healthBarStartSequence.Insert(0.3f, m_Lives.DOColor(Color.red, 0.2f));
        healthBarStartSequence.Insert(0.7f, m_Lives.DOColor(Effects.s_EffectColors["Yellow"], 0.2f));
        healthBarStartSequence.Insert(1.8f, m_Lives.DOColor(Color.green, 0.2f));
    }

    #endregion

    #region UI Callbacks

    /// <summary>
    /// Shows the players currency
    /// </summary>
    /// <param name="coins"></param>
    void UpdateCoinsUI(float coins)
    {
        m_Coins.text = coins.ToString("N0"); //Makes sure the text wont show decimals
    }

    /// <summary>
    /// Updates the lives
    /// </summary>
    /// <param name="lives">Lives left</param>
    void UpdateLivesUI(float lives)
    {
        m_Lives.DOFillAmount(lives/10,0.3f);
        if(lives > 7)
        {
            Effects.ChangeImageColor(m_Lives, Effects.s_EffectColors["Green"], 0.3f);
        }
        else if(lives <= 7 && lives > 3)
        {
            Effects.ChangeImageColor(m_Lives, Effects.s_EffectColors["Yellow"], 0.3f);
        }
        else if(lives <= 3)
        {
            Effects.ChangeImageColor(m_Lives, Effects.s_EffectColors["Red"], 0.3f);
        }
    }

    /// <summary>
    /// Updates the song
    /// </summary>
    /// <param name="currentSong">Current song index</param>
    /// <param name="maxSongs">Max song count</param>
    /// <param name="songName">Name of the song</param>
    void UpdateSongUI(int currentSong, int maxSongs,string songName)
    {
        m_SongText.text = currentSong + "/" + maxSongs + "  " + songName;
    }

    /// <summary>
    /// Updates the Preparation timer
    /// </summary>
    /// <param name="time">Time left</param>
    private void UpdatePreparationTime(int time)
    {
        m_PreparationTimer.color = new Color(m_PreparationTimer.color.r, m_PreparationTimer.color.g, m_PreparationTimer.color.b, 0);
        m_PreparationTimer.transform.localScale = new Vector3(1.3f, 1.3f);

        m_PreparationTimer.transform.DOScale(1, 0.2f).SetEase(Ease.OutExpo);
        m_PreparationTimer.DOFade(1, 0.1f);

        m_PreparationTimer.transform.DOScale(0.7f, 0.2f).SetEase(Ease.InExpo).SetDelay(0.55f);
        m_PreparationTimer.DOFade(0, 0.1f).SetDelay(0.65f);

        if(time == 0)
        {
            m_PreparationTimer.text = "";
        }
        else
            m_PreparationTimer.text = time.ToString();
    }

    #endregion
}