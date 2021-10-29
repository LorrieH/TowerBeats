using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    /// <summary>
    /// Available Map sizes(Info)
    /// </summary>
    public enum Mapsizes
    {
        Small,
        Medium,
        Large,
        Locked,
        
    }

    /// <summary>
    /// Available Map difficulties(Info)
    /// </summary>
    public enum MapDifficulties
    {
        Easy,
        Normal,
        Hard,
        Locked
    }

    /// <summary>
    /// Amount of turrets you can place in the map(Info)
    /// </summary>
    public int TurretPlacements;

    /// <summary>
    /// The level's map size(Info)
    /// </summary>
    [Header("Map size")]
    public Mapsizes MapSize = Mapsizes.Locked;

    /// <summary>
    /// The level's map difficulty(Info)
    /// </summary>
    [Header("Map difficulty")]
    public MapDifficulties MapDifficulty = MapDifficulties.Locked;

    /// <summary>
    /// The level's map name(Info)
    /// </summary>
    [Header("Map name")]
    public string MapName;

    /// <summary>
    /// The level's playlist(Info)
    /// </summary>
    [Space(5)]
    public string[] Songs;

    /// <summary>
    /// Bool to check whether the level is locked or not.
    /// </summary>
    [Space(5)]
    public bool Locked;
}
