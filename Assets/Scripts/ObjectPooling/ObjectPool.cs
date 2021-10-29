using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject ObjectPrefab { get; set; }
    public ObjectPoolManager ObjectPoolManager { get; set; }

    private Queue<PoolObj> m_DeadList;
    private PooledSubObject m_objEnum;

    private int m_ObjectsInPool;
    private int m_ObjectsAliveInPool;

    private int m_CleanThreshold;
    private int m_IncreaseIncrement;
    private int m_anagerTicksBeforeClean;
    private int m_CurrentTick;

    private bool m_NotAGameObject;

    private bool m_isAboveThreshold;

    /// <summary>
    /// This fuction sets the params for the object pool that will be created.
    /// </summary>
    /// <param name="GenericObj">Choose from the pooledSubject enum what the secondary "generic" object is</param>
    /// <param name="PoolStartSize">the amount of objects the pool starts with</param>
    /// <param name="IncreaseIncrement">the amount of objects that will be added when the pool hits its current limit</param>
    /// <param name="ManagerTicksBeforeClean">the amount of ticks needed before the pool wil check for a clean</param>
    /// <param name="CleanThreshold">the amount of unused objects needed for a clean to happen example</param>
    public void Init(PooledSubObject GenericObj, int PoolStartSize = 20, int IncreaseIncrement = 5, int ManagerTicksBeforeClean = 5, int CleanThreshold = 20)
    {       
        m_ObjectsAliveInPool = 0;


        m_objEnum = GenericObj;
        PoolObj TempData = ObjectPrefab.GetComponent<PoolObj>();
        TempData.Pool = this;
        this.m_CleanThreshold = CleanThreshold;
        this.m_IncreaseIncrement = IncreaseIncrement;
        this.m_anagerTicksBeforeClean = ManagerTicksBeforeClean;

        //m_DeadList = new Queue<GameObject>();
        m_DeadList = new Queue<PoolObj>();

        for (int i = 0; i < PoolStartSize; i++)
        {
            GameObject NewPoolGameObj = Instantiate(ObjectPrefab, transform);
            PoolObj poolObj = NewPoolGameObj.GetComponent<PoolObj>();
            switch (GenericObj)
            {
                case PooledSubObject.Default:
                    poolObj.GenericObj = null;
                    break;
                case PooledSubObject.GameObject:
                    poolObj.GenericObj = poolObj.gameObject;
                    break;
                case PooledSubObject.VisualEffect:
                    poolObj.GenericObj = poolObj.gameObject.GetComponent<VisualEffect>();
                    break;
                case PooledSubObject.Enemy:
                    poolObj.GenericObj = poolObj.gameObject.GetComponent<Enemy>();
                    break;
                case PooledSubObject.Rigidbody:
                    poolObj.GenericObj = poolObj.gameObject.GetComponent<Rigidbody>();
                    break;
                case PooledSubObject.TowerProjectile:
                    poolObj.GenericObj = poolObj.gameObject.GetComponent<TowerProjectile>();
                    break;
                default:
                    //m_GenericObj = null;
                    break;
            }
            m_DeadList.Enqueue(poolObj);

            m_ObjectsInPool++;
            NewPoolGameObj.SetActive(false);
        }
    }
    /// <summary>
    /// This function returns a object from the pool and increase the pool size when needed.
    /// </summary>
    /// <returns>PoolObj</returns>
    public PoolObj GetFromPool() 
    {
        if (m_ObjectsAliveInPool < m_ObjectsInPool)
        {
            m_ObjectsAliveInPool++;
            PoolObj returnObject = m_DeadList.Dequeue();
            returnObject.gameObject.SetActive(true);
            return (returnObject);
        }
        else
        {
            for (int i = 0; i < m_IncreaseIncrement; i++)
            {
                GameObject NewPoolGameObj = Instantiate(ObjectPrefab, transform);
                PoolObj poolObj = NewPoolGameObj.GetComponent<PoolObj>();
                switch (m_objEnum)
                {
                    case PooledSubObject.Default:
                        poolObj.GenericObj = null;
                        break;
                    case PooledSubObject.GameObject:
                        poolObj.GenericObj = poolObj.gameObject;
                        break;
                    case PooledSubObject.VisualEffect:
                        poolObj.GenericObj = poolObj.gameObject.GetComponent<VisualEffect>();
                        break;
                    case PooledSubObject.Enemy:
                        poolObj.GenericObj = poolObj.gameObject.GetComponent<Enemy>();
                        break;
                    case PooledSubObject.Rigidbody:
                        poolObj.GenericObj = poolObj.gameObject.GetComponent<Rigidbody>();
                        break;
                    case PooledSubObject.TowerProjectile:
                        poolObj.GenericObj = poolObj.gameObject.GetComponent<TowerProjectile>();
                        break;
                    default:
                        //m_GenericObj = null;
                        break;
                }
                m_DeadList.Enqueue(poolObj);

                m_ObjectsInPool++;
                NewPoolGameObj.SetActive(false);
            }

            m_ObjectsAliveInPool++;
            PoolObj returnObject = m_DeadList.Dequeue();
            returnObject.gameObject.SetActive(true);
            return (returnObject);
        }
    }

    /// <summary>
    /// This contains every function that should go off every tick
    /// </summary>
    public void OnTick()
    {
        Clean();    
        m_CurrentTick++;
    }

    /// <summary>
    /// put the given object back into the pool
    /// </summary>
    /// <param name="obj">this object will be put back in the pool</param>
    public void AddObjectToPool(PoolObj obj)
    {
        obj.gameObject.SetActive(false);
        m_DeadList.Enqueue(obj);
        m_ObjectsAliveInPool--;
    }

    /// <summary>
    /// Check if cleaning is necessary and if it is it cleans up the pool. 
    /// </summary>
    private void Clean()
    {
        if (m_CurrentTick >= m_anagerTicksBeforeClean)
        {
            if (m_ObjectsInPool - m_ObjectsAliveInPool > m_CleanThreshold)
            {
                for (int i = 0; i < m_CleanThreshold; i++)
                {
                    PoolObj returnObject = m_DeadList.Dequeue();
                    Destroy(returnObject.gameObject);
                    m_ObjectsInPool--;
                }
                m_CurrentTick = m_anagerTicksBeforeClean;
            }
            else
            {
                m_CurrentTick = 0;
            }
        }
    }

    /// <summary>
    /// the pool removes itself from the pool list in the manager when it gets destroyed
    /// </summary>
    private void OnDestroy()
    {
        ObjectPoolManager.ObjectPools.Remove(this);
    }
}
