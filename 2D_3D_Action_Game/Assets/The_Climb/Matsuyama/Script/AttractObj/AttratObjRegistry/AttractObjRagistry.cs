using System.Collections.Generic;
using UnityEngine;

namespace TheClimb.Astral
{
    public class AttractObjResistry    //  万有引力の影響を受けるオブジェクトのレジストリー
    {
        private static readonly List<AttractTargetEntry> _entries = new();    //  レジストリ
        
        //  --  Public API

        public static IReadOnlyList<AttractTargetEntry> Entries => _entries;

        public static void RegistTarget(AttractTargetMarker target, Rigidbody targetRB)    //  引き寄せ対象登録
        {
            Debug.Log("登録");
            if (_entries.Exists(e => e.target == target))
            {
                return;
            }
            _entries.Add(new AttractTargetEntry(target, targetRB));
        }

        public static void UnregisterTarget(AttractTargetMarker target, Rigidbody rigidbody)    //  引き寄せ対象登録解除
        {
            _entries.RemoveAll(e => e.target == target && e.rigidbody == rigidbody);
        }
    }
}