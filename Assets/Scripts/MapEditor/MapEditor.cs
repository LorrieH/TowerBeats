using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#region Map Data classes

[System.Serializable]
public class Map
{
    /// <summary>
    /// Name of the Map
    /// </summary>
    public string Name;

    /// <summary>
    /// Grid size of the Map
    /// </summary>
    public Vector2Int GridSize;

    /// <summary>
    /// Data of all the tiles in the Map
    /// </summary>
    public List<TileData> TilesData = new List<TileData>();
}

[System.Serializable]
public class Maps
{
    /// <summary>
    /// List of all the maps
    /// </summary>
    public List<Map> MapsData = new List<Map>();
}

#endregion

public class MapEditor : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Instance of the MapEditor
    /// </summary>
    public static MapEditor s_Instance;

    /// <summary>
    /// Gets called when a TileData was added
    /// </summary>
    public static Action s_OnTileDataAdded;

    /// <summary>
    /// The current selected tile
    /// </summary>
    public Tile CurrentSelectedTile { get; private set; }

    /// <summary>
    /// Data class of all the maps
    /// </summary>
    private Maps m_Maps;

    /// <summary>
    /// Data class of the current map
    /// </summary>
    private Map m_Map;

    /// <summary>
    /// Counter of the current path tiles
    /// </summary>
    private int m_PathCounter;

    /// <summary>
    /// Whether the HeadQuarters is showing or not
    /// </summary>
    private bool m_ShowHeadQuarters;

    /// <summary>
    /// Path to the Maps.json file
    /// </summary>
    private string m_FilePath;

    #endregion

    #region Monobehaviour Functions

    private void Awake()
    {
        Init();
        AddListeners(true);
    }

    private void OnDestroy()
    {
        AddListeners(false);
    }

    #endregion

    #region Initialization

    private void Init()
    {
        if (s_Instance == null)
            s_Instance = this;
        else
            Destroy(gameObject);

        m_FilePath = Path.Combine(Application.dataPath, "Data/Maps.json");

        m_Maps = new Maps();
        m_Map = new Map();

        LoadData();
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
            Tile.s_OnTileClicked += TileClicked;
            PropSelector.s_OnPropSelectorConfirm += PropSelected;
        }
        else
        {
            Tile.s_OnTileClicked -= TileClicked;
            PropSelector.s_OnPropSelectorConfirm -= PropSelected;
        }
    }

    #endregion

    #region Core

    /// <summary>
    /// Callback for when a tile is clicked
    /// </summary>
    /// <param name="tile"></param>
    private void TileClicked(Tile tile)
    {
        CurrentSelectedTile = tile;
        if (m_ShowHeadQuarters)
            ToggleHeadQuarters();
    }

    /// <summary>
    /// Sets the selected tile as a prop in the MapData
    /// </summary>
    /// <param name="propPath"></param>
    private void PropSelected(string propPath)
    {
        m_Map.TilesData.Add(new TileData(TileState.PROP, -1, CurrentSelectedTile.PositionInGrid, propPath));
        CurrentSelectedTile.SetTileVisualsState(TileVisualState.PROP, true, propPath);
    }

    /// <summary>
    /// Sets the selected tile as a path tile in the Map Data
    /// </summary>
    public void SelectPath()
    {
        int pathCount = CurrentPathCount();
        m_Map.TilesData.Add(new TileData(TileState.PATH, (pathCount > 0 ? pathCount : 0), CurrentSelectedTile.PositionInGrid));

        if (pathCount <= 0)
            CurrentSelectedTile.SetTileVisualsState(TileVisualState.ENEMY_SPAWN);
        else
            CurrentSelectedTile.SetTileVisualsState(TileVisualState.PATH);

        TileDataAdded();
    }

    /// <summary>
    /// Sets the selected tile as a turret spawn in the Map Data
    /// </summary>
    public void SelectTurretSpawn()
    {
        m_Map.TilesData.Add(new TileData(TileState.TURRET_SPAWN, -1, CurrentSelectedTile.PositionInGrid));
        CurrentSelectedTile.SetTileVisualsState(TileVisualState.TURRET_SPAWN);

        TileDataAdded();
    }

    /// <summary>
    /// Toggle the HeadQuarters visuals (For visualisation)
    /// </summary>
    public void ToggleHeadQuarters()
    {
        m_ShowHeadQuarters = !m_ShowHeadQuarters;

        print(m_ShowHeadQuarters);
        int counter = 0;
        for (int i = m_Map.TilesData.Count - 1; i >= 0; i--)
        {
            print(m_Map.TilesData[i].State);
            if(m_Map.TilesData[i].State == TileState.PATH)
            {
                if (counter < 2)
                {
                        if (counter <= 0)
                            HexGrid.s_Instance.GetTile(m_Map.TilesData[i].PathTilePosition).SetTileVisualsState(( m_ShowHeadQuarters ? TileVisualState.HEADQUARTERS : TileVisualState.PATH));
                        else
                            HexGrid.s_Instance.GetTile(m_Map.TilesData[i].PathTilePosition).SetTileVisualsState((m_ShowHeadQuarters ? TileVisualState.BASE : TileVisualState.PATH));

                    counter++;
                }
                else
                    return;
            }
        }
       
        TileDataAdded();
    }

    /// <summary>
    /// Loads the MapData
    /// </summary>
    public void LoadMapData()
    {
        m_Map = MapLoader.s_Instance.MapData;
    }

    /// <summary>
    /// Callback when a Tile Data was added to the Map
    /// </summary>
    private void TileDataAdded()
    {
        CurrentSelectedTile = null;
        if (s_OnTileDataAdded != null) s_OnTileDataAdded();
    }

    /// <summary>
    /// Sets the Map name
    /// </summary>
    /// <param name="name">Name of the map</param>
    public void SetMapName(string name)
    {
        m_Map.Name = name;
    }

    /// <summary>
    /// Resets the current selected Tile
    /// </summary>
    public void ResetTile()
    {
        TileData currentTileData = GetTileDataFromMap(CurrentSelectedTile.PositionInGrid);

        if(currentTileData.State == TileState.PATH)
        {
            for (int i = m_Map.TilesData.Count - 1; i >= 0; i--)
            {
                if (m_Map.TilesData[i].State == TileState.PATH)
                {
                    if (m_Map.TilesData[i].PathTileIndex >= currentTileData.PathTileIndex)
                    {
                        ResetTileVisual(m_Map.TilesData[i]);
                        RemoveTile(m_Map.TilesData[i]);
                    }
                }
            }
        }
        else
        {
            ResetTileVisual(currentTileData);
            RemoveTile(currentTileData);
        }

    }

    /// <summary>
    /// Removes a tile from the MapDat
    /// </summary>
    /// <param name="tile">Tile that needs to be removed</param>
    private void RemoveTile(TileData tile)
    {
        m_Map.TilesData.Remove(tile);
    }

    /// <summary>
    /// Resets the tile's visuals
    /// </summary>
    /// <param name="tile">Tile that needs to be reset</param>
    private void ResetTileVisual(TileData tile)
    {
        HexGrid.s_Instance.GetTile(tile.PathTilePosition).SetTileVisualsState(TileVisualState.BASE);
    }

    /// <summary>
    /// Gets the tiles in a map and their data / state
    /// </summary>
    /// <param name="tilePosition">The position in the grid of the tile that needs to be checked</param>
    /// <returns></returns>
    public TileData GetTileDataFromMap(Vector2Int tilePosition)
    {
        for (int i = 0; i < m_Map.TilesData.Count; i++)
        {
            if (m_Map.TilesData[i].PathTilePosition == tilePosition)
                return m_Map.TilesData[i];
        }
        return null;
    }

    /// <summary>
    /// Save a created map
    /// </summary>
    public void SaveMap()
    {
        if(!MapNameAlreadyExists(m_Map.Name))
        {
            m_Map.GridSize = HexGrid.s_Instance.GridSize;
            m_Maps.MapsData.Add(m_Map);
            SaveData();
            print("Map saved");
        }
        else
        {
            print("Could not save map, map name already exists");
        }
    }

    #endregion

    #region Utils

    /// <summary>
    /// Clears the Editor
    /// </summary>
    public void ClearEditor()
    {
        m_Map = new Map();
        HexGrid.s_Instance.ClearGrid();
    }

    /// <summary>
    /// Returns the current path count
    /// </summary>
    /// <returns>The current path count</returns>
    private int CurrentPathCount()
    {
        int counter = 0;
        for (int i = 0; i < m_Map.TilesData.Count; i++)
        {
            if (m_Map.TilesData[i].PathTileIndex > -1)
                counter++;
        }

        return counter;
    }

    /// <summary>
    /// Prints the Path
    /// </summary>
    public void PrintPath()
    {
        for (int i = 0; i < m_Map.TilesData.Count; i++)
        {
            if (m_Map.TilesData[i].State == TileState.PATH)
            {
                print("Tile index in pathdata = " + m_Map.TilesData[i].PathTileIndex + " Position in grid = " + m_Map.TilesData[i].PathTilePosition);
            }
        }
    }

    /// <summary>
    /// Checks if the map name already exists
    /// </summary>
    /// <param name="mapName">The map name to check</param>
    /// <returns></returns>
    private bool MapNameAlreadyExists(string mapName)
    {
        for (int i = 0; i < m_Maps.MapsData.Count; i++)
        {
            if (m_Maps.MapsData[i].Name.ToUpper() == mapName.ToUpper())
                return true;
        }
        return false;
    }

    /// <summary>
    /// Copies the string from the parameter to the clipboard
    /// </summary>
    /// <param name="s">String that needs to be copied</param>
    private void CopyToClipboard(string s)
    {
        TextEditor te = new TextEditor
        {
            text = s
        };

        te.SelectAll();
        te.Copy();
    }

    /// <summary>
    /// Copies the map to the clipboard
    /// </summary>
    public void CopyMapToClipboard()
    {
        CopyToClipboard(JsonUtility.ToJson(m_Map, true));
    }

    /// <summary>
    /// Returns to the main menu
    /// </summary>
    public void BackToMainMenu()
    {
        Sceneloader.s_Instance.LoadScene("MainMenu");
    }

    #endregion

    #region Save And Load Data

    /// <summary>
    /// Loads all the data
    /// </summary>
    public void LoadData()
    {
#if UNITY_EDITOR
        TryCreateFile();
#endif  

        string jsonString = File.ReadAllText(m_FilePath);
        JsonUtility.FromJsonOverwrite(jsonString, m_Maps);
    }

    /// <summary>
    /// Saves all the data
    /// </summary>
    public void SaveData()
    {
        TryCreateFile();

        string jsonString = JsonUtility.ToJson(m_Maps, true);
        File.WriteAllText(m_FilePath, jsonString);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    /// <summary>
    /// Tries to create a .JSON file
    /// </summary>
    private void TryCreateFile()
    {
        if (!File.Exists(m_FilePath))
            File.Create(m_FilePath).Dispose();
    }

    #endregion
}