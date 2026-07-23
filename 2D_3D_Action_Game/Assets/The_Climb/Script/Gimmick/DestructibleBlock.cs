using Hanzzz.MeshDemolisher;
using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{

    [SerializeField] private GameObject BlockObject;    // ブロック本体(ON/OFFを切り替える対象)
    [SerializeField] private MeshDemolisherExample demolisher; //足場を壊す演出

    [SerializeField] private GameObject[] dark; // 暗闇

    private bool IsBroken = false; 

    private void Awake()
    {
        if (BlockObject == null)
        {
            BlockObject = this.gameObject;
        }
        Debug.Log("イベントが呼ばれました");
    }

    // ブロックの表示を切り替える(爆発時に呼ばれる)
    public void BreakBlock()
    {
        if (IsBroken) return;

        IsBroken = true;

        // 破壊エフェクト
        if (demolisher != null)
        {
            demolisher.RequestDemolish();
        }

        // コライダー無効化
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 暗闇があれば消す
        if (dark != null)
        {
            foreach (var d in dark)
            {
                if (d != null)
                {
                    d.SetActive(false);
                }
            }
        }

        Debug.Log("ブロックを破壊しました");
    }
}