using System.Collections.Generic;
using UnityEngine;

public class PlayerMimicRecorder : MonoBehaviour
{
    public static PlayerMimicRecorder Instance { get; private set; }

    [Header("記録設定")]
    public int maxHistoryFrames = 600;

    private readonly List<Vector3> positionHistory = new List<Vector3>();
    private readonly List<Quaternion> rotationHistory = new List<Quaternion>();

    // プレイヤーが使用する全アニメーションパラメータの履歴
    private readonly List<float> speedHistory = new List<float>();
    private readonly List<float> motionSpeedHistory = new List<float>();
    private readonly List<bool> groundedHistory = new List<bool>();
    private readonly List<bool> jumpHistory = new List<bool>();
    private readonly List<bool> freeFallHistory = new List<bool>();
    private readonly List<bool> inWaterHistory = new List<bool>();

    private Animator playerAnimator;
    private Transform modelTransform;

    private void Awake()
    {
        Instance = this;
        playerAnimator = GetComponentInChildren<Animator>();
        if (playerAnimator != null)
            modelTransform = playerAnimator.transform;
        else
            modelTransform = transform;
    }

    private void FixedUpdate()
    {
        Vector3 forwardDir = transform.forward;
        Quaternion flatRot = Quaternion.LookRotation(new Vector3(forwardDir.x, 0, forwardDir.z));

        positionHistory.Add(transform.position);
        rotationHistory.Add(flatRot);

        if (playerAnimator != null)
        {
            speedHistory.Add(playerAnimator.GetFloat("Speed"));
            motionSpeedHistory.Add(playerAnimator.GetFloat("MotionSpeed"));
            groundedHistory.Add(playerAnimator.GetBool("Grounded"));
            jumpHistory.Add(playerAnimator.GetBool("Jump"));
            freeFallHistory.Add(playerAnimator.GetBool("FreeFall"));
            inWaterHistory.Add(playerAnimator.GetBool("InWater"));
        }
        else
        {
            speedHistory.Add(0f);
            motionSpeedHistory.Add(1f);
            groundedHistory.Add(true);
            jumpHistory.Add(false);
            freeFallHistory.Add(false);
            inWaterHistory.Add(false);
        }

        if (positionHistory.Count > maxHistoryFrames)
        {
            positionHistory.RemoveAt(0);
            rotationHistory.RemoveAt(0);
            speedHistory.RemoveAt(0);
            motionSpeedHistory.RemoveAt(0);
            groundedHistory.RemoveAt(0);
            jumpHistory.RemoveAt(0);
            freeFallHistory.RemoveAt(0);
            inWaterHistory.RemoveAt(0);
        }
    }

    public bool TryGetHistory(int frameDelay, out Vector3 pos, out Quaternion rot, out float speed, out float motionSpeed, out bool grounded, out bool jump, out bool freeFall, out bool inWater)
    {
        pos = Vector3.zero;
        rot = Quaternion.identity;
        speed = 0f;
        motionSpeed = 1f;
        grounded = true;
        jump = false;
        freeFall = false;
        inWater = false;

        if (positionHistory.Count <= frameDelay)
            return false;

        int index = positionHistory.Count - frameDelay - 1;
        pos = positionHistory[index];
        rot = rotationHistory[index];
        speed = speedHistory[index];
        motionSpeed = motionSpeedHistory[index];
        grounded = groundedHistory[index];
        jump = jumpHistory[index];
        freeFall = freeFallHistory[index];
        inWater = inWaterHistory[index];
        return true;
    }

    public int HistoryCount => positionHistory.Count;
}