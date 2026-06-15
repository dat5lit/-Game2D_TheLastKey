using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Prallax : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<LoopingBG> LPG = new List<LoopingBG> ();
    public float speed;
    public float _way;


    // Update is called once per frame
    void Update()
    {
        float cam =GameManager.instance.cam.transform.position.x;
        float player = GameManager.instance.player.transform.position.x;
        float distance = Mathf.Abs(player -cam);
        //Debug.Log(GameManager.instance.player.Speed);
        if (GameManager.instance.player.Speed != 0  && !(distance >= 2f) )
        {
            _way = GameManager.instance.player.transform.localScale.x < 0 ? 1 : -1;
        }
        else _way = 0;
        for(int i = 0; i < LPG.Count; i++)
        {
            LPG[i].transform.Translate(new Vector3((i +0.5f)* _way * speed * Time.deltaTime, 0, 0));
        }
        
    }
}
