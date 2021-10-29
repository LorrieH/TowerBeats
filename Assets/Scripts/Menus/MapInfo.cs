using UnityEngine;
using UnityEngine.UI;

public class MapInfo : MonoBehaviour {

    /// <summary>
    /// Delegate which gets called whenever the player changes a level.
    /// </summary>
    /// <param name="selectedLevel"></param>
    public delegate void OnLevelChange(Level selectedLevel);
    public static OnLevelChange s_OnLevelChange;

    /// <summary>
    /// m_TurretPlacement: Amount of total turret slots.
    /// m_MapSize: Size of the map(Small, Medium, Large).
    /// m_MapDifficulty: Difficulty of the map(Easy, Normal, Hard).
    /// </summary>
    [Header("General level info")]
    [SerializeField] private Text m_TurretPlacement;
    [SerializeField] private Text m_MapSize;
    [SerializeField] private Text m_MapDifficulty;

    /// <summary>
    /// Array of songs in the selected level.
    /// </summary>
    [Header("Song names")]
    [SerializeField] private Text[] m_Songs = new Text[3];

    private void OnEnable()
    {
        s_OnLevelChange += SetMapInfo;
    }

    /// <summary>
    /// Sets all UI info to the selected level's info.
    /// </summary>
    /// <param name="selectedLevel">The selected level</param>
    private void SetMapInfo(Level selectedLevel)
    {
        if(selectedLevel != null)
        {
            m_TurretPlacement.text = selectedLevel.TurretPlacements.ToString();
            m_MapSize.text = selectedLevel.MapSize.ToString();
            m_MapDifficulty.text = selectedLevel.MapDifficulty.ToString();

            for (int i = 0; i < m_Songs.Length; i++)
            {
                m_Songs[i].text = "";
                m_Songs[i].transform.parent.gameObject.SetActive(false);
            }

            for (int i = 0; i < selectedLevel.Songs.Length; i++)
            {
                m_Songs[i].transform.parent.gameObject.SetActive(true);
                m_Songs[i].text = selectedLevel.Songs[i];
            }
        }
    }

    private void OnDisable()
    {
        s_OnLevelChange -= SetMapInfo;
    }
}