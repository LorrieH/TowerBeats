using UnityEngine;

public class Menu : MonoBehaviour
{
    /// <summary>
    /// Is this menu open?
    /// </summary>
    private bool m_IsMenuOpen;
    public bool IsMenuOpen { get { return m_IsMenuOpen; } set { m_IsMenuOpen = value; } }

    /// <summary>
    /// Show this menu.
    /// </summary>
    public virtual void Show()
    {
        m_IsMenuOpen = true;
        gameObject.SetActive(true);
        MenuManager.s_IsPaused = true;
    }

    /// <summary>
    /// Hide this menu.
    /// </summary>
    public virtual void Hide()
    {
        m_IsMenuOpen = false;
        gameObject.SetActive(false);

        if (MenuManager.s_OnMenuClosed != null) MenuManager.s_OnMenuClosed(this);
        MenuManager.s_IsPaused = false;
    }
}