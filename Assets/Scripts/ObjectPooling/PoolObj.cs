using UnityEngine;

public class PoolObj : MonoBehaviour
{
    public ObjectPool Pool { get; set; }
    public Object GenericObj { get; set; }
    
    public void ReturnToPool()
    {
        if (Pool != null && this != null)
        {
            Pool.AddObjectToPool(this);
        }
        else if(this != null)
        {
            Destroy(gameObject);
        }
    }
}

public enum PooledSubObject
{
    Default = 0,
    GameObject = 1,
    VisualEffect = 2,
    Enemy = 3,
    Rigidbody = 4,
    TowerProjectile = 5
}