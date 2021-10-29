using UnityEngine;

public class PlayerData : MonoBehaviour {

    public delegate void PlayerDataUpdate(float dataChangeValue);
    public static PlayerDataUpdate s_OnUpdateCoins;
    public static PlayerDataUpdate s_OnUpdateLives;

    public static PlayerData s_Instance;

    [SerializeField]private float m_Coins;
    public float Coins
    {
        get { return m_Coins; }
        set { m_Coins = value; }
    }

    [SerializeField] private int m_Lives;
    public int Lives
    {
        get { return m_Lives; }
        set { m_Lives = value; }
    }

    private void Awake()
    {
        if(s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Adds or removes coins (-50 for example removes 50 coins) and updates the UI to show these changes
    /// </summary>
    /// <param name="coins">Amount of coins to add</param>
    public void ChangeCoinAmount(float coins)
    {
        m_Coins += coins;
        if (s_OnUpdateCoins != null)
        {
            s_OnUpdateCoins(m_Coins);
        }
        if(m_Coins <= 0)
        {
            m_Coins = 0; //If coins drop under 0, rounds the amount of coins up to 0
        }
    }

    /// <summary>
    /// Adds or removes lives and updates the UI to show these changes
    /// </summary>
    /// <param name="lives">Lives to add</param>
    public void ChangeLivesAmount(int lives)
    {
        m_Lives += lives;
        if (s_OnUpdateLives != null)
        {
            s_OnUpdateLives(m_Lives);
        }

        if (m_Lives <= 0)
        {
            //Show game over panel
            GameManager.s_OnGameStop();
            MenuManager.s_Instance.ShowMenu(MenuNames.GAME_OVER_MENU);
        }
    }
}