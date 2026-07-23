using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "Item/DropTable")]
public class DropTable : ScriptableObject
{
    public ItemData[] PossibleItems;
}
