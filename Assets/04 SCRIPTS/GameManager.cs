using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private CameraFllow _cameraFollow;
    public CameraFllow cam => _cameraFollow;
    public  PlayerController player => _player;
    [Header("Score")]

    [SerializeField] private float _coin = 0f;
    [Header("Level")]
    [SerializeField] public float Level = 1f;
    public float coin => _coin;
    public void Start()
    {
        UPdateLevel();
    }

    public void UPdateLevel()
    {
        Observer.instance.Notify(CONSTANT.UILevel);
    }
    public void updateCoin(float coin)
    {
        _coin += coin;
        Observer.instance.Notify(CONSTANT.UICoin);
    }
}
