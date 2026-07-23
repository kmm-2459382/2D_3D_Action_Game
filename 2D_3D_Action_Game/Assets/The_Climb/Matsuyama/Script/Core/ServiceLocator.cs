using System;
using System.Collections.Generic;

namespace TheClimb.Core
{
    public static class ServiceLocator    //  InterfaceやBaseClassをKey、インスタンスをValueのStatic辞書を持って、APIFacadeになる
    {
        static readonly Dictionary<Type, object> services = new();    //  サービス辞書

        //  --  Public API

        public static void Register<T>(T service)    //  APIを登録
        {
            if (services.ContainsKey(typeof(T)))
            {
                return;
            }

            services[typeof(T)] = service;
        }

        public static T Resolve<T>()    //  API取り出し(Interfaceにキャスト)
        {
            return (T)services[typeof(T)];
        }

        public static void Unregister<T>(T service)    //  登録解除
        {
            services.Remove(typeof(T));
        }

        public static void TrimServiceDict()    //  UnRegisterを呼び終わった後にメモリを解放するために辞書をTrim
        {
            services.TrimExcess();
        }
    }
}