using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameManager : Singleton<UIGameManager>
{
    // Start is called before the first frame update
    [Header("Score")]
    [SerializeField] private Text _textCoin;
    [SerializeField] private Text gold;
    [SerializeField] private Image _HP;
    [SerializeField] private Text CoinWinUI;
    [Header("Boss Message")]
    [SerializeField] private GameObject bossMessagePanel;
    [SerializeField] private Text bossMessageText;

    [Header("Time")]
    [SerializeField] private Text _textTime;

    /*[Header("UILevel")]
    [SerializeField] private Text UILevel;*/

    protected override void Awake()
    {
        Observer.instance.AddListener(CONSTANT.UICoin, setCoin);
        Observer.instance.AddListener(CONSTANT.UIDamge, setHP);
        Observer.instance.AddListener(CONSTANT.TimeUI, setTime);
        Observer.instance.AddListener(CONSTANT.BossMessage, ShowBossMessage);
    }
    public void setCoin()
    {
        _textCoin.text =   GameManager.instance.coin.ToString();
        gold.text = GameManager.instance.coin.ToString();
        CoinWinUI.text =  GameManager.instance.coin.ToString(); 

    }
    public void setHP()
    {
        float currentHP = GameManager.instance.player.currenHP;
        float maxHp = GameManager.instance.player._HP;
        float hp = currentHP / maxHp;
        _HP.fillAmount = hp;

    }
    public void setTime()
    {
        _textTime.text = "Time : " +  GameManager.instance.GetTimeString();
    }
    private void ShowBossMessage()
    {
        StartCoroutine(BossMessageRoutine(
            "Ha ha ha! Ta đang giữ chìa khóa.Muốn lấy nó thì hãy đánh bại ta!"
        ));
    }
    private IEnumerator BossMessageRoutine(string message)
    {
        bossMessagePanel.SetActive(true);

        bossMessageText.text = message;

        yield return new WaitForSeconds(4f);

        bossMessagePanel.SetActive(false);
    }
}
