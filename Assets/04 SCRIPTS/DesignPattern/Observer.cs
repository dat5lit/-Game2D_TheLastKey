using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : Singleton<Observer>
{
    // Start is called before the first frame update
    private Dictionary<string , List<Action>> _listeners = new Dictionary<string, List<Action>> ();

    public bool AddListener(string key,  Action value)
    {
        List<Action> actions =  new List<Action> ();
        if(_listeners.ContainsKey(key))
        {
            actions = _listeners[key];
        }
        else _listeners.TryAdd(key, actions);
        _listeners[key].Add(value);
        return true;

    }
    public void Notify(string key)
    {
        if (_listeners.ContainsKey(key))
        {
           foreach(Action a in _listeners[key])
           {
                try
                {
                    a?.Invoke();
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
           }
           return;

        }
        Debug.LogErrorFormat("Listener {0} not exist" , key);
        
    }
        
}
