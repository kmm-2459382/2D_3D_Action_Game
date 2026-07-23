using UnityEngine;
using UnityEngine.Playables;

public class KidnapBuddy : MonoBehaviour
{
    [HideInInspector] public StalkerHandController handController;
    private LightDarkWorld lightDark;

    private void Start()
    {
        handController = transform.parent.gameObject.GetComponent<StalkerHandController>();
        lightDark = handController.lightDarkWorld;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!handController.buddyController.beingKidnapped && !handController.isKidnapping 
            && ((gameObject.layer == 18 && lightDark.brightnessState == LightDarkWorld.brightness.Light) 
            || (gameObject.layer == 19 && lightDark.brightnessState == LightDarkWorld.brightness.Dark) 
            || gameObject.layer == 29))
        {
            if (other.CompareTag("Buddy") && !handController.playerState.carryingBuddy)  //Buddyが孤立してる場合
            {
                handController.BuddyGet();
            }
            else if (other.CompareTag("Player") && handController.playerState.carryingBuddy)  //Buddyをおんぶしてる場合
            {
                PlayerMind playerMind = other.GetComponent<PlayerMind>();

                //敵とプレイヤーの位置でノックバックの方向を決める
                int dir = handController.mainStalker.transform.position.x - other.gameObject.transform.position.x <= 0 ? 1 : -1;
                handController.playerKnock.DoKnockBack(dir); //ノックバック
                handController.BuddyGet();
                playerMind.SanityDecreaseEvent(5);
            }
        }
    }
}
