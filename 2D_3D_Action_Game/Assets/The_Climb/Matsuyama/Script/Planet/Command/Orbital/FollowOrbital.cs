using System.Collections;
using UnityEngine;
using TheClimb.Core;
using TheClimb.Logging;

namespace TheClimb.Astral
{
    public class FollowOrbital : PlanetCommadBase    //  PlanetControllerから呼ばれる、天体をマウス位置に追従させるコマンド
    {
        OrbitalContext _context;        //  コンテキスト、Bootstrapから渡される
        
        Coroutine orbitalFollowLoop;    //  天体が軌道上を動く

        StageClear stageClear;

        bool IsRunning;      //  Followコルーチンが走っているかどうか

        public FollowOrbital(OrbitalContext orbitalCtx)    //  PlanetCommandProviderから呼ばれる
        {
            _context = orbitalCtx;
            stageClear = orbitalCtx.StageClear;
        }

        public override void Execute()    //  軌道追従開始
        {
            if (IsRunning) { return; }
            orbitalFollowLoop = ServiceLocator.Resolve<ICoroutineRunnerFacade>().StartCoroutine(OrbitalFollowLoop());
            //orbitalFollowLoop = _CoroutineRunner.StartCoroutine(OrbitalFollowLoop());    //  マウス追従ループ開始
        }

        IEnumerator OrbitalFollowLoop()    //  マウス位置に応じて円軌道を追従させるコルーチンループ
        {
            LogUtility.Log(LogPrefix.orbitalFollower, "天体円軌道追従開始", LogLevel.Debug);
            while (true && !stageClear.IsClearing)
            {
                yield return MoveAlongCircleByAngle(
                    _context.planetTransform,
                    _context.playerTransform,
                    _context.orbitalStatusBlock.OrbitRadius,
                    _context.orbitalStatusBlock.Duration
                    );
            }
        }

        IEnumerator MoveAlongCircleByAngle(Transform obj, Transform centerTF, float radius, float duration)    //  天体移動
        {
            Plane plane = new Plane(Vector3.forward, centerTF.position);    //  Ray検知用Plane

            float elapsed = 0f;    //  経過時間
            float startAngle = Mathf.Atan2(obj.position.y - centerTF.position.y, obj.position.x - centerTF.position.x) * Mathf.Rad2Deg;    //  開始アングル

            if (obj != null && centerTF != null)
            {
                while (elapsed < duration && !stageClear.IsClearing)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Debug.Log(centerTF);
                    if (plane.Raycast(ray, out float distance))
                    {
                        Vector3 mousePos = ray.GetPoint(distance);
                        float endAngle = Mathf.Atan2(mousePos.y - centerTF.position.y, mousePos.x - centerTF.position.x) * Mathf.Rad2Deg;
                        if (endAngle < 0) endAngle += 360f;

                        float t = elapsed / duration;
                        float angle = Mathf.LerpAngle(startAngle, endAngle, t) * Mathf.Deg2Rad;

                        obj.position = centerTF.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;

                        if (elapsed + Time.deltaTime <= duration)
                            startAngle = endAngle;
                    }

                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

        public void Stop()    //  マウス追従停止
        {
            if (orbitalFollowLoop != null)
            {
                LogUtility.Log(LogPrefix.orbitalFollower, "天体円軌道追従開始", LogLevel.Debug);
                ServiceLocator.Resolve<ICoroutineRunnerFacade>().StartCoroutine(OrbitalFollowLoop());
                orbitalFollowLoop = null;
            }
            IsRunning = false;
        }
    }
}