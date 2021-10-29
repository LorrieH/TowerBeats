using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Projectile Data Struct

/// <summary>
/// Data of a TowerProjectile
/// </summary>
public struct ProjectileData
{
    /// <summary>
    /// Constructor of the ProjectileData
    /// </summary>
    /// <param name="StartPosition">Start position of the projectile</param>
    /// <param name="Target">Target of the projectile</param>
    /// <param name="MoveDuration">Move duration of the projectile</param>
    /// <param name="Damage">Damage of the projectile</param>
    public ProjectileData(Vector2 StartPosition, Enemy Target, float MoveDuration, float Damage)
    {
        this.StartPosition = StartPosition;
        this.Target = Target;
        this.MoveDuration = MoveDuration;
        this.Damage = Damage;
    }

    /// <summary>
    /// Start position of the projectile
    /// </summary>
    public Vector2 StartPosition;
    /// <summary>
    /// Target of the projectile
    /// </summary>
    public Enemy Target;
    /// <summary>
    /// Move duration of the projectile
    /// </summary>
    public float MoveDuration;
    /// <summary>
    /// Damage of the projectile
    /// </summary>
    public float Damage;
}

#endregion

public class TowerProjectile : PoolObj
{
    #region Variables

    [SerializeField] private AnimationCurve m_ProjectileArc;
    [SerializeField] private float m_ProjectileHeight = 4f;
    [SerializeField] private float m_ProjectileRotationSpeed = 8f;
    [SerializeField] private float m_OffsetY = 0.6f;

    private bool m_Pause;
    public ProjectileData Data { get; private set; }

    #endregion

    #region Add/Remove Listeners

    private void OnEnable()
    {
        PauseCheck.Pause += TogglePause;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        PauseCheck.Pause -= TogglePause;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Projectile Data

    /// <summary>
    /// Set the data of this projectile
    /// </summary>
    /// <param name="startPosition">Start position of the projectile</param>
    /// <param name="target">Target of the projectile</param>
    /// <param name="moveDuration">Move duration of the projectile</param>
    /// <param name="damage">Damage of the projectile</param>
    public void SetData(Vector2 startPosition, Enemy target, float moveDuration, float damage)
    {
        SetData(new ProjectileData(startPosition, target, moveDuration, damage));
    }

    /// <summary>
    /// Set the data of this projectile
    /// </summary>
    /// <param name="data">Data of the projectile</param>
    public void SetData(ProjectileData data)
    {
        Data = data;
        StartCoroutine(CurveMoveToTarget());
    }

    #endregion

    #region Projectile Move Behaviour

    /// <summary>
    /// Moves the projectile in a curve to the target
    /// </summary>
    private IEnumerator CurveMoveToTarget()
    {
        // Store the time
        float time = 0;

        // Move as long as the Move Duration is
        while(time < Data.MoveDuration)
        {
            // Break the coroutine and remove the projectile when the target is dead
            if (Data.Target == null)
            {
                ReturnToPool();
                yield break;
            }

            // Only move the projectile if the game isn't paused
            yield return new WaitWhile(() => m_Pause);

            // Update the time
            time += Time.deltaTime;

            // Update the end position
            Vector3 end = Data.Target.transform.position;

            // Get the time between 0 and 1
            float linearT = time / Data.MoveDuration;

            // Get the height value of the projectile arc
            float heightT = m_ProjectileArc.Evaluate(linearT);

            // Lerp the height to fake the arc
            float height = Mathf.Lerp(0, m_ProjectileHeight, heightT);

            // Lerp the projectile to the target, add the height of the arc so that it creates an actual arc
            transform.position = Vector2.Lerp(Data.StartPosition, end, linearT) + new Vector2(0f, m_OffsetY + height);

            Vector3 difference;
            // Add rotation to the projectile to create the visual effect of an arc
            if (time > (Data.MoveDuration / 2) - 0.15f)
            {
                // Get the difference between the target and the projectile
                difference = Data.Target.transform.position - transform.position;
            }
            else
            {
                // Get the difference between the projectile and the starting position
                difference = transform.position - (Vector3)Data.StartPosition;
            }

            // Get the calculated rotation based on the difference
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            // Lerp the rotation of the projectile
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rotationZ - 90), m_ProjectileRotationSpeed * Time.deltaTime);
        }
        // if the target isn't dead already, apply damage
        if (Data.Target != null)
            Data.Target.TakeDamage(Data.Damage, "Drum");

        // Return the projectile to the pool
        ReturnToPool();
    }

    #endregion

    #region Callbacks

    /// <summary>
    /// Toggle the projectile's behaviour (Pause / Unpause)
    /// Gets called when the game pauses / unpauses
    /// </summary>
    /// <param name="pause">State of the pause</param>
    public void TogglePause(bool pause)
    {
        // Set the pause state
        m_Pause = pause;
    }

    /// <summary>
    /// Gets called when a scene loades
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Return the projectile to the pool when a scene loads
        ReturnToPool();
    }

    #endregion
}
