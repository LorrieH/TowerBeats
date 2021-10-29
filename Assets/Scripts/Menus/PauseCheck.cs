using UnityEngine;

public class PauseCheck : MonoBehaviour {
    
    /// <summary>
    /// Instance of this script.
    /// </summary>
    public static PauseCheck s_Instance;

    /// <summary>
    /// Is the game paused?
    /// </summary>
    [SerializeField]
    private bool m_onPause;

    /// <summary>
    /// Event which methods can describe to for when the game gets paused.
    /// </summary>
    public static event System.Action<bool> Pause;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        SongManager.s_OnPlaylistComplete += PauseOn;
    }

    private void Init()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Unpause the game.
    /// </summary>
    public void PauseOff()
    {
        m_onPause = false;
        if (Pause != null)
        {
            Pause(m_onPause);
        }
    }

    /// <summary>
    /// Pause the game. 
    /// </summary>
    public void PauseOn()
    {
        m_onPause = true;
        if (Pause != null)
        {
            Pause(m_onPause);
        }
    }

    /// <summary>
    /// Toggle the pause state.
    /// </summary>
    public void TogglePause()
    {
        m_onPause = !m_onPause;
        if (Pause != null)
        {
            Resources.UnloadUnusedAssets();
            Pause(m_onPause);
        }
    }

    private void OnDisable()
    {
        SongManager.s_OnPlaylistComplete -= PauseOn;
    }
}