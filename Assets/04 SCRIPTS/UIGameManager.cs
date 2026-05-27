using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameManager : Singleton<UIGameManager>
{
    // Start is called before the first frame update
    [Header("Score")]
    [SerializeField] private Text _textCoin;
    [SerializeField] private Image _HP;

    [Header("UILevel")]
    [SerializeField] private Text UILevel;

    void Start()
    {
        Observer.instance.AddListener(CONSTANT.UICoin, setCoin);
        Observer.instance.AddListener(CONSTANT.UIDamge, setHP);
        Observer.instance.AddListener(CONSTANT.UILevel ,setLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setCoin()
    {
        _textCoin.text = "Coin : " + GameManager.instance.coin.ToString();

    }
    public void setHP()
    {
        float currentHP = GameManager.instance.player.currenHP;
        float maxHp = GameManager.instance.player._HP;
        float hp = currentHP / maxHp;
        _HP.fillAmount = hp;

    }
    public void setLevel()
    {
        float level = GameManager.instance.Level;
        UILevel.text = "Level : " + level.ToString();
    }
}
