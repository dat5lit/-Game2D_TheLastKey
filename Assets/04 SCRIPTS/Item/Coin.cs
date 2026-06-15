using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.AddCoin(coinValue);
            AudioController.instance.PlaySound("coin_2");

            Destroy(gameObject);
        }
    }
}