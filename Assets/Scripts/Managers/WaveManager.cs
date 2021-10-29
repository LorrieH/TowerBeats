using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager s_Instance;
    public int CurrentWaveIndex { get; private set; }
    private Dictionary<int, List<Enemy>> m_WaveData = new Dictionary<int, List<Enemy>>();
    public Dictionary<int, List<Enemy>> WaveData { get { return m_WaveData; } }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
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

    public void SpawnNextWave()
    {
        //Spawns the next wave
    }

    public void DespawnWave(int waveIndex)
    {
        //Destroy enemy first
        m_WaveData[waveIndex].Clear();
        m_WaveData.Remove(waveIndex);
    }
}