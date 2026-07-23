namespace TheClimb.Logging
{
    public readonly struct LogLevel
    {
        // 事前定義タグ
        public static readonly LogLevel VerBose = new(0, "[Verbose]");
        public static readonly LogLevel Debug = new(1, "[Debug]");
        public static readonly LogLevel Info = new(2, "[Info]");
        public static readonly LogLevel Warning = new(3, "[Warning]");
        public static readonly LogLevel Error = new(4, "Error");
        public static readonly LogLevel None = new(5, "None");

        public int Priority { get; }
        public string Name { get; }

        private LogLevel(int priority, string name)
        {
            Priority = priority;
            Name = name;
        }

        public override string ToString() => Name;    //  ToStringオーバーライド

        public int CompareTo(LogLevel other) => Priority.CompareTo(other.Priority);    //  比較関数

        public bool Equals(LogLevel other) => Priority == other.Priority;

        public override bool Equals(object obj) => obj is LogLevel other && Equals(other);

        //  ハッシュコードゲット
        public override int GetHashCode() => Priority.GetHashCode();    //  ハッシュコード取得
        public static bool operator <(LogLevel a, LogLevel b) => a.Priority < b.Priority;
        public static bool operator >(LogLevel a, LogLevel b) => a.Priority > b.Priority;
        public static bool operator <=(LogLevel a, LogLevel b) => a.Priority <= b.Priority;
        public static bool operator >=(LogLevel a, LogLevel b) => a.Priority >= b.Priority;
        public static bool operator ==(LogLevel a, LogLevel b) => a.Equals(b);
        public static bool operator !=(LogLevel a, LogLevel b) => !a.Equals(b);

        // 動的作成
        public static LogLevel Custom(int priority, string name) => new(priority, name);
    }
}