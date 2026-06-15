using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject deathPanel;

   
    [SerializeField] private int reviveCost = 50;

    private void Start()
    {
        
    }

    public void ShowDeathPanel()
    {
        deathPanel.SetActive(true);
        GameManager.instance.cam.GetComponent<AudioSource>().Stop();
        Time.timeScale = 0f;
    }

    public void HideDeathPanel()
    {
        deathPanel.SetActive(false);
       
        Time.timeScale = 1f;
    }

    public void Revive()
    {
        if (!GameManager.instance.SpendCoin(reviveCost))
        {
            Debug.Log("Not enough coin");
            return;
        }
        GameManager.instance.cam.GetComponent<AudioSource>().Play();

        Time.timeScale = 1f;

        deathPanel.SetActive(false);

        GameManager.instance.player.RevivePlayer();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GameManager.instance.cam.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}