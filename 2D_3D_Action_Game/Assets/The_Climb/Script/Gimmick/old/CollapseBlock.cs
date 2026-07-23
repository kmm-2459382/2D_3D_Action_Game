using Hanzzz.MeshDemolisher;
using UnityEngine;

public class CollapseBlock : MonoBehaviour
{
    public float CollapseDelay = 1f;   //崩れるまでの遅延時間
    public float RespawnDelay  = 3f;　 //リスポーンするまでの時間

    private bool IsCollapsing = false; //崩れる処理が進行中か判定
    private bool IsCollapsed  = false; //崩れたかどうかの判定
    private float CollapseTimer;       //崩れるまでのタイマー
    private float RespawnTimer;        //リスポーンまでのタイマー

 
    [SerializeField] private Collider BlockCollider;   //ブロックのコライダー
    [SerializeField] private Collider TriggerCollider; //当たり判定用コライダー

    [SerializeField] private MeshDemolisherExample demolisher; //足場を壊す演出

    void Start()
    {
        CollapseTimer = CollapseDelay;                 //タイマーを初期化
    }

    void Update()
    {
        //プレイヤーが乗った時、崩れるまでのタイマーを進める
        if(IsCollapsing)
        {
            Debug.Log("isCollapse内");
            CollapseTimer -= Time.deltaTime;
            if(CollapseTimer <= 0)
            {
                Collapse();
            }
        }


        if (IsCollapsed)
        {
            Debug.Log("非アクティブ中のリスポーン処理");
            RespawnTimer -= Time.deltaTime;
            if (RespawnTimer <= 0)
            {
                Respawn();
            }
        }       
    }

    //プレイヤーが上に乗った時に崩れる処理を進行
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || IsCollapsing) return;

        if (other.transform.position.y > transform.position.y - 0.4f)
        {
            IsCollapsing = true;
        }
    }

    //足場を崩す処理
    void Collapse()
    {
        //コライダーを無効化
        if (BlockCollider != null)
        {
            BlockCollider.enabled = false;
        }
        if (TriggerCollider != null)
        {
            TriggerCollider.enabled = false;
        }

        //破壊を開始する
        if (demolisher != null)
            demolisher.RequestDemolish();

        
        CollapseTimer = CollapseDelay;
        IsCollapsing = false;
        IsCollapsed = true;
        RespawnTimer = RespawnDelay;
    }

    //足場を元に戻す処理
    void Respawn()
    {
        if (BlockCollider != null)
        {
            BlockCollider.enabled = true;
        }
        if (TriggerCollider != null)
        {
            TriggerCollider.enabled = true;
        }

        //壊れるブロックの生成
        if (demolisher != null)
        {
            demolisher.DemolishAsync();
            demolisher.Reset();
            IsCollapsed = false;
        }
    }
}
