//using System.Collections.Generic;
//using UnityEngine;

//namespace TheClimb.Astral
//{
//    public class PlanetCommandProvider
//    {
//        public Dictionary<PlanetCommandKey, ICommandBase> PlanetCommandMap { get; }

//        public PlanetCommandProvider(PlanetMover Mover, Transform Planet, GravitationStatusBlock GravitationStatusBlock)    //  コンストラクタ
//        {
//            PlanetCommandMap = new Dictionary<PlanetCommandKey, ICommandBase>
//            {
//                {PlanetCommandKey.RotationPlanet, new RotationPlanet(() => Mover.RotationPlanet(Planet, GravitationStatusBlock.RotationSpeed)) }
//            };
//        }

//        public void Register(PlanetCommandKey commnadKey, ICommandBase Command)    //  コマンド代入
//        {
//            PlanetCommandMap[commnadKey] = Command;
//        }

//        public T Get<T>(PlanetCommandKey commandKey) where T : class, ICommandBase    //  コマンド取得
//        {
//            return PlanetCommandMap.TryGetValue(commandKey, out var cmd) ? cmd as T : null;
//        }
//    }
//}