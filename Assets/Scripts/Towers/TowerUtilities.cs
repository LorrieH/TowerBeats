using UnityEngine;

public class TowerUtilities : MonoBehaviour {

    public delegate void UpgradeEvent();
    public static UpgradeEvent s_OnUpgrade;

    public Tile CurrentTile { get; set; }

    /// <summary>
    /// Sells a tower and returns 75% of the coins invested in the tower
    /// </summary>
    public void SellTower()
    {
        if (CurrentTile == null) return;

        PlayerData.s_Instance.ChangeCoinAmount(CurrentTile.Tower.TowerData.SellValue);
        CurrentTile.Tower.Sell();
        CurrentTile.Tower = null;
        CurrentTile.CurrentState = TileState.TURRET_SPAWN;
        CurrentTile.SetTileVisualsState(TileVisualState.TURRET_SPAWN);
        PopUpManager.s_Instance.HidePopUp(PopUpNames.TOWER_MENU);
    }

    /// <summary>
    /// Upgrade a tower to the next level
    /// </summary>
    public void Upgrade()
    {
        if (CurrentTile == null) return;

        if (CurrentTile.Tower.TowerData.Level < CurrentTile.Tower.TowerData.MaxLevel 
            && PlayerData.s_Instance.Coins >= CurrentTile.Tower.TowerData.UpgradeCost)
        {
            PlayerData.s_Instance.ChangeCoinAmount(-CurrentTile.Tower.TowerData.UpgradeCost);
            CurrentTile.Tower.TowerData = TowerConfig.s_Towers[CurrentTile.Tower.TowerData.Type][CurrentTile.Tower.TowerData.Level];
            if (s_OnUpgrade != null)
            {
                s_OnUpgrade();
            }
        }
    }

    /// <summary>
    /// Sets the towers targeting type
    /// </summary>
    public void ChangeTowerTargeting()
    {
        if (CurrentTile == null) return;

        switch (CurrentTile.Tower.TargetType)
        {
            case TargetTypes.NORMAL:
                CurrentTile.Tower.TargetType = TargetTypes.FURTHEST;
                break;
            case TargetTypes.FURTHEST:
                CurrentTile.Tower.TargetType = TargetTypes.CLOSEST;
                break;
            case TargetTypes.CLOSEST:
                CurrentTile.Tower.TargetType = TargetTypes.NORMAL;
                break;
        }

        if (s_OnUpgrade != null)
        {
            s_OnUpgrade();
        }
    }
}