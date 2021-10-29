using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour {

    public static ObjectPoolManager s_Instance;
    public List<ObjectPool> ObjectPools;
    public event System.Action OnTick;

    [SerializeField]
    private float m_TickIntervalValue;
    private WaitForSeconds m_TickInterval;
    
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        m_TickInterval = new WaitForSeconds(m_TickIntervalValue);
        Init();
        ObjectPools = new List<ObjectPool>();
        StartCoroutine(Ticker());
    }

    //make instance
    private void Init()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// This returns the objectpool that contains your prefab.
    /// If the pool does not exist yet, it will automatically create 
    /// </summary>
    /// <param name="Prefab">The prefab that will be pooled</param>
    /// <param name="PoolStartSize">the starting size of the object pool</param>
    /// <param name="IncreaseIncrement">the steps in which the pool will increase when needed</param>
    /// <param name="ManagerTicksBeforeClean">amount of ticks before clean check</param>
    /// <param name="CleanThreshold">threshold for the amount of disabled objects needed before it will be cleaned</param>
    /// <param name="DontDestroyOnLoadBool"></param>
    /// <returns></returns>
    public ObjectPool GetObjectPool(GameObject Prefab, int PoolStartSize = 20, int IncreaseIncrement = 5, int ManagerTicksBeforeClean = 5, int CleanThreshold = 20 , bool DontDestroyOnLoadBool = false, PooledSubObject generic = PooledSubObject.Default)
    {
        if(Prefab.GetComponent<PoolObj>() == null)
        {            
            Debug.LogError("POOLOBJECT COMPONENT NOT FOUND ON PREFAB");
            return null;
        }
        else
        {
            for (int i = 0; i < ObjectPools.Count; i++)
            {
                if(ObjectPools[i].ObjectPrefab == Prefab)
                {
                    return ObjectPools[i];
                }
            }

            GameObject newPoolObj = new GameObject(Prefab.name + " pool");
            if(DontDestroyOnLoadBool)
            {
                DontDestroyOnLoad(newPoolObj);
            }
            newPoolObj.AddComponent<ObjectPool>();

            ObjectPool newPool = newPoolObj.GetComponent<ObjectPool>();
            newPool.ObjectPrefab = Prefab;
            newPool.ObjectPoolManager = this;
            OnTick += newPool.OnTick;
            newPool.Init(generic,PoolStartSize,IncreaseIncrement,ManagerTicksBeforeClean,CleanThreshold);
            ObjectPools.Add(newPool);

            return newPool;
        }        
    }

    /// <summary>
    /// this contains an action that will have all onTick functions from the object pools in it
    /// </summary>
    /// <returns></returns>
    private IEnumerator Ticker()
    {
        while(true)
        {
            if (OnTick != null)
            {
                OnTick();
            }
            yield return m_TickInterval;
        }        
    }
}