using System;
using System.Collections.Generic;

namespace TheClimb.Item
{
    public static class ImpactableRegistry    //  衝撃球影響対象レジストリ
    {
        private static readonly List<IImpactable> _impactables = new();    //  登録用リスト

        public static void Register(IImpactable impactable)    //  登録
        {
            if (!_impactables.Contains(impactable))
                _impactables.Add(impactable);
        }

        public static void Unregister(IImpactable impactable)    //  登録解除
        {
            if (_impactables.Contains(impactable))
                _impactables.Remove(impactable);
        }

        public static IImpactable GetPlayer(Func<IImpactable, bool> predicate = null)    //  プレイヤー取得
        {
            if (predicate != null)
            {
                foreach (var i in _impactables)
                    if (predicate(i)) return i;
                return null;
            }
            return _impactables.Count > 0 ? _impactables[0] : null;
        }

        public static IReadOnlyList<IImpactable> GetAll() => _impactables.AsReadOnly();    //  衝撃球影響対象全取得
    }
}