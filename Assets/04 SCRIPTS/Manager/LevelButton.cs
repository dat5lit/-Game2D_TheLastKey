using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public int levelNumber;
    public string sceneName;

    void Start()
    {
        GetComponent<Button>().interactable =
            levelNumber <= LevelManager.UnlockedLevel;
    }

    public void LoadLevel()
    {
        if (levelNumber <= LevelManager.UnlockedLevel)
        {
            AudioController.instance.PlaySound("Click");
            SceneManager.LoadScene(sceneName);
        }
    }
}