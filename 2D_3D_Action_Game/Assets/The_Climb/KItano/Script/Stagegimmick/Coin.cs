using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return; // ← これで二重取得を完全防止

        if (other.CompareTag("Player"))
        {
            collected = true;

            CoinGameManager.Instance.AddCoin();

            gameObject.SetActive(false);
        }
    }
}
