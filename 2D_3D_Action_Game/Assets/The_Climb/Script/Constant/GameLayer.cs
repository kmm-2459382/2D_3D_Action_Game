using System.Collections.Generic;
using UnityEngine;

public enum GameLayers    //  ゲームのレイヤー一覧
{
    GROUND,
}

public static class LayerNames    //  レイヤー名一覧
{
    public const string Ground = "Ground";
}
public static class GameLayer    //  レイヤーデータ
{
    public static readonly Dictionary<GameLayers, string> LayerEnumToName = new()    //  レイヤーenumとstringの辞書型
    {
        { GameLayers.GROUND, LayerNames.Ground },
    };
    public static readonly Dictionary<GameLayers, int> LayerEnumToIndex = new();    //  レイヤーenumとレイヤーインデックスの辞書型
    public static readonly Dictionary<GameLayers, LayerMask> LayerEnumToMask = new();    //  レイヤーenumとレイヤーマスクの辞書型

    //  辞書設定
    static GameLayer()
    {
        foreach(KeyValuePair<GameLayers, string> kvp in LayerEnumToName)
        {
            int index = LayerMask.NameToLayer(kvp.Value);
            if(index == -1)
            {
#if UNITY_EDITOR
                Debug.LogError($"[GameLayer] Laeyr ' {kvp.Value}' does not exist in project settings.");
#endif
            }
            LayerEnumToIndex[kvp.Key] = index;
            LayerEnumToMask[kvp.Key] = LayerMask.GetMask(kvp.Value);
        }
    }

    public static LayerMask ToMask(GameLayers layer) => LayerEnumToMask[layer];
    public static int ToIndex(GameLayers layer) => LayerEnumToIndex[layer];
    public static string ToName(GameLayers layer) => LayerEnumToName[layer];
    //  レイヤーインデックスからレイヤー取得
    public static GameLayers? FromIndex(int index)
    {
        foreach (KeyValuePair<GameLayers, int> kvp in LayerEnumToIndex)
        {
            if (kvp.Value == index)
            {
                return kvp.Key;
            }
        }
        return null;
    }
    //  レイヤー名からレイヤー取得
    public static GameLayers? FromName(string layerName)
    {
        foreach(KeyValuePair<GameLayers, string> kvp in LayerEnumToName)
        {
            if(kvp.Value == layerName)
            {
                return kvp.Key;
            }
        }
        return null;
    }

}
