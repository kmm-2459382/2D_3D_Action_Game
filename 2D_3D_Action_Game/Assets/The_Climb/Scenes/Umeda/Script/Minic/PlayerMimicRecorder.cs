using System.Collections.Generic;
using UnityEngine;

public class PlayerMimicRecorder : MonoBehaviour
{
    public static PlayerMimicRecorder Instance { get; private set; }

    [Header("記録設定")]
    public int maxHistoryFrames = 600;
    public float recordThreshold = 0.0001f;

    private readonly List<Vector3> positionHistory = new List<Vector3>();
    private readonly List<Quaternion> rotationHistory = new List<Quaternion>();
    private readonly List<AnimatorStateInfo> animHistory = new List<AnimatorStateInfo>();

    private Animator playerAnimator;
    private Transform modelTransform; // 実際の見た目オブジェクト
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private void Awake()
    {
        Instance = this;
        playerAnimator = GetComponentInChildren<Animator>();

        if (playerAnimator != null)
            modelTransform = playerAnimator.transform; // 見た目モデルを記録対象に
        else
            modelTransform = transform; // フォールバック

        lastPosition = transform.position;
        lastRotation = modelTransform.rotation;
    }

    private void FixedUpdate()
    {
        Vector3 forwardDir = transform.forward;
        Quaternion flatRot = Quaternion.LookRotation(new Vector3(forwardDir.x, 0, forwardDir.z));

        positionHistory.Add(transform.position);
        rotationHistory.Add(flatRot); // ← 向いている実方向を記録
        animHistory.Add(playerAnimator.GetCurrentAnimatorStateInfo(0));

        if (positionHistory.Count > maxHistoryFrames)
        {
            positionHistory.RemoveAt(0);
            rotationHistory.RemoveAt(0);
            animHistory.RemoveAt(0);
        }
    }

    public bool TryGetHistory(int frameDelay, out Vector3 pos, out Quaternion rot, out AnimatorStateInfo anim)
    {
        pos = Vector3.zero;
        rot = Quaternion.identity;
        anim = default;

        if (positionHistory.Count <= frameDelay)
            return false;

        int index = positionHistory.Count - frameDelay - 1;
        pos = positionHistory[index];
        rot = rotationHistory[index];
        anim = animHistory[index];
        return true;
    }

    public int HistoryCount => positionHistory.Count;
}
