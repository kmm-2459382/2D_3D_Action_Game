using System;
using UnityEngine;
using TMPro;
using TheClimb.Item;
using TheClimb.Astral;
using System.Collections;

namespace TheClimb.Core
{
    public class ObjectLabelController : MonoBehaviour    //  オブジェクトのラベルの制御をするクラス
    {
        ObjectLabelConfigBase _labelConfig;    //  ラベルのコンフィグ
        AttractableListenerBase itemContoroller;    //  アイテムコントローラー
        TextMeshPro label;    //  オブジェクトテキスト
        Transform _labelTargetTF;    //  衝撃球のトランスフォーム
        Transform _labelRootTF;    //  ラベルのRootオブジェクトのトランスフォーム
        Transform _cameraTF;    //  カメラのトランスフォーム
        RectTransform _rectTF;
        Vector3 _originLabelPos;
        Coroutine effectProcessRoutine = null;

        //  --  Unity LifeCycle

        void LateUpdate()
        {
            LookCamera();    //  カメラ方向を向かせる
            HomingObject();    //  テキストをオブジェクトに追従

            Action process = _labelConfig.ObjectKind switch    //  アイテム種類で分別する、簡易ステートパターン
            {
                ObjectKind.Time_Action => () => TimeItemUpdate(),
                _ => () => Debug.LogWarning($"ItemKind value is Unacceptable")
            };

            if(_labelConfig.EffectType != LabelEffectType.None && effectProcessRoutine == null)
            {
                //effectProcessRoutine = StartCoroutine(Shake());    //  🐻🐻🐻🐻🐻🐻🐻🐻
            }

            process();
            //effectProcess();
        }

        //  --  Public API

        public void Initialize(ObjectLabelConfigBase config, ObjectLabelContext _context)    //  表示するためのトランスフォームと、SOの参照をもらってる
        {
            _labelConfig = config;

            _cameraTF = _context.MainCameraTF;
            _labelTargetTF = _context.ImpactBallTF;

            _labelRootTF = this.transform;
            _labelRootTF.localPosition = _labelConfig.LabelOffset;

            itemContoroller = _context.ItemController;
            label = _context.ObjectLabel;
            _rectTF = label.rectTransform;
            _originLabelPos = _rectTF.anchoredPosition3D;
        }

        //  --  Private API

        void LookCamera()    //  ラベルをカメラに向かせる
        {
            Vector3 dir = _labelRootTF.position - _cameraTF.position;


            if (dir.sqrMagnitude < 0.0001f)
            { return; }

            if (_labelTargetTF == null || _labelRootTF == null || _labelConfig == null)
            {
                _labelRootTF.position = _labelTargetTF.position + _labelConfig.LabelOffset;
                _labelRootTF.rotation = Quaternion.LookRotation(dir);
            }
        }

        void HomingObject()    //  テキストをオブジェクトに追従させる
        {
            _labelRootTF.position = _labelTargetTF.position;
        }

        //  固有処理関数(ラベル処理)

        void TimeItemUpdate()    //  時間制限系アイテムのUpdate
        {
            ApplyFuseTime();
        }
        
        void ApplyFuseTime()    //  爆発までの時間をテキストに反映する
        {
            float remainFuseTime = itemContoroller.RemainCount;
            int display = Mathf.CeilToInt(remainFuseTime);
            label.text = display.ToString();    //  0じゃなくて1の表示の時に爆発させるために繰り上げ
            if (itemContoroller.RemainCount <= 0)
            {
                label.enabled = false;
            }
        }

        //  固有処理関数(エフェクト)

        //IEnumerator Shake()    //  テキスト震え
        //{
        //    try
        //    {
        //        Debug.Log("悪党が奏でるこの歌");

        //        float CountProgress = itemContoroller.ConfigFuseTime / itemContoroller.RemainCount;
        //        float time = 0f;

        //        while (_labelConfig.ShakeFrequency > time)
        //        {
        //            time += Time.deltaTime;
        //            yield return null;
        //        }
        //        Vector2 p = UnityEngine.Random.insideUnitCircle * _labelConfig.ShakeAmplitude;    //  🐻🐻🐻🐻🐻🐻🐻🐻

        //        _rectTF.anchoredPosition3D = _originLabelPos + new Vector3(p.x, p.y, 0);
        //    }
        //    finally
        //    {
        //        OnShakeFinished();    //  状態変数管理
        //    }
        //}
        //void OnShakeFinished()    //  Shakekコルーチン終了時の変数管理
        //{
        //    effectProcessRoutine = null;
        //}
    }
}