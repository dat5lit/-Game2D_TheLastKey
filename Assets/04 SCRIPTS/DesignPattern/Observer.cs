using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : Singleton<Observer>
{
    // Start is called before the first frame update
    private Dictionary<string , List<Action>> _listeners = new Dictionary<string, List<Action>> ();

    public bool AddListener(string key, Action value)
    {
        if (!_listeners.ContainsKey(key))
        {
            _listeners.Add(key, new List<Action>());
        }

        // Nếu hàm này đã tồn tại trong danh sách rồi thì xóa đi trước để tránh trùng lặp
        if (_listeners[key].Contains(value))
        {
            _listeners[key].Remove(value);
        }

        _listeners[key].Add(value);
        return true;
    }
    public void Notify(string key)
    {
        if (_listeners == null) return;

        if (!_listeners.ContainsKey(key))
        {
            Debug.LogError($"Listener {key} not exist");
            return;
        }

        foreach (Action a in _listeners[key])
        {
            try { a?.Invoke(); }
            catch (Exception e) { Debug.LogException(e); }
        }
    }
    public void RemoveListener(string key, Action value)
    {
        if (_listeners.ContainsKey(key))
        {
            _listeners[key].Remove(value);
        }
    }
}
