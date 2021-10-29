using System.Collections.Generic;

public class TowerConfig
{
    /// <summary>
    /// Dictionary which holds the data for all the towers and their upgrades.
    /// </summary>
    public static Dictionary<string, List<TowerData>> s_Towers = new Dictionary<string, List<TowerData>>()
    {
        // TYPE, LEVEL, MAXLEVEL, UPGRADECOST, BUYCOST, VALUE (Sell), DAMAGE, RANGE, INTERVAL
        {TowerTypeTags.BASS_TOWER, new List<TowerData>() {
            new TowerData(TowerTypeTags.BASS_TOWER, 1, 3, 200, 150, 75, 1.5f, 3.1f, 1.5f),
            new TowerData(TowerTypeTags.BASS_TOWER, 2, 3, 200, 0, 225, 3f, 3.1f, 1.25f),
            new TowerData(TowerTypeTags.BASS_TOWER, 3, 3, 0, 0, 375, 4.5f, 3.1f, .75f)
        }},
        {TowerTypeTags.DRUM_TOWER, new List<TowerData>()
        {
            new TowerData(TowerTypeTags.DRUM_TOWER, 1, 3, 200, 300, 225, 4f, 6f, 0.35f),
            new TowerData(TowerTypeTags.DRUM_TOWER, 2, 3, 300, 0, 375, 5.5f, 8f,0.3f),
            new TowerData(TowerTypeTags.DRUM_TOWER, 3, 3, 0, 0 , 600, 8.5f, 10f, 0.25f)
        }},
        {TowerTypeTags.LEAD_TOWER, new List<TowerData>()
        {  
            new TowerData(TowerTypeTags.LEAD_TOWER, 1, 3, 200, 200, 150, 0.75f, 3.25f, 0.025f),
            new TowerData(TowerTypeTags.LEAD_TOWER, 2, 3, 300, 0, 300, 1.5f, 3.5f, 0.025f),
            new TowerData(TowerTypeTags.LEAD_TOWER, 3, 3, 0, 0, 525, 3f, 3.75f, 0.025f)
        }}
    };
}//