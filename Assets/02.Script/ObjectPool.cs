using System.Collections.Generic;
using Spine;
using UnityEngine;

public class ObjectPool<T> where T : PoolItem
{
    private Stack<T> inPool;
    private Dictionary<int, T> outPool;

    private T prefab;
    private Transform parent;

    public ObjectPool(T prefab, Transform parent, int initialPoolSize = 0)
    {
        this.prefab = prefab;
        this.parent = parent;

        inPool = new Stack<T>();
        outPool = new Dictionary<int, T>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            MakeItem();
        }
    }

    private void MakeItem()
    {
        var initObject = Object.Instantiate<T>(prefab, parent);

        initObject.SetReturnFunc(ReturnObject);

        initObject.gameObject.SetActive(false);

        inPool.Push(initObject);
    }

    public T GetItem()
    {
        if (inPool.Count == 0)
        {
            MakeItem();
        }

        T obj = inPool.Pop();

        if (outPool.ContainsKey(obj.GetInstanceID()))
        {
            
        }
        
        outPool.Add(obj.GetInstanceID(), obj);

        obj.gameObject.SetActive(true);

        return obj;
    }

    private void ReturnObject(PoolItem obj)
    {
        obj.gameObject.SetActive(false);

        inPool.Push(obj as T);

        outPool.Remove(obj.GetInstanceID());
    }

    public void DestroyAllItems()
    {
        foreach (T obj in inPool)
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