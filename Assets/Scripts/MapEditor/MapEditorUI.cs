using UnityEngine;
using UnityEngine.UI;

public class MapEditorUI : MonoBehaviour
{
    #region Variables

    [SerializeField] private Text m_CurrentSelectedTileInfo;
    [SerializeField] private GameObject m_TypeSelector;
    [SerializeField] private GameObject m_PropSelector;
    [SerializeField] private GameObject m_TileUtilities;
    [SerializeField] private GameObject m_Reset;
    [SerializeField] private GameObject m_SaveAndLoad;
    [SerializeField] private GameObject m_Saved;
    [SerializeField] private GameObject m_CopiedToClipboard;
    [Space]
    [SerializeField] private Text m_CurrentLayerText;
    [SerializeField] private InputField m_MapNameInput;

    #endregion

    #region Monobehaviour Functions

    private void Awake()
    {
        AddListeners(true);
    }

    private void OnDestroy()
    {
        AddListeners(false);
    }

    #endregion

    #region Listeners

    /// <summary>
    /// Add or remove the Listeners
    /// </summary>
    /// <param name="add">Add the Listeners?</param>
    private void AddListeners(bool add)
    {
        if (add)
        {
            Tile.s_OnTileClicked += TileClicked;
            MapEditor.s_OnTileDataAdded += ResetMapEditorUI;
        }
        else
        {
            Tile.s_OnTileClicked -= TileClicked;
            MapEditor.s_OnTileDataAdded -= ResetMapEditorUI;
        }
    }

    #endregion

    #region Tiles

    /// <summary>
    /// Gets called when a tile gets clicked
    /// </summary>
    /// <param name="tile">The tile that was clicked</param>
    private void TileClicked(Tile tile)
    {
        // Set the tile info on the UI
        SetTileInfo(tile.name);

        // Get the data of the Tile
        TileData data = MapEditor.s_Instance.GetTileDataFromMap(tile.PositionInGrid);

        // Determine whether the Tile has data yet
        if (data != null)
        {
            // Tile has data, show utilities
            if (data.State != TileState.NOT_USABLE)
                ShowReset();
            else
                ShowTileUtilities();
        }
        else
        {
            // Tile had no data, show the selector
            ShowTypeSelector();
        }

    }

    /// <summary>
    /// Sets the Tile Info on the UI
    /// </summary>
    /// <param name="tileInfo">String that contains all the data to display on UI</param>
    private void SetTileInfo(string tileInfo)
    {
        m_CurrentSelectedTileInfo.text = tileInfo;
    }

    #endregion

    #region Visual States

    /// <summary>
    /// Reset the Map Editor UI
    /// </summary>
    public void ResetMapEditorUI()
    {
        m_TypeSelector.SetActive(false);
        m_PropSelector.SetActive(false);
        m_TileUtilities.SetActive(false);
        m_Reset.SetActive(false);
        m_SaveAndLoad.SetActive(false);
        m_CurrentSelectedTileInfo.text = "Select a Tile";
    }

    /// <summary>
    /// Shows the Type Selector
    /// </summary>
    public void ShowTypeSelector()
    {
        m_TypeSelector.SetActive(true);
        m_PropSelector.SetActive(false);
        m_TileUtilities.SetActive(false);
        m_Reset.SetActive(false);
        m_SaveAndLoad.SetActive(false);
        m_Saved.SetActive(false);
        m_CopiedToClipboard.SetActive(false);
    }

    /// <summary>
    /// Shows the Prop Selector
    /// </summary>
    public void ShowPropSelector()
    {
        m_TypeSelector.SetActive(false);
        m_PropSelector.SetActive(true);
        m_TileUtilities.SetActive(false);
        m_Reset.SetActive(false);
        m_SaveAndLoad.SetActive(false);
        m_Saved.SetActive(false);
        m_CopiedToClipboard.SetActive(false);
    }

    /// <summary>
    /// Shows the Reset Button
    /// </summary>
    public void ShowReset()
    {
        m_TypeSelector.SetActive(false);
        m_PropSelector.SetActive(false);
        m_TileUtilities.SetActive(false);
        m_Reset.SetActive(true);
        m_SaveAndLoad.SetActive(false);
        m_Saved.SetActive(false);
        m_CopiedToClipboard.SetActive(false);
    }

    /// <summary>
    /// Shows the Tile Utilities
    /// </summary>
    public void ShowTileUtilities()
    {
        m_TypeSelector.SetActive(false);
        m_PropSelector.SetActive(false);
        m_TileUtilities.SetActive(true);
        m_Reset.SetActive(false);
        m_SaveAndLoad.SetActive(false);
        m_Saved.SetActive(false);
        m_CopiedToClipboard.SetActive(false);

        m_CurrentLayerText.text = MapEditor.s_Instance.CurrentSelectedTile.GetLayer(TileVisualState.PROP).ToString();
    }

    /// <summary>
    /// Shows the Save and Load
    /// </summary>
    public void ShowSaveAndLoad()
    {
        m_TypeSelector.SetActive(false);
        m_PropSelector.SetActive(false);
        m_TileUtilities.SetActive(false);
        m_Reset.SetActive(false);
        m_SaveAndLoad.SetActive(true);
        m_Saved.SetActive(false);
        m_CopiedToClipboard.SetActive(false);
    }

    /// <summary>
    /// Shows the Saved
    /// </summary>
    public void ShowSaved()
    {
        m_TypeSelector.SetActive(false);
        m_PropSelector.SetActive(false);
        m_TileUtilities.SetActive(false);
        m_Reset.SetActive(false);
        m_SaveAndLoad.SetActive(false);
        m_Saved.SetActive(true);
        m_CopiedToClipboard.SetActive(false);
    }

    /// <summary>
    /// Shows the Copied to Clipboard
    /// </summary>
    public void ShowCopiedToClipboard()
    {
        m_TypeSelector.SetActive(false);
        m_PropSelector.SetActive(false);
        m_TileUtilities.SetActive(false);
        m_Reset.SetActive(false);
        m_SaveAndLoad.SetActive(false);
        m_Saved.SetActive(false);
        m_CopiedToClipboard.SetActive(true);
    }

    #endregion

    #region Prop Layering

    /// <summary>
    /// Sets the layer of the prop higher (1 layer)
    /// </summary>
    public void UpPropLayer()
    {
        int layer = MapEditor.s_Instance.CurrentSelectedTile.GetLayer(TileVisualState.PROP);
        layer++;
        MapEditor.s_Instance.CurrentSelectedTile.SetLayer(TileVisualState.PROP, layer);
        MapEditor.s_Instance.GetTileDataFromMap(MapEditor.s_Instance.CurrentSelectedTile.PositionInGrid).LayerIndex = layer;

        m_CurrentLayerText.text = layer.ToString();
    }

    /// <summary>
    /// Sets the layer of the prop lower (1 layer)
    /// </summary>
    public void DownPropLayer()
    {
        int layer = MapEditor.s_Instance.CurrentSelectedTile.GetLayer(TileVisualState.PROP);
        layer--;
        MapEditor.s_Instance.CurrentSelectedTile.SetLayer(TileVisualState.PROP, layer);
        MapEditor.s_Instance.GetTileDataFromMap(MapEditor.s_Instance.CurrentSelectedTile.PositionInGrid).LayerIndex = layer;

        m_CurrentLayerText.text = layer.ToString();
    }

    #endregion

    #region Save and Load

    /// <summary>
    /// Save a map with the name given in the input text field
    /// </summary>
    public void SaveMap()
    {
        MapEditor.s_Instance.SetMapName(m_MapNameInput.text);
        MapEditor.s_Instance.SaveMap();
    }

    /// <summary>
    /// Loads a map with the input given in the corresponding input text field
    /// </summary>
    public void LoadMap()
    {
        MapLoader.s_Instance.LoadMap(m_MapNameInput.text, false);
    }

    #endregion
}