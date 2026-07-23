using UnityEngine;
using TheClimb.Astral;

namespace TheClimb.UniversalGravity
{
    public class GravitationTargetAttracter : MonoBehaviour    //  万有引力影響対象のオブジェクトを引き寄せるするスクリプト
    {
        [SerializeField] PlanetStatus planetStatus;         //  天体のステータス群
        GravitationStatusBlock gravitationStatusBlock;                //  天体のステータスクブロック


        float CurrentAttractRange;    //  現在の万有引力の影響半径
        float CurrentPlanetMass;      //  現在の万有引力の強さ

        void Awake()
        {
            gravitationStatusBlock = planetStatus.GetGraviatationStatus(PlanetIDs.Earth);    //  地球のステータス取得

            CurrentPlanetMass = gravitationStatusBlock.Mass;
            CurrentAttractRange = gravitationStatusBlock.AttractRange;
        }

        void FixedUpdate()
        {
            AttractTarget();    //  ターゲット引き寄せ
        }

        void AttractTarget()    //  ターゲットを引き寄せる
        {
            foreach (AttractTargetEntry targetEntry in AttractObjResistry.Entries)
            {
                Vector3 targetPosition = targetEntry.target.transform.position;
                float Distance = Vector3.Distance(this.transform.position, targetPosition);

                if (Distance < CurrentAttractRange && Distance > 0.01f)
                {
                    if (targetEntry.target.TryGetComponent<IAttractable>(out var targetData))
                    {
                        AttractTargetStatusBlock targetStatusBlock = targetData.statProperty;
                        Debug.Log(targetStatusBlock);

                        if(targetData.currentStateIDProperty != AttractTargetStateID.Attracting)
                        {
                            if(targetData == null)
                            {
                                Debug.Log("生成された");
                            }
                            targetData.OnAttract();
                        }
                        Vector3 AttractForce = AttractVectleCaluculate.CalculateAttractVectle(this.transform.position, targetPosition, CurrentPlanetMass, targetStatusBlock.Mass, Distance);
                        targetEntry.rigidbody.AddForce(AttractForce * targetStatusBlock.Mass, ForceMode.Force);
                    }
                }
            }
        }
    }
}