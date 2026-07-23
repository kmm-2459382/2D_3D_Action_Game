#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

//  キッカー移動テスト
public class KickerMoveCommanderTest
{
    GameObject testKicker;    //  テストキッカーインスタンス
    KickerMoveCommander kickerMoveCommander;    //  キッカームーブコマンダーインスタンス    
    EnemyMover enemyMover;    //  エネミームーバーインスタンス

    [SetUp]
    public void Setup()
    {
        //  テスト用キッカー生成
        testKicker = new GameObject("testKicker");

        // コンポーネント追加
        testKicker.AddComponent<Rigidbody>();
        testKicker.AddComponent<BoxCollider>();
        testKicker.AddComponent<EnemyMover>();
        testKicker.AddComponent<CharacterGroundChecker>();
        testKicker.AddComponent<KickerMoveCommander>();
        testKicker.AddComponent<CharacterStateVisualizer>();

        // デフォルト状態に初期化
        kickerMoveCommander = testKicker.GetComponent<KickerMoveCommander>();
        enemyMover = testKicker.GetComponent<EnemyMover>();

        testKicker.AddComponent<LandGroundNotifier>();

        // ステータス差し替え
        var kickerStatus = ScriptableObject.CreateInstance<KickerStatus>();
        kickerStatus.StateStats.Add(new KickerStatus.StateStatPair
        {
            enemyMode = EnemyMode.NORMAL,
            Stats = new KickerStatBlock
            {
                MoveSpd = 10f,
                EdgeRayOffset = Vector3.right * 0.5f,
                JumpForce = 5f,
                JumpFrequency = 1f,
                BlowForceX = 100f,
                BlowForceY = 100f,
                GroundBlowMode = ForceMode.Acceleration,
                AirBlowMode = ForceMode.Impulse
            }
        });

        // Awake再実行用に呼び出し
        kickerMoveCommander.kickerStatus = kickerStatus;
        kickerMoveCommander.InitializeForTest(kickerStatus);
    }

    [Test]
    public void Move_RightDirection_MovesPositiveX()
    {
        // Arrange
        var beforePos = testKicker.transform.position;
        kickerMoveCommander.GetType().GetField("CurrentMoveDir", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(kickerMoveCommander, MoveDir.RIGHT);

        // Act
        kickerMoveCommander.Move();
        var afterPos = testKicker.transform.position;

        // Assert
        Assert.Greater(afterPos.x, beforePos.x);
    }

    [Test]
    public void FlipMoveDir_FromRight_ToLeft()
    {
        // Arrange
        kickerMoveCommander.GetType().GetField("CurrentMoveDir", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(kickerMoveCommander, MoveDir.RIGHT);

        // Act
        kickerMoveCommander.FlipMoveDir();
        var dir = (MoveDir)kickerMoveCommander.GetType().GetField("CurrentMoveDir", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(kickerMoveCommander);

        // Assert
        Assert.AreEqual(MoveDir.LEFT, dir);
    }
}
#endif
//  以下コード保存所  //
//kickerMoveCommander.GetType().GetField("kickerStatus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(kickerMoveCommander, kickerStatus);
//kickerMoveCommander.GetType().GetMethod("InitializeForTest", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(kickerMoveCommander, new object[] { kickerStatus });
