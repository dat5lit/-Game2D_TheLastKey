using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoopingBG : MonoBehaviour
{
    Texture _textTrue;
    [SerializeField] int _pixelPerUnit;
    PlayerController _player;
    float imgWidth = 0;
    void Start()
    {
        _textTrue =  this.GetComponent<SpriteRenderer>().sprite.texture;
        imgWidth = _textTrue.width / _pixelPerUnit;
        _player = GameManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(_player.transform.position.x - this.transform.position.x) >= imgWidth)
        {
            Vector2 pos = this.transform.position;
            pos.x = _player.transform.position.x;
            this.transform.position = pos;
        }
        
    }
}
