using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YieldPool
{

    public static YieldPool Inst
    {
        get
        {
            if (_yieldPool == null)
                _yieldPool = new YieldPool();
            return _yieldPool;
        }
    }
    private static YieldPool _yieldPool;
    
    public Dictionary<float, WaitForSeconds> WaitList;
    public WaitForEndOfFrame WaitForEndOfFrame;

    protected YieldPool()
    {
        WaitList = new Dictionary<float, WaitForSeconds>();
    }

    public WaitForSeconds GetWaitForSeconds(float duration)
    {
        if (WaitList.ContainsKey(duration))
            return WaitList[duration];
        else
        {
            WaitList.Add(duration, new WaitForSeconds(duration));
            return WaitList[duration];
        }    
    }
}
