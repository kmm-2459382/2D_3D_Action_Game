namespace TheClimb.Logging
{
    public readonly struct LogPrefix    //  ログプレフィクス定義クラス
    {
        // 事前定義タグ
        public static readonly LogPrefix screenFader = new("[ScreenFader]");
        public static readonly LogPrefix idleState = new("[IdleState]");
        public static readonly LogPrefix fadeImageSetter = new("[FadeImageSetter]");
        public static readonly LogPrefix fadeImageEditorWindow = new("[FadeImageEditorWindow]");
        public static readonly LogPrefix uiManager = new("[UIManager]");
        public static readonly LogPrefix uiFactory = new("[UIFacotry]");
        public static readonly LogPrefix titleButtonmanager = new("[TitleButtonManager]");
        public static readonly LogPrefix sceneUtility = new("[SceneUtility]");
        public static readonly LogPrefix toPlanetVectleCalculater = new("[ToPlanetVectleCalculater]");
        public static readonly LogPrefix orbitalFollower = new("[OrbitalFollower]");
        public static readonly LogPrefix countTillActivate = new("[CountTillActivate]");

        public string Value { get; }

        private LogPrefix(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        // 等価比較（==, !=）できるように
        public override bool Equals(object obj)
        {
            return obj is LogPrefix other && Value == other.Value;
        }

        //  ハッシュコードゲット
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(LogPrefix a, LogPrefix b) => a.Equals(b);
        public static bool operator !=(LogPrefix a, LogPrefix b) => !a.Equals(b);

        // 動的に作成も可能
        public static LogPrefix Custom(string value) => new(value);
    }
}
//namespace TheClimb.Logging
//{
//    public static class LogPrefixes
//    {
//        public static class Gameplay
//        {
//            public static readonly LogPrefix Explosion = new("[Gameplay.Explosion]");
//            public static readonly LogPrefix Player = new("[Gameplay.Player]");
//            public static readonly LogPrefix Orbital = new("[Gameplay.Orbital]");
//        }

//        public static class UI
//        {
//            public static readonly LogPrefix ScreenFader = new("[UI.ScreenFader]");
//            public static readonly LogPrefix Menu = new("[UI.Menu]");
//        }

//        public static class System
//        {
//            public static readonly LogPrefix Scene = new("[System.Scene]");
//            public static readonly LogPrefix Logging = new("[System.Logging]");
//        }
//    }
//}