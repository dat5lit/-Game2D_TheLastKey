using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : Singleton<ObjectPooling>
{
    // Start is called before the first frame update
    Dictionary<GameObject, List<GameObject>> OP = new Dictionary<GameObject, List<GameObject>>();
    void Start()
    {
        OP.Clear();
    }
    public GameObject GetPreFab(GameObject obj)
    {
        if(OP.ContainsKey(obj))
        {
            foreach(GameObject p in OP[obj])
            {
                if (!p.gameObject.activeSelf)
                {
                    return p;
                    
                }
            }
            GameObject g = Instantiate(obj);
            OP[obj].Add(g);
            g.gameObject.SetActive(false);
            return g;
        }
        List<GameObject> list = new List<GameObject>();
        GameObject g2 = Instantiate(obj);
        g2.gameObject.SetActive(false);
        list.Add(g2);
        OP.Add(obj,list);
        return g2;    
    }
   
}
