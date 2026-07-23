using UnityEngine;

public class CoinGameManager : MonoBehaviour
{
    public static CoinGameManager Instance;

    [Header("必要コイン数")]
    public int targetCoinCount = 5;

    [Header("ゴールオブジェクト")]
    public GameObject goalObject;  // 最初は非表示にしておく

    private int currentCoinCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddCoin()
    {
        currentCoinCount++;
        Debug.Log("コイン取得！ 現在: " + currentCoinCount);

        if (currentCoinCount >= targetCoinCount)
        {
            GoalAppear();
        }
    }

    void GoalAppear()
    {
        Debug.Log("必要数に達したのでゴール出現！");
        goalObject.SetActive(true);   // ゴールを表示
    }
}
