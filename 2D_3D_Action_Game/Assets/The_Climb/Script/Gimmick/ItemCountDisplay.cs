using UnityEngine;
using TMPro;

public class ItemCountDisplay : MonoBehaviour
{
    [SerializeField] private string ItemName;
    private ItemDataBase ItemDB;
    public TextMeshProUGUI CountText;

    private int LastCount = -1;

    private void Start()
    {
        ItemDB = FindObjectOfType<ItemDataBase>();

        if (ItemDB == null)
        {
            Debug.LogError("ItemDataBaseがシーン内に見つかりません");
        }

        UpDateDisplay();
    }

    private void Update()
    {
        if (ItemDB != null && !string.IsNullOrEmpty(ItemName))
        {
            int CurrentCount = ItemDB.GetItemCount(ItemName);
            if (CurrentCount != LastCount)
            {
                LastCount = CurrentCount;
                UpDateDisplay();
            }
        }
    }

    private void UpDateDisplay()
    {
        if (CountText != null)
        {
            CountText.text = LastCount.ToString();
        }
    }

}
