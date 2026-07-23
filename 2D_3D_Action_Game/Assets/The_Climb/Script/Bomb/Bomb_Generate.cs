using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Bomb_Generate : MonoBehaviour
{
     [SerializeField] GameObject bombPrefab; 
    [SerializeField] private Bomb_Aim p_aim; 
    public int shoot_power = 10000;

    // 投げた爆弾を保持
    private Player_Bomb currentBomb;

    void Update()
    {
        if (Time.timeScale == 0f) return;

        //爆弾発射
        if (Input.GetMouseButtonDown(0))
        {
            if (currentBomb == null)
            {
                ShootBomb();
            }
            else
            {
                // 爆弾が存在しているなら強制爆発
                currentBomb.ForceExplosion();
            }
        }
    }

    //爆弾の処理
    private void ShootBomb()
    {
        Transform spawnPos = p_aim.GetSpawnPos();     // マウス位置で左右上下決定
        Vector3 shootDir = p_aim.GetShootDirection();

        GameObject bombObj = Instantiate(bombPrefab, spawnPos.position, Quaternion.identity);

        // 生成した爆弾のPlayer_Bombを保持
        currentBomb = bombObj.GetComponent<Player_Bomb>();
        currentBomb.SetOnExplodedCallback(() =>
        {
            currentBomb = null;
        });

        Rigidbody rb = bombObj.GetComponent<Rigidbody>();
        rb.AddForce(shootDir * shoot_power);
    }
}
