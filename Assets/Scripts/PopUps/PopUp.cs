using UnityEngine;

public class PopUp : MonoBehaviour
{
    public Tile LastClickedFromTile { get; private set; }

    /// <summary>
    /// Shows the pop up this is called from
    /// </summary>
    /// <param name="calledFrom">The tile thats passed to the pop up (For positioning and data reasons)</param>
    public virtual void Show(Tile calledFrom)
    {
        LastClickedFromTile = calledFrom;
        transform.position = calledFrom.transform.position;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the pop up this is called from
    /// </summary>
    public virtual void Hide()
    {
        ClearLastClickedTile();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Clears the last clicked tile (Sets it to null)
    /// </summary>
    public void ClearLastClickedTile()
    {
        LastClickedFromTile = null;
    }
}