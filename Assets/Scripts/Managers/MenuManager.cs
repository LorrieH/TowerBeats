using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Names of all menus.
/// </summary>
public struct MenuNames
{
    public const string SETTINGS_MENU = "SettingsMenu";
    public const string VICTORY_MENU = "VictoryMenu";
    public const string CREDITS_MENU = "CreditsMenu";
    public const string TOWER_SHOP_MENU = "TowerShopMenu";
    public const string TOWER_MENU = "TowerMenu";
    public const string GAME_OVER_MENU = "GameOverMenu";
    public const string BACK_MENU = "BackMenu";
}

public class MenuManager : MonoBehaviour
{
    /// <summary>
    /// Instance of this script.
    /// </summary>
    public static MenuManager s_Instance;

    /// <summary>
    /// Delegate for opening a menu.
    /// </summary>
    /// <param name="menu">Menu to open.</param>
    public delegate void MenuOpened(Menu menu);
    public static MenuOpened s_OnMenuOpened;

    /// <summary>
    /// Delegate for closing a menu.
    /// </summary>
    /// <param name="menu">menu to close.</param>
    public delegate void MenuClosed(Menu menu);
    public static MenuClosed s_OnMenuClosed;

    /// <summary>
    /// Is the game paused?
    /// </summary>
    public static bool s_IsPaused;

    /// <summary>
    /// List of all menus.
    /// </summary>
    [SerializeField] private List<Menu> m_Menus = new List<Menu>();

    /// <summary>
    /// Current opened menu.
    /// </summary>
    private Menu m_CurrentOpenMenu;

    /// <summary>
    /// Is there an active menu?
    /// </summary>
    public bool IsAnyMenuOpen
    {
        get { return (m_CurrentOpenMenu != null) ? true : false; }
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Shows a menu determined by the string passed as parameter
    /// </summary>
    /// <param name="menuName"></param>
    public void ShowMenu(string menuName)
    {
        for (int i = 0; i < m_Menus.Count; i++)
        {
            if (m_Menus[i].name == menuName)
                ShowMenu(m_Menus[i]);
        }
    }

    /// <summary>
    /// Shows a menu, component has to inherit from Menu
    /// </summary>
    /// <param name="menu">The menu to open</param>
    public void ShowMenu(Menu menu)
    {
        if (IsAnyMenuOpen)
        {
            m_CurrentOpenMenu.Hide();
            m_CurrentOpenMenu = null;
        }
        menu.Show();
        m_CurrentOpenMenu = menu;
        if (s_OnMenuOpened != null) s_OnMenuOpened(menu);
    }

    /// <summary>
    /// Shows a menu
    /// </summary>
    /// <param name="menuName">Menu to show by name reference</param>
    /// <param name="position">Position for menu to spawn at</param>
    public void ShowMenu(string menuName,Vector3 position)
    {
        Camera cam = Camera.main;
        for (int i = 0; i < m_Menus.Count; i++)
        {
            if (m_Menus[i].name == menuName)
            {
                ShowMenu(m_Menus[i]);
                m_Menus[i].transform.position = cam.WorldToScreenPoint(position);
            }
        }
    }

    /// <summary>
    /// Hides a menu
    /// </summary>
    /// <param name="menuName">Menu to hide by name reference</param>
    public void HideMenu(string menuName)
    {
        for (int i = 0; i < m_Menus.Count; i++)
        {
            if(m_Menus[i].name == menuName)
            {
                HideMenu(m_Menus[i]);
            }
        }
    }

    /// <summary>
    /// Hides a menu
    /// </summary>
    /// <param name="menu">Menu to hide by script reference</param>
    public void HideMenu(Menu menu)
    {
        menu.Hide();
    }

    /// <summary>
    /// Toggles a menu, if it is already open it closes
    /// </summary>
    /// <param name="menu">menu to toggle</param>
    public void ToggleMenu(Menu menu)
    {
        if (IsAnyMenuOpen)
        {
            if (m_CurrentOpenMenu != menu)
            {
                m_CurrentOpenMenu.Hide();
                menu.Show();
                m_CurrentOpenMenu = menu;
                if (s_OnMenuOpened != null) s_OnMenuOpened(menu);
            }
            else
            {
                m_CurrentOpenMenu = null;
                menu.Hide();
            }
        }
        else
        {
            menu.Show();
            m_CurrentOpenMenu = menu;
            if (s_OnMenuOpened != null) s_OnMenuOpened(menu);
        }
    }

    /// <summary>
    /// Loads a scene
    /// </summary>
    /// <param name="sceneName">Scene to load by string</param>
    public void LoadScene(string sceneName)
    {
        Sceneloader.s_Instance.LoadScene(sceneName);
    }

    /// <summary>
    /// Reloads the scene and map
    /// </summary>
    public void ReloadGameScene()
    {
        Sceneloader.s_Instance.LoadGameSceneWithLevel(Sceneloader.s_Instance.m_LevelToLoad);
    }

    public void StopAllAudio()
    {
        SongManager.s_Instance.ClearAudio();
    }
}