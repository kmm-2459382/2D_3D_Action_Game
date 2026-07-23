using UnityEngine;

public class RockBiteCollider : MonoBehaviour
{
    public BossEnemy_RockMovement rock;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rock.OnBiteSuccess(); // プレイヤーに当たった
        }
        else
        {
            rock.OnBiteFail(); // 失敗扱い（ここは演出次第で変えてOK）
        }
    }
}