using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUI : MonoBehaviour
{
    public GameObject winPanel;

    public void ShowWin()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        GameManager.instance.cam.GetComponent<AudioSource>().Stop();
        LevelManager.UnlockNextLevel(currentLevel);

        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    // Chơi lại màn hiện tại
    public void ReplayLevel()
    {
        Time.timeScale = 1f;
        GameManager.instance.cam.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // Sang màn tiếp theo
    public void NextLevel()
    {
        Time.timeScale = 1f;
        GameManager.instance.cam.GetComponent<AudioSource>().Play();
        int nextLevel =
            SceneManager.GetActiveScene().buildIndex + 1;

        SceneManager.LoadScene(nextLevel);
    }

    // Về menu chính
    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}