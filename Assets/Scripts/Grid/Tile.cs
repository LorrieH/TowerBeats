using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

#region Enums

/// <summary>
/// Open = buildable, Not_Usable = unusable tile, Path = Enemy AI Path, Occupied = Tile that contains tower
/// </summary>
public enum TileState
{
    OPEN,
    NOT_USABLE,
    PATH,
    OCCUPIED,
    TURRET_SPAWN,
    HEADQUARTERS,
    PROP
}

public enum TileVisualState
{
    BASE,
    PATH,
    ENEMY_SPAWN,
    PROP,
    TURRET_SPAWN,
    TURRET_SELECTED,
    HEADQUARTERS
}

#endregion

#region Serializables

[System.Serializable]
public struct TileArt
{
    public TileVisualState VisualState;
    public SpriteRenderer VisualStateRenderer;
}

[System.Serializable]
public class TileData
{
    public TileData(TileState State, int PathTileIndex = -1, Vector2Int PathTilePosition = default(Vector2Int), string FilePathToAsset = null, int LayerIndex = 0)
    {
        this.State = State;
        this.PathTileIndex = PathTileIndex;
        this.PathTilePosition = PathTilePosition;
        this.FilePathToAsset = FilePathToAsset;
        this.LayerIndex = LayerIndex;
    }

    public TileState State;
    public int PathTileIndex;
    public Vector2Int PathTilePosition;
    public string FilePathToAsset;
    public int LayerIndex;
}

#endregion

public class Tile : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// The currentstate of the tile
    /// </summary>
    public TileState CurrentState { get; set; }

    /// <summary>
    /// The tiles position in the grid
    /// </summary>
    public Vector2Int PositionInGrid { get; set; }
    public int X { get { return PositionInGrid.x; } set { PositionInGrid = new Vector2Int(value, PositionInGrid.y); } }
    public int Y { get { return PositionInGrid.y; } set { PositionInGrid = new Vector2Int(PositionInGrid.x, value); } }

    /// <summary>
    /// Bool to check if camera should move to tile on click or not
    /// </summary>
    private bool m_MoveCameraToTileOnClick = true;

    /// <summary>
    /// Visual states of a tile
    /// </summary>
    [SerializeField]private List<TileArt> m_TileArt = new List<TileArt>();

    /// <summary>
    /// Sets the clickable state on a tile
    /// </summary>
    /// <param name="state">The state to set the tile to</param>
    public delegate void SetClickableState(bool state);
    public static SetClickableState s_OnSetTileClickableState;

    /// <summary>
    /// Delegate that gets called when a clickable tile is clicked
    /// </summary>
    /// <param name="tile">Tile that gets clicked</param>
    public delegate void TileClicked(Tile tile);
    public static TileClicked s_OnTileClicked;

    /// <summary>
    /// Bool to check if tile is clickable or not
    /// </summary>
    public bool CanClick { get; set; }

    /// <summary>
    /// The tower on this tile
    /// </summary>
    public Tower Tower { get; set; }

    #endregion

    #region Monobehavior functions
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        HexGrid.s_OnSelectedTileChanged += OnSelectedTileChanged;
        s_OnSetTileClickableState += SetTileClickableState;
        CanClick = true;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        HexGrid.s_OnSelectedTileChanged -= OnSelectedTileChanged;
        s_OnSetTileClickableState -= SetTileClickableState;
    }

    /// <summary>
    /// Callback for when the scene has finished loading
    /// </summary>
    /// <param name="scene">Scene that gets loaded</param>
    /// <param name="mode">Loading mode of the scene</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CurrentState = TileState.NOT_USABLE;
        Tower = null;
    }

    /// <summary>
    /// Only counts as a button click if the click down and up are both on the same tile
    /// </summary>
    private void OnMouseUpAsButton()
    {
        OnTileClick();
    }

    #endregion

    #region Tile functions

    /// <summary>
    /// Function that gets called when a clickable tile gets clicked
    /// </summary>
    public void OnTileClick()
    {
        if (!CameraMovement.s_Instance.IsPointerOverUIObject())
        {
            if (!CanClick) return;

            if (s_OnTileClicked != null) s_OnTileClicked(this);

            if (PopUpManager.s_Instance == null) return;

            switch (CurrentState)
            {
                case TileState.TURRET_SPAWN:
                    if (m_MoveCameraToTileOnClick)
                        CameraMovement.s_Instance.ScrollCameraToPosition(this, 0.5f, true, true);
                    if (PopUpManager.s_Instance != null)
                    {
                        PopUpManager.s_Instance.ShowPopUp(PopUpNames.TOWER_SHOP_MENU, this);
                    }
                    //Open tower shop menu
                    break;
                case TileState.OCCUPIED:
                    if (m_MoveCameraToTileOnClick)
                        CameraMovement.s_Instance.ScrollCameraToPosition(this, 0.5f, true, true);
                    //Open tower menu and shows the stats of the tower on this tile
                    if (PopUpManager.s_Instance != null)
                    {
                        PopUpManager.s_Instance.ShowPopUp(PopUpNames.TOWER_MENU, this);
                    }
                    if (m_MoveCameraToTileOnClick)
                        CameraMovement.s_Instance.ScrollCameraToPosition(this, 0.5f, true, true);
                    break;
                default:
                    PopUpManager.s_Instance.HideAll();
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the tile's visual state to the given state
    /// </summary>
    /// <param name="state">State of the tile</param>
    public void SetTileVisualsState(TileVisualState state, bool visible = true, string filePath = null)
    {
        ResetTileVisuals();

        if (state == TileVisualState.BASE) return;


        SpriteRenderer renderer = GetRenderer(state);

        if (renderer == null)
        {
            Debug.LogWarning("<color=orange>[Tile]</color> Could not find renderer of state: " + state.ToString());
            return;
        }

        renderer.enabled = true;

        if (!visible)
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0);

        switch (state)
        {
            case TileVisualState.PROP:
                renderer.sprite = Resources.Load<Sprite>(filePath);
                SetLayer(TileVisualState.PROP, HexGrid.s_Instance.GridSize.y - PositionInGrid.y);
                break;
            case TileVisualState.HEADQUARTERS:
                SetLayer(TileVisualState.HEADQUARTERS, HexGrid.s_Instance.GridSize.y - PositionInGrid.y);
                break;
        }
    }

    /// <summary>
    /// Changes the selected tile and the visual state of the current and previously selected tile
    /// </summary>
    /// <param name="lastSelected">The previously selected tile</param>
    /// <param name="currentSelected">The tile that is currently selected</param>
    private void OnSelectedTileChanged(Tile lastSelected, Tile currentSelected)
    {
        if(lastSelected == this)
        {
            if(lastSelected.CurrentState == TileState.OCCUPIED)
                SetTileVisualsState(TileVisualState.TURRET_SPAWN);
        }
    }

    /// <summary>
    /// Sets the clickable state of the tile
    /// </summary>
    /// <param name="state">Bool to check if the tile should be clickable or not</param>
    private void SetTileClickableState(bool state)
    {
        CanClick = state;
    }

    /// <summary>
    /// Resets the tiles visual state
    /// </summary>
    private void ResetTileVisuals()
    {
        for (int i = 0; i < m_TileArt.Count; i++)
        {
            if(m_TileArt[i].VisualStateRenderer != null)
                m_TileArt[i].VisualStateRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Gets the tiles renderer
    /// </summary>
    /// <param name="state">The tiles visual state</param>
    /// <returns></returns>
    private SpriteRenderer GetRenderer(TileVisualState state)
    {
        return m_TileArt.Find(x => x.VisualState == state).VisualStateRenderer;
    }

    /// <summary>
    /// Animate the tile with a fade and scale
    /// </summary>
    /// <param name="state">The visual state of the tile</param>
    public void AnimateFadeScaleIn(TileVisualState state)
    {
        SpriteRenderer renderer = GetRenderer(state);

        if(renderer != null)
        {
            renderer.DOFade(1f, 0.2f);
            renderer.transform.localScale = new Vector2(1.4f, 1.4f);
            renderer.transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo);
        }
    }

    /// <summary>
    /// Animate the tile with a scale and a bounce effect
    /// </summary>
    /// <param name="state">The visual state of the tile</param>
    public void AnimateScaleBounceIn(TileVisualState state)
    {
        SpriteRenderer renderer = GetRenderer(state);

        if(renderer != null)
        {
            renderer.transform.localScale = Vector2.zero;
            renderer.DOFade(1f, 0.2f).SetEase(Ease.InQuad);
            renderer.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        }
    }

    /// <summary>
    /// Set the layer of the tile's art
    /// </summary>
    /// <param name="state">State of the tile</param>
    /// <param name="layer">Layer index</param>
    public void SetLayer(TileVisualState state, int layer)
    {
        for (int i = 0; i < m_TileArt.Count; i++)
        {
            switch (state)
            {
                case TileVisualState.PROP:
                case TileVisualState.HEADQUARTERS:
                    m_TileArt[i].VisualStateRenderer.sortingOrder = layer;
                    break;
            }
        }
    }

    /// <summary>
    /// Get the layer of the tile's art
    /// </summary>
    /// <param name="state">State of the tile</param>
    public int GetLayer(TileVisualState state)
    {
        for (int i = 0; i < m_TileArt.Count; i++)
        {
            switch (state)
            {
                case TileVisualState.PROP:
                case TileVisualState.HEADQUARTERS:
                    return m_TileArt[i].VisualStateRenderer.sortingOrder;
            }
        }
        return 0;
    }

    #endregion
}