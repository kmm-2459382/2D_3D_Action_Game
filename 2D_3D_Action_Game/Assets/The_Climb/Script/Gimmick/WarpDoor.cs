using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.BoolParameter;

public class WarpDoor : MonoBehaviour
{
    [SerializeField] private bool canGoBack = false;  //そのドアは引き返すことが可能か
    [Header("canGoBackが\ntrueであればgoToDoorを\nfalseならgoToWhereを指定しろ")]
    [SerializeField] private GameObject goToDoor;     //行先のドア
    [SerializeField] private GameObject goToWhere;    //行先の場所
    private bool nearPlayer = false;  //近くにプレイヤーがいるか判定
    private bool savePlayer = false;  //プレイヤーのオブジェクトを取得できているか判定
    private Transform player;         //プレイヤーのtransform
    private PlayerState state;
    private GameObject hukidashi;     //吹き出しオブジェクト
    private TextMeshPro hukidashiText;  //吹き出しテキスト
    private GameObject buddyMissing;  //Buddyがいないときの吹き出しオブジェクト
    private float displayTime = 2.0f;  //Buddyがいないときの吹き出し表示時間
    private bool buddyMissingActive = false;  //Buddyがいないときの吹き出しが表示中か判定
    [SerializeField] private bool buddyStage = false;  //ここはBuddyステージか？

    void Start()
    {
        if ((canGoBack && goToDoor == null) || (!canGoBack && goToWhere == null))
        {
            Debug.LogError(canGoBack ? "goToDoor" : "goToWhere" + "を指定してください。");
        }

        if (SceneManager.GetActiveScene().name == "Nakamura")
        {
            buddyStage = true;
        }

        buddyMissing = transform.Find("BuddyMissing").gameObject;
        buddyMissing.SetActive(false);
    }

    void Update()
    {
        //ドア使用
        if (nearPlayer && Input.GetKeyDown(KeyCode.W))
        {
            if ((buddyStage && state.carryingBuddy) || !buddyStage)
            {
                if (canGoBack)
                {
                    //ドアに移動
                    player.GetComponent<Rigidbody>().MovePosition(goToDoor.transform.position + new Vector3(0f, -1f, 0f));
                }
                else
                {
                    //指定した場所に移動
                    player.GetComponent<Rigidbody>().MovePosition(goToWhere.transform.position);
                }
            }
            else if (!buddyMissingActive)
            {
                //Buddyがいないときの吹き出し表示
                buddyMissing.SetActive(true);
                buddyMissingActive = true;
                Invoke("HideMissingBuddy", displayTime);
            }
        }
    }

    //吹き出し非表示
    private void HideMissingBuddy()
    {
        buddyMissing.SetActive(false);
        buddyMissingActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearPlayer = true;

            //プレイヤーオブジェクトの取得
            if (!savePlayer)
            {
                player = other.transform;
                state = player.GetComponent<PlayerState>();
                hukidashi = state.hukidashi;
                hukidashiText = state.hukidashiText;
                savePlayer = true;
            }

            //吹き出し表示
            hukidashi.SetActive(true);
            hukidashiText.text = "W";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearPlayer = false;
            //吹き出し非表示
            hukidashi.SetActive(false);
        }
    }
}
