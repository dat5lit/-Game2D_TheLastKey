using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    [SerializeField] private PlayerController _player;
    [SerializeField] private CameraFllow _cameraFollow;

    public CameraFllow cam => _cameraFollow;
    public PlayerController player => _player;

    [Header("Timer")]
    [SerializeField] private float _gameTime;

    public float gameTime => _gameTime;

    [Header("Coin")]
    [SerializeField] private int _coin = 150;
    public int coin => _coin;

    [Header("Level")]
    [SerializeField] private int _level = 1;
    public int level => _level;

    [Header("Revive")]
    public Vector3 deathPosition;

    #region Unity
    private void Update()
    {
        _gameTime += Time.deltaTime;
        if(Time.timeScale == 0) Observer.instance.Notify(CONSTANT.TimeUI);
       

    }

    private void Start()
    {
        SaveData();
        LoadData();
        Observer.instance.Notify(CONSTANT.UICoin);
        Observer.instance.Notify(CONSTANT.UIDamge);
    }

    #endregion

    #region Coin

    public void AddCoin(int amount)
    {
        _coin += amount;

        SaveData();

        Observer.instance.Notify(CONSTANT.UICoin);
    }

    public bool SpendCoin(int amount)
    {
        Debug.Log(_coin +" : "+ amount);
        if (_coin < amount)
            return false;

        _coin -= amount;

        SaveData();

        Observer.instance.Notify(CONSTANT.UICoin);

        return true;
    }

    #endregion

    #region Level

    public void SetLevel(int level)
    {
        _level = level;

        SaveData();

        Observer.instance.Notify(CONSTANT.UILevel);
    }

    #endregion

    #region Player

    public void RegisterPlayer(PlayerController player)
    {
        _player = player;
    }

    public void SetDeathPosition(Vector3 position)
    {
        deathPosition = position;
    }

    #endregion

    #region Save Load

    public void SaveData()
    {
        PlayerPrefs.SetInt("Coin", _coin);
        PlayerPrefs.SetInt("Level", _level);

        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        _coin = PlayerPrefs.GetInt("Coin", _coin);
        _level = PlayerPrefs.GetInt("Level", 1);
    }
    public string GetTimeString()
    {
        
        int minutes = Mathf.FloorToInt(_gameTime / 60);
        int seconds = Mathf.FloorToInt(_gameTime % 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void NewGame()
    {
        _coin = 0;
        _level = 1;

        SaveData();

        Observer.instance.Notify(CONSTANT.UICoin);
    }
    
    #endregion
}