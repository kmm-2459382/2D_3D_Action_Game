using UnityEngine;
using System.Collections;

// ステージの進行状況(X座標)に応じてBGMのクリップを直接切り替えするスクリプト
public class BuddyStageBGM : MonoBehaviour
{
    [SerializeField] Transform target; // 追跡対象のトランスフォーム
    [SerializeField] float thresholdX = 620f; // BGMを切り替えるX座標のしきい値
    [SerializeField] AudioClip StageClip; // ステージ前半用のBGMクリップ
    [SerializeField] AudioClip BossClip; // ボス（ステージ後半）用のBGMクリップ

    [SerializeField] AudioSource audioSource; // BGM再生用の単一オーディオソース
    bool isHigh = false; // しきい値を超えているかどうかのフラグ

    // コンポーネント起動時に呼び出される初期化処理
    void Awake()
    {
        // オーディオソースの初期設定を行うために呼び出す
        InitializeAudioSource();
    }

    // オーディオソースの初期設定を行う関数
    void InitializeAudioSource()
    {
        // インスペクターで設定されていない場合は動的に追加するために呼び出す
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.loop = true; // ループ再生を有効化
        audioSource.playOnAwake = false; // 自動再生を無効化
    }

    // 更新の最初に呼び出される初期設定処理
    void Start()
    {
        // ターゲットが設定されていない場合の警告を表示するために呼び出す
        if (target == null) Debug.LogWarning("BuddyStageBGM: target が設定されていません。");

        // 初期状態の判定を行うために呼び出す
        isHigh = (target != null && target.position.x >= thresholdX);
        
        // クリップを設定して再生を開始するために呼び出す
        audioSource.clip = isHigh ? BossClip : StageClip;
        if (audioSource.clip != null) audioSource.Play();
    }

    // 毎フレーム呼び出される更新処理
    void Update()
    {
        // ターゲットがいない場合は何もしないために呼び出す
        if (target == null) return;

        // 現在の座標がしきい値を超えているか判定するために呼び出す
        bool nowHigh = target.position.x >= thresholdX;
        
        // 状態が変化した場合にクリップを切り替えるために呼び出す
        if (nowHigh != isHigh)
        {
            isHigh = nowHigh; // 状態を更新
            SwitchClip(isHigh ? BossClip : StageClip); // クリップの切り替え処理を呼び出すために呼び出す
        }
    }

    // クリップを切り替えて再生し直す関数
    void SwitchClip(AudioClip nextClip)
    {
        // 現在再生中のクリップと異なる場合のみ処理を行うために呼び出す
        if (audioSource.clip == nextClip) return;

        audioSource.Stop(); // 一旦停止するために呼び出す
        audioSource.clip = nextClip; // 新しいクリップをセット
        
        // クリップが設定されているなら再生するために呼び出す
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
