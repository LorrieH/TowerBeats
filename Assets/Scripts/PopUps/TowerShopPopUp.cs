using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerShopPopUp : PopUp {

    [SerializeField] private List<Tower> m_Towers = new List<Tower>();

    [Header("Text colors")]
    [SerializeField] private Color m_AvailableColor;
    [SerializeField] private Color m_UnavailableColor;

    [Header("Tower costs")]
    [SerializeField] private Text m_BassTowerCost;
    [SerializeField] private Text m_DrumTowerCost;
    [SerializeField] private Text m_LeadTowerCost;

    [Space]
    [SerializeField] private TowerShopAnimation m_Animation;

    private Tile m_CurrentTile;

    /// <summary>
    /// Shows the tower shop pop up
    /// </summary>
    /// <param name="calledFrom">The tile this is called on</param>
    public override void Show(Tile calledFrom)
    {
        UpdateCosts();

        m_CurrentTile = calledFrom;
        base.Show(calledFrom);
        PlayerData.s_OnUpdateCoins += OnPlayerCoinsUpdated;
        m_Animation.AnimateIn();
    }

    /// <summary>
    /// Hides the tower shop pop up
    /// </summary>
    public override void Hide()
    {
        PlayerData.s_OnUpdateCoins -= OnPlayerCoinsUpdated;
        ClearLastClickedTile();
        m_Animation.AnimateOut(delegate {
            base.Hide();
        });
    }

    private void OnDestroy()
    {
        PlayerData.s_OnUpdateCoins -= OnPlayerCoinsUpdated;
    }

    /// <summary>
    /// Gets called when the Player's coins are updated
    /// </summary>
    /// <param name="value"></param>
    private void OnPlayerCoinsUpdated(float value)
    {
        UpdateCosts();
    }

    /// <summary>
    /// Updates the text
    /// </summary>
    private void UpdateCosts()
    {
        CompareCostAndSetText(TowerConfig.s_Towers[TowerTypeTags.BASS_TOWER][0].BuyCost, m_BassTowerCost);
        CompareCostAndSetText(TowerConfig.s_Towers[TowerTypeTags.DRUM_TOWER][0].BuyCost, m_DrumTowerCost);
        CompareCostAndSetText(TowerConfig.s_Towers[TowerTypeTags.LEAD_TOWER][0].BuyCost, m_LeadTowerCost);
    }

    /// <summary>
    /// Checks if the player has enough currency and sets the color of the text accordingly
    /// </summary>
    /// <param name="cost">Cost value of a tower</param>
    /// <param name="textToColor">Text target that gets colored</param>
    void CompareCostAndSetText(float cost, Text textToColor)
    {
        if (textToColor == null) return;

        float coins = PlayerData.s_Instance.Coins;
        textToColor.text = cost.ToString();
        if (coins >= cost)
        {
            textToColor.color = m_AvailableColor;
        }else
        {
            textToColor.color = m_UnavailableColor;
        }
    }

    /// <summary>
    /// Purchase a tower
    /// </summary>
    /// <param name="towerType">The type of tower the player tries to purchase</param>
    public void PurchaseTower(string towerType)
    {
        //If player has enough coins
        if(TowerConfig.s_Towers[towerType][0].BuyCost <= PlayerData.s_Instance.Coins && m_CurrentTile.CurrentState == TileState.TURRET_SPAWN)
        {
            //Gets the buy cost from the towers data
            PlayerData.s_Instance.ChangeCoinAmount(-TowerConfig.s_Towers[towerType][0].BuyCost);
            
            //Spawns a tower of the type (parameter) passed
            switch (towerType)
            {
                case TowerTypeTags.BASS_TOWER:
                    SpawnTower(towerType, 0);
                    break;
                case TowerTypeTags.DRUM_TOWER:
                    SpawnTower(towerType, 1);
                    break;
                case TowerTypeTags.LEAD_TOWER:
                    SpawnTower(towerType, 2);
                    break;
            }
            m_CurrentTile.CurrentState = TileState.OCCUPIED;
        }
    }

    /// <summary>
    /// Spawn a purchased tower on the map
    /// </summary>
    /// <param name="towerType">Type of tower to spawn</param>
    /// <param name="indexInList">The index in the tower list. Check the list to get the correct tower</param>
    void SpawnTower(string towerType,int indexInList)
    {
        EffectsManager.s_Instance.SpawnEffect(EffectType.TURRET_SPAWN, false, m_CurrentTile.transform.position);
        Tower newTower;
        newTower = Instantiate(m_Towers[indexInList]);
        newTower.TowerData = TowerConfig.s_Towers[towerType][0];
        newTower.transform.position = m_CurrentTile.transform.position;
        m_CurrentTile.Tower = newTower;

        PopUpManager.s_Instance.ShowPopUp(PopUpNames.TOWER_MENU, m_CurrentTile);

        int orderInLayer = (HexGrid.s_Instance.GridSize.y - m_CurrentTile.Y);

        newTower.GetComponent<Renderer>().sortingOrder = orderInLayer;
    }
}