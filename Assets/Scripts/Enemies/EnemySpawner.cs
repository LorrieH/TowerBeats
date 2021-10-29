using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    /// <summary>
    /// Instance of the EnemySpawner
    /// </summary>
    public static EnemySpawner s_Instance;
    
    /// <summary>
    /// prefab object lists
    /// </summary>
    [SerializeField] private List<GameObject> m_EnemyPrefabs;
    [SerializeField] private List<GameObject> m_Bosses;

    /// <summary>
    /// List of spawned enemies
    /// </summary>
    public List<Enemy> SpawnedEnemies = new List<Enemy>();
    
    /// <summary>
    /// Is the EnemySpawner paused?
    /// </summary>
    private bool m_Paused;

    /// <summary>
    /// Spawning coroutine
    /// </summary>
    private Coroutine m_SpawnEnemies;
    

    /// <summary>
    /// the object pools
    /// </summary>
    public List<ObjectPool> EnemyPools = new List<ObjectPool>();
    public List<ObjectPool> BossPools = new List<ObjectPool>();
    
    /// <summary>
    /// the 
    /// </summary>
    [SerializeField] private ParticleSystem m_SpawnerParticles;


    /// <summary>
    /// wave counters
    /// </summary>
    private int m_BossWaveCounter = 0;
    private int m_WaveCounter = 0;

    private void Awake()
    {
        if(s_Instance == null)
        {
            s_Instance = this;
        }

        Enemy.s_OnDestroyEnemy += RemoveEnemyFromList;
        GameManager.s_OnGameStop += StopEnemySpawning;
        PauseCheck.Pause += TogglePause;
        MapLoader.s_OnMapLoaded += OnMapLoaded;
    }

    private void Start()
    {
        for (int i = 0; i < m_EnemyPrefabs.Count; i++)
        {
            EnemyPools.Add(ObjectPoolManager.s_Instance.GetObjectPool(m_EnemyPrefabs[i].gameObject, 5, 5, int.MaxValue, int.MaxValue, false, PooledSubObject.Enemy));
        }
        for (int i = 0; i < m_Bosses.Count; i++)
        {
            BossPools.Add(ObjectPoolManager.s_Instance.GetObjectPool(m_Bosses[i].gameObject, 5, 5, int.MaxValue, int.MaxValue, false, PooledSubObject.Enemy));
        }
    }

    /// <summary>
    /// Callback for when the map has been loaded
    /// </summary>
    private void OnMapLoaded()
    {
        m_SpawnerParticles.gameObject.transform.position = MapLoader.s_Instance.Path[0].transform.position;
        MapLoader.s_OnMapLoaded -= OnMapLoaded;
    }

    /// <summary>
    /// Spawns a random enemy
    /// </summary>
    /// <returns></returns>
    public void SpawnEnemy(bool isBoss)
    {
        Enemy newEnemy = null;
        if (!isBoss)
        {
            newEnemy = EnemyPools[UnityEngine.Random.Range(0, EnemyPools.Count)].GetFromPool().GenericObj as Enemy;
            if (m_WaveCounter > 5)
            {
                newEnemy.SetMaxHealth((int)(20 + ((m_WaveCounter - 5) * (3+ m_WaveCounter / 20)) +(((m_WaveCounter-5)*(m_WaveCounter - 5)) /30)));
            }
        }
        else
        {
            newEnemy = BossPools[UnityEngine.Random.Range(0, BossPools.Count)].GetFromPool().GenericObj as Enemy;
            if (m_WaveCounter > 5)
            {
                newEnemy.SetMaxHealth((int)(40 + ((m_WaveCounter - 5) * (8 + m_WaveCounter/20)) + (((m_WaveCounter - 5 )*( m_WaveCounter - 5)) / 15)));
            }
        }
        newEnemy.RestoreHealth();
        newEnemy.IsAlive = true;
        newEnemy.SkeletonAnims.AnimationState.SetAnimation(0, newEnemy.EnemyString + "MOVE", true);

        SpawnedEnemies.Add(newEnemy);
              
        newEnemy.transform.position = MapLoader.s_Instance.Path[0].transform.position;

        EffectsManager.s_Instance.SpawnEffect(EffectType.ENEMY_SPAWN, false, MapLoader.s_Instance.Path[0].transform.position);

        newEnemy.Move(MapLoader.s_Instance.Path[0].transform.position);
    }

    /// <summary>
    /// function that is added to the togglepouse action
    /// </summary>
    /// <param name="Pause">turn pouse on or off</param>
    private void TogglePause(bool Pause)
    {
        m_Paused = Pause;
    }
    /// <summary>
    /// spawn a new wave
    /// </summary>
    /// <param name="amountOfEnemies">the number of enemies to spawn</param>
    /// <param name="interval">the interval between spawns</param>
    /// <param name="callback">the callback function</param>
    public void SpawnWave(int amountOfEnemies, float interval, Action callback = null)
    {
        if (m_BossWaveCounter < 3)
        {
            m_SpawnEnemies = StartCoroutine(SpawnEnemies(amountOfEnemies, interval, callback));
            m_BossWaveCounter++;
            m_WaveCounter++;
        }
        else if(m_BossWaveCounter >= 3)
        {
            m_SpawnEnemies = StartCoroutine(SpawnBossWave(amountOfEnemies, interval, callback));
            m_BossWaveCounter = 0; //resets wave count
            m_WaveCounter++;
        }
        else
        {
            //Spawn no wave, just in case
        }
    }

    /// <summary>
    /// the coroutine that times the spawning of the enemies
    /// </summary>
    /// <param name="amountOfEnemies">the number of enemies to spawn</param>
    /// <param name="interval">the interval between spawns</param>
    /// <returns></returns>
    private IEnumerator SpawnEnemies(int amountOfEnemies, float interval, Action callback = null)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < amountOfEnemies; i++)
        {
            SpawnEnemy(false);

            float timer = 0;
            while (timer <= interval)
            {
                if (!m_Paused)
                {
                    timer += Time.deltaTime;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        if(callback != null)
            callback();
    }

    /// <summary>
    /// Spawns a boss wave
    /// </summary>
    /// <param name="amountOfEnemies">Amount of enemies</param>
    /// <param name="interval">Interval between spawning</param>
    /// <param name="callback">OnComplete callback</param>
    /// <returns></returns>
    private IEnumerator SpawnBossWave(int amountOfEnemies, float interval, Action callback = null)
    {
        yield return new WaitForSeconds(2);
        for (int i = 0; i < amountOfEnemies; i++)
        {
            SpawnEnemy(false);
            yield return new WaitForSeconds(interval);
        }
        yield return new WaitForSeconds(interval);
        SpawnEnemy(true);

        if(callback != null)
        {
            callback();
        }

    }

    /// <summary>
    /// remove enemy from the spawned enemies list
    /// </summary>
    /// <param name="enemy"></param>
    void RemoveEnemyFromList(Enemy enemy)
    {
        SpawnedEnemies.Remove(enemy);
    }

    /// <summary>
    /// stops the spawning coroutine
    /// </summary>
    void StopEnemySpawning()
    {
        StopCoroutine(m_SpawnEnemies);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
        GameManager.s_OnGameStop -= StopEnemySpawning;
        Enemy.s_OnDestroyEnemy -= RemoveEnemyFromList;
    }
}