using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    [SerializeField] private float HP = 10f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject.CompareTag(CONSTANT.PlayerTAG))
        {
            GameManager.instance.player.HeartPicKup(HP);
            Destroy(this.gameObject);
        }
    }
}
