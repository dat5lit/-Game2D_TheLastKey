using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T instance => _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        if (this.GetInstanceID() != _instance.GetInstanceID()) { Destroy(this.gameObject); }
    }
}
