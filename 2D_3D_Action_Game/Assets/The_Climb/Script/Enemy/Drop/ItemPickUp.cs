using TMPro;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private ItemData itemData;  // ← Inspector で設定できるように
    private ItemDataBase itemDB;
    public TextMeshProUGUI CountText;

    private void Start()
    {
        // シーン内のItemDataBaseを探す（または直接アタッチしてもOK）
        itemDB = FindObjectOfType<ItemDataBase>();

        if (itemDB == null)
        {
            Debug.LogError("ItemDataBaseがシーン内に見つかりません！");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (itemDB != null && itemData != null)
            {
                itemDB.AddOrUpdateItem(itemData);
                Debug.Log($"{itemData.Name}を拾った");
                Destroy(gameObject);
                //CountText.text = itemDB.GetItemCount(itemData.Name).ToString();
            }
            else
            {
                Debug.LogWarning("ItemDBまたはItemDataが設定されていません。");
            }
        }
    }

}
