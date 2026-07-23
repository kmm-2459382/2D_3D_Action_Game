using UnityEngine;

namespace TheClimb.Astral
{
    public class OrbitalContext : PlanetCommandBaseCtx    //  orbitalのcommandClassに渡すためのctx
    {

        //  --  publicAPI

        public Transform playerTransform;    //  orbitalの中心位置用
        //public StageClear StageClear { get; }    //  orbitalの中心位置用
        public StageClear StageClear;    //  orbitalの中心位置用

        public OrbitalContext(Transform planetTF, OrbitalStatusBlock status, Transform playerTF, StageClear stageClear)
            : base (planetTF, status)    //  コンテキスト
        {
            playerTransform = playerTF;
            StageClear = stageClear;
        }
    }
}