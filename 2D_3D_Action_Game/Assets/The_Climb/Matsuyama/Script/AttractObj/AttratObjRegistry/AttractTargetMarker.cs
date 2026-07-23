using UnityEngine;

namespace TheClimb.Astral
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class AttractTargetMarker : AttractableBase     //  万有引力影響対象につけるコンポーネント
    {
        [SerializeField] AttractTargetStatusBlock attractTargetStatusBlock;    //  万有引力操作対象のステータス

        public override AttractTargetStatusBlock statProperty => attractTargetStatusBlock;
        public Rigidbody rigidBody { get; private set ;}

        public override AttractTargetStateID currentStateIDProperty => curretStateID;
        IAttractableListener[] attractableListener;

        void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();

            attractableListener = GetComponents<IAttractableListener>();
        }
        void Initialize()    //  初期化
        {
            
        }
        void OnEnable()
        {
            AttractObjResistry.RegistTarget(this, rigidBody);    //  レジストリーに登録
        }
        
        private void OnDisable()
        {
            AttractObjResistry.UnregisterTarget(this, rigidBody);    //  レジストリーから登録解除
        }

        public override void OnAttract()    //  引き寄せがスタートした瞬間の処理
        {
            base.OnAttract();

            foreach (var listener in attractableListener)
                listener?.OnAttract();
        }
    }
}