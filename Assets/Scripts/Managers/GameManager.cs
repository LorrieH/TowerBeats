using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

public class GameManager : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Instance of the GameManager
    /// </summary>
    public static GameManager s_Instance;

    /// <summary>
    /// Preparation time before the game starts (Time to place towers)
    /// </summary>
    private int m_PreparationTime = 5;

    /// <summary>
    /// Delegate of the preparation time (used to let others know about the time left)
    /// </summary>
    /// <param name="time">Time left</param>
    public delegate void PreparationTimeUpdated(int time);
    /// <summary>
    /// Gets called when the preparation time updates
    /// </summary>
    public static PreparationTimeUpdated s_OnPreparationTimeUpdated;

    /// <summary>
    /// Game Action Delegate
    /// </summary>
    public delegate void GameAction();
    /// <summary>
    /// Gets called when the game starts
    /// </summary>
    public static GameAction s_OnGameStart;
    /// <summary>
    /// Gets called when the game stops
    /// </summary>
    public static GameAction s_OnGameStop;

    /// <summary>
    /// Level that was loaded
    /// </summary>
    public string LoadedLevel { get; set; }

    /// <summary>
    /// Used to determine whether the tutorial should be shown
    /// </summary>
    private readonly string m_TutorialKey = "TUTORIAL";

    #endregion

    #region Monobehaviour Functions

    /// <summary>
    /// Gets called upon Awaking the Game Object
    /// </summary>
    private void Awake()
    {
        // Initialize the Instance
        InitializeInstance();

        // Set the targetFrameRate to 60
        Application.targetFrameRate = 60;
    }
    
    /// <summary>
    /// Initializes the Instance
    /// </summary>
    private void InitializeInstance()
    {
        if (s_Instance == null)
            s_Instance = this;
        else
            Destroy(gameObject);
    }

    #endregion

    #region Core

    /// <summary>
    /// Starts the game
    /// </summary>
    /// <param name="mapName">Name of the map that needs to be loaded</param>
    /// <param name="animateMap">Does the map need to be animated?</param>
    public void StartGame(string mapName, bool animateMap = true)
    {
        // Set all tiles' clickable state inactive
        Tile.s_OnSetTileClickableState(false);

        // Load the map
        MapLoader.s_Instance.LoadMap(mapName, animateMap);

        // Determine whether the game needs to be started with the tutorial
        bool startGameWithTutorial = (mapName.ToUpper() == m_TutorialKey.ToUpper() ? true : false);

        // Start the Start Game Sequence
        StartCoroutine(StartGameSequence(startGameWithTutorial));
    }

    /// <summary>
    /// Starts a Game Sequence
    /// </summary>
    /// <param name="showTutorial">Show the tutorial?</param>
    private IEnumerator StartGameSequence(bool showTutorial = false)
    {
        // Wait for the HexGrid's Instance to be available
        yield return new WaitUntil(() => HexGrid.s_Instance != null);
        // Wait For the Grid to be created
        yield return new WaitUntil(() => HexGrid.s_Instance.GridCreated == true);

        //Move and zoom teh camera to the middlepoint of the map
        CameraMovement.s_Instance.ScrollCameraToPosition(HexGrid.s_Instance.GetMiddlepointTile(), 0f, false);
        CameraMovement.s_Instance.ZoomAnimated(1, 0, DG.Tweening.Ease.Linear);

        // Wait for the MapLoader's Instance to be available
        yield return new WaitUntil(() => MapLoader.s_Instance != null);
        // Wait for the Map to be fully loaded
        yield return new WaitUntil(() => MapLoader.s_Instance.MapLoaded);

        // Show tutorial
        if(showTutorial)
        {
            bool hasShownTutorial = false;

            StartCoroutine(ShowTutorial(() => hasShownTutorial = true ));
            yield return new WaitUntil(() =>  hasShownTutorial == true );
        }

        // Enable the Tile's interactable state
        Tile.s_OnSetTileClickableState(true);

        // Allow the player to freely move the camera
        CameraMovement.s_Instance.CanMoveCamera = true;

        // Start the preparation timer
        StartCoroutine(StartPreparationTime(() => {
            // Start the game
            if (s_OnGameStart != null) s_OnGameStart();
            // Spawn continuous waves
            SpawnContinuousWaves();
        }));
    }


    /// <summary>
    /// Shows the countdown till enemies start spawning
    /// </summary>
    /// <param name="callback">Callback that gets called after the timer has finished</param>
    private IEnumerator StartPreparationTime(Action callback = null)
    {
        for (int i = m_PreparationTime; i >= 0; i--)
        {
            if (i == 0)
            {
                StartCoroutine(ShowEnemiesIncoming(2.5f));
            }
            else
            {
                if (s_OnPreparationTimeUpdated != null)
                    s_OnPreparationTimeUpdated(i);
            }

            yield return new WaitForSeconds(1);
        }
        if (callback != null)
            callback();
    }

    /// <summary>
    /// Shows a small notification of Don Diablo saying that enemies are incoming
    /// </summary>
    /// <param name="showDuration">Duration of the notification</param>
    private IEnumerator ShowEnemiesIncoming(float showDuration)
    {
        // Show the Don Diablo Headshot
        NotificationManager.s_Instance.ShowDon();

        // Enqueue a new notification
        NotificationManager.s_Instance.EnqueueNotification("Enemies Incoming!", showDuration);

        // Wait for the notification to finish
        yield return new WaitForSeconds(showDuration);

        // Hide the Don Diablo Headshot
        NotificationManager.s_Instance.HideDon();
    }

    #endregion

    #region Tutorial

    /// <summary>
    /// Shows the player the basics of the game at the start of the tutorial level
    /// </summary>
    /// <param name="callback">A callback for what happens when the tutorial is finished, null by default</param>
    /// <returns></returns>
    private IEnumerator ShowTutorial(System.Action callback = null)
    {
        //Base tutorial
        yield return new WaitForSeconds(0.5f);
        CameraMovement.s_Instance.ZoomAnimated(0, 1, DG.Tweening.Ease.InOutQuad);
        NotificationManager.s_Instance.ShowDon();
        NotificationManager.s_Instance.EnqueueNotification("Your objective is to defend this tower in Hexagonia.", 3);
        CameraMovement.s_Instance.ScrollCameraToPosition(MapLoader.s_Instance.HeadQuarters, 1, false);
        yield return new WaitForSeconds(3f);
        NotificationManager.s_Instance.HideDon();
        MapLoader.s_Instance.HeadQuarters.transform.DOScale(1.1f, 0.3f).SetLoops(4, LoopType.Yoyo);
        yield return new WaitForSeconds(2f);

        //Enemy spawning tutorial
        NotificationManager.s_Instance.ShowDon();
        NotificationManager.s_Instance.EnqueueNotification("Enemies will spawn from this red tile.", 3);
        CameraMovement.s_Instance.ScrollCameraToPosition(MapLoader.s_Instance.Path[0], 1, false);
        yield return new WaitForSeconds(3f);
        NotificationManager.s_Instance.HideDon();
        yield return new WaitForSeconds(0.5f);
        MapLoader.s_Instance.Path[0].transform.DOScale(1.1f, 0.3f).SetLoops(4, LoopType.Yoyo);
        yield return new WaitForSeconds(2f);

        //Tower tutorial
        NotificationManager.s_Instance.ShowDon();
        NotificationManager.s_Instance.EnqueueNotification("You can place towers by clicking on a white tile.", 3);
        CameraMovement.s_Instance.ScrollCameraToPosition(HexGrid.s_Instance.GetTurretSpawnpoints()[0], 1, false);
        yield return new WaitForSeconds(3f);
        NotificationManager.s_Instance.HideDon();
        yield return new WaitForSeconds(0.5f);
        HexGrid.s_Instance.GetTurretSpawnpoints()[0].transform.DOScale(1.1f, 0.3f).SetLoops(4, LoopType.Yoyo);
        yield return new WaitForSeconds(2f);
        NotificationManager.s_Instance.ShowDon();
        NotificationManager.s_Instance.EnqueueNotification("Clicking on the tower you bought will open a menu that will allow you to upgrade or sell the tower.", 6f);
        yield return new WaitForSeconds(6f);

        //Beat meter tutorial
        NotificationManager.s_Instance.EnqueueNotification("Towers react to the music, each tower responds to a different sound.", 5);
        yield return new WaitForSeconds(5f);
        NotificationManager.s_Instance.EnqueueNotification("In the top left corner you can see which tower reacts to which sound.", 5);
        yield return new WaitForSeconds(5.5f);
        NotificationManager.s_Instance.HideDon();
        if (RMSSliders.s_HighlightSlider != null)
        {
            RMSSliders.s_HighlightSlider();
        }
        yield return new WaitForSeconds(2f);
        NotificationManager.s_Instance.ShowDon();
        NotificationManager.s_Instance.EnqueueNotification("When the value of a sound reaches a threshold (white stripe in the bar) the matching towers will start shooting.", 6f);
        yield return new WaitForSeconds(6f);
        NotificationManager.s_Instance.HideDon();

        //Finish tutorial
        CameraMovement.s_Instance.CanMoveCamera = true;
        if (callback != null) callback();
    }

    #endregion

    #region Enemy Spawning

    /// <summary>
    /// Spawn continious waves of enemies
    /// </summary>
    private void SpawnContinuousWaves()
    {
        EnemySpawner.s_Instance.SpawnWave(6, 1.5f, () => { SpawnContinuousWaves(); });
    }

    #endregion
}