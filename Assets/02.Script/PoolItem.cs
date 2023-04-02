using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolItem : MonoBehaviour
{
    private Action<PoolItem> returnFunc;

    public void SetReturnFunc(Action<PoolItem> returnFunc)
    {
        this.returnFunc = returnFunc;
    }

    private void ReturnToPool()
    {
        returnFunc?.Invoke(this);
    }

    public abstract void ResetState();
}