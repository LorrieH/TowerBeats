using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class Sceneloader : MonoBehaviour
{   
    /// <summary>
    /// Instance of this script.
    /// </summary>
    public static Sceneloader s_Instance;

    /// <summary>
    /// Delegate which goes off when a scene is loaded.
    /// </summary>
    public delegate void OnSceneLoaded();
    public static OnSceneLoaded s_OnSceneLoaded;

    /// <summary>
    /// Boolean to check if we've already added a callback to a function.
    /// </summary>
    private static bool s_AddedCallback;

    /// <summary>
    /// Bool to check if we're loading the game.
    /// </summary>
    private bool m_Loading;

    /// <summary>
    /// Bool to check if we're currently fading.
    /// </summary>
    private bool m_Fading;

    /// <summary>
    /// Fade image.
    /// </summary>
    private Image m_Fader;

    /// <summary>
    /// Name of the level we're going to load.
    /// </summary>
    public string m_LevelToLoad;

    private void OnEnable()
    {
        if (!s_AddedCallback)
        {
            SceneManager.sceneLoaded += OnSceneLoadedCallback;
            s_AddedCallback = true;
        }
    }

    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(gameObject);

        m_Fader = GetComponentInChildren<Image>();
    }

    /// <summary>
    /// Callback which gets called when the scene is loaded.
    /// </summary>
    /// <param name="scene">Scene which gets loaded</param>
    /// <param name="mode"></param>
    private void OnSceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        m_Loading = false;
        Fade(false, 0.5f, Ease.InOutSine);
        if(scene.name == "Game")
        {
            if(s_OnSceneLoaded != null)
                s_OnSceneLoaded();

            GameManager.s_Instance.StartGame(m_LevelToLoad);
        }
    }

    /// <summary>
    /// Loads a scene by name
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    public void LoadScene(string sceneName)
    {
        if (m_Loading) return;

        m_Loading = true;
        Fade(true, 0.5f, Ease.InOutSine, () => {
            SceneManager.LoadSceneAsync(sceneName);
        });
    }

    /// <summary>
    /// Loads a scene with the designated level.
    /// </summary>
    /// <param name="level">Level to load</param>
    public void LoadGameSceneWithLevel(string level)
    {
        LoadScene("Game");
        m_LevelToLoad = level;
    }

    /// <summary>
    /// Reloads the current scene (Used when pressing the "Retry" button after failing a level
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    /// <summary>
    /// Fades the screen
    /// </summary>
    /// <param name="state">Fade in or out?</param>
    /// <param name="duration">Duration of the fading</param>
    /// <param name="easing">Easing type</param>
    /// <param name="onComplete">Callback when it's done fading</param>
    private void Fade(bool state, float duration, Ease easing, Action onComplete = null)
    {
        if (m_Fading) return;
        m_Fading = true;
        m_Fader.DOFade(Convert.ToInt32(state), duration).SetEase(easing).OnComplete(delegate { m_Fading = false; if(onComplete != null) onComplete(); });
    }
}