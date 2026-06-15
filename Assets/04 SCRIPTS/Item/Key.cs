using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public WinUI winUI;

    private void Start()
    {
        winUI = FindFirstObjectByType<WinUI>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("YOU WIN!");

            winUI.ShowWin();

            Destroy(gameObject);
        }
    }
}