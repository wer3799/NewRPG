using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private Stack<PoolItem> inPool;
    private Dictionary<int, PoolItem> outPool;

    private PoolItem prefab;
    private Transform parent;

    public ObjectPool(PoolItem prefab, Transform parent, int initialPoolSize = 0)
    {
        this.prefab = prefab;
        this.parent = parent;

        inPool = new Stack<PoolItem>();
        outPool = new Dictionary<int, PoolItem>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            MakeItem();
        }
    }

    private void MakeItem()
    {
        var initObject = Object.Instantiate<PoolItem>(prefab, parent);

        initObject.SetReturnFunc(ReturnObject);

        initObject.gameObject.SetActive(false);

        inPool.Push(initObject);
    }

    public PoolItem GetObject()
    {
        if (inPool.Count == 0)
        {
            MakeItem();
        }

        PoolItem obj = inPool.Pop();

        outPool.Add(obj.GetInstanceID(), obj);

        obj.gameObject.SetActive(true);

        return obj;
    }

    private void ReturnObject(PoolItem obj)
    {
        obj.gameObject.SetActive(false);

        inPool.Push(obj);

        outPool.Remove(obj.GetInstanceID());
    }

    public void DestroyAllItems()
    {
        foreach (PoolItem obj in inPool)
        {
            Object.Destroy(obj.gameObject);
        }

        inPool.Clear();
        inPool = null;

        foreach (var obj in outPool)
        {
            Object.Destroy(obj.Value.gameObject);
        }

        outPool.Clear();
        outPool = null;
    }
}