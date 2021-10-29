using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PropSelector : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Delegate that returns the prop's file path when selected
    /// </summary>
    /// <param name="propPath">The prop's file path</param>
    public delegate void PropSelectorConfirm(string propPath);
    /// <summary>
    /// Gets called when a prop gets selected and confirmed
    /// </summary>
    public static PropSelectorConfirm s_OnPropSelectorConfirm;

    [SerializeField] private Image m_Image;
    private Sprite[] m_Props;

    /// <summary>
    /// Index of the current prop
    /// </summary>
    private int m_CurrentPropIndex;

    #endregion

    #region Monobehaviour Functions

    private void Start()
    {
        // Load all the props
        m_Props = Resources.LoadAll<Sprite>("Background/Buildings");

        // Reset the current prop index
        m_CurrentPropIndex = 0;

        // Update the selected prop
        UpdateProp();
    }

    #endregion

    #region Selection

    /// <summary>
    /// Selects the previous prop
    /// </summary>
    public void MoveLeft()
    {
        if (m_CurrentPropIndex <= 0)
            m_CurrentPropIndex = m_Props.Length - 1;
        else
            m_CurrentPropIndex--;

        UpdateProp();
    }

    /// <summary>
    /// Selects the next prop
    /// </summary>
    public void MoveRight()
    {
        if (m_CurrentPropIndex >= m_Props.Length - 1)
            m_CurrentPropIndex = 0;
        else
            m_CurrentPropIndex++;

        UpdateProp();
    }

    /// <summary>
    /// Confirm the selected prop
    /// </summary>
    public void ConfirmSelection()
    {
#if UNITY_EDITOR
        string[] fileExtensions = { ".png", ".jpg" };

        string path = AssetDatabase.GetAssetPath(m_Image.sprite);

        path = path.Remove(0, 17); // Remove "/Assets/Resources/" from path string

        for (int i = 0; i < fileExtensions.Length; i++)
        {
            if (path.Contains(fileExtensions[i]))
                path = path.Replace(fileExtensions[i], "");
        }

        if (s_OnPropSelectorConfirm != null) s_OnPropSelectorConfirm(path);
#endif
    }

    /// <summary>
    /// Updates the size of the placed prop to its native size and updates the sprite
    /// </summary>
    private void UpdateProp()
    {
        m_Image.sprite = m_Props[m_CurrentPropIndex];
        m_Image.SetNativeSize();
    }

    #endregion
}