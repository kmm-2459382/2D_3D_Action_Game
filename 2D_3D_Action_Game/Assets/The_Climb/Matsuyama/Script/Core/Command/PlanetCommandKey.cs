namespace TheClimb.Astral
{
    //  コマンドKeyに使うデータの構造体
    public struct PlanetCommandKey
    {
        // 事前定義タグ
        public static readonly PlanetCommandKey RotationPlanet = new ("RitationPlanet");    //  天体を回転させる関数Key

        public string Value { get; }

        private PlanetCommandKey(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        // 等価比較（==, !=）できるように
        public override bool Equals(object obj)
        {
            return obj is PlanetCommandKey other && Value == other.Value;
        }

        //  ハッシュコードゲット
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(PlanetCommandKey a, PlanetCommandKey b) => a.Equals(b);
        public static bool operator !=(PlanetCommandKey a, PlanetCommandKey b) => !a.Equals(b);

        // 動的に作成も可能
        public static PlanetCommandKey Custom(string value) => new(value);
    }
}