using System.Collections.Generic;
using UnityEngine;

public class PopUpNames
{
    public const string TOWER_SHOP_MENU = "Tower Shop";
    public const string TOWER_MENU = "Tower Upgrades";
}

public class PopUpManager : MonoBehaviour {

    public static PopUpManager s_Instance;
    [SerializeField]private List<PopUp> m_PopUps = new List<PopUp>();


    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Shows a small pop up window
    /// </summary>
    /// <param name="popUpName">Pop up to open</param>
    /// <param name="position">Spawn position of the pop up</param>
    public void ShowPopUp(string popUpName, Tile tile)
    {
        for (int i = 0; i < m_PopUps.Count; i++)
        {
            if (m_PopUps[i].name.ToUpper() == popUpName.ToUpper())
            {
                if(m_PopUps[i].LastClickedFromTile != tile)
                    m_PopUps[i].Show(tile);
            }
            else
            {
                //Disable the other pop ups
                m_PopUps[i].Hide();
            }
        }
    }

    /// <summary>
    /// Hide a single pop up
    /// </summary>
    /// <param name="popUpName">Pop up to hide by name reference</param>
    public void HidePopUp(string popUpName)
    {
        PopUp popup = m_PopUps.Find(x => x.name.ToUpper() == popUpName.ToUpper()) as PopUp;
        HidePopUp(popup);
    }

    /// <summary>
    /// Hide a single pop up
    /// </summary>
    /// <param name="popup">Pop up to hide by script reference</param>
    public void HidePopUp(PopUp popup)
    {
        popup.ClearLastClickedTile();
        popup.Hide();
    }

    /// <summary>
    /// Hide all pop ups
    /// </summary>
    public void HideAll()
    {
        for (int i = 0; i < m_PopUps.Count; i++)
        {
            if (m_PopUps[i].isActiveAndEnabled)
                HidePopUp(m_PopUps[i]);
        }
    }

    /// <summary>
    /// Checks if the pop up is already opened on the selected tile
    /// </summary>
    /// <param name="tile">The tile to check</param>
    /// <returns></returns>
    public bool IsPopUpOpenOnTile(Tile tile)
    {
        for (int i = 0; i < m_PopUps.Count; i++)
        {
            if (m_PopUps[i].LastClickedFromTile == tile)
                return true;
        }
        return false;
    }
}