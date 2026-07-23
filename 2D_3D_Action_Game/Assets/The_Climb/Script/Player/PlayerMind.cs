using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerMind : MonoBehaviour
{
    private PlayerState state;
    private LightDarkWorld lightDarkWorld;

    private int sanityMax = 100;             //正気度の最大値
    private bool inFog = false;              //霧の中
    private float erosionIncreaseCounter = 0f; //侵蝕増加分の追跡
    private bool buddyStage = false;         //相棒ステージか判定

    void Start()
    {
        state = GetComponent<PlayerState>();
        if (GameObject.Find("LightDarkWorld") != null)
        {
            lightDarkWorld = GameObject.Find("LightDarkWorld").GetComponent<LightDarkWorld>();
        }

        if (SceneManager.GetActiveScene().name == "Nakamura")
        {
            buddyStage = true;
        }
    }

    void Update()
    {
        if (buddyStage)
        {
            //正気度１００超えたら１００にする
            if (state.sanityLevel > sanityMax)
            {
                state.sanityLevel = sanityMax;
            }
            //侵蝕度０下回ったら０にする
            if (state.erosionLevel < 0)
            {
                state.erosionLevel = 0;
            }

            //暗い闇の中で侵蝕度増加
            if (lightDarkWorld != null)
            {
                if (lightDarkWorld.brightnessState == LightDarkWorld.brightness.Dark)
                {
                    ErosionIncrease();
                }
                else if (!inFog)  //光かつNot霧の中で侵蝕度リセット
                {
                    ErosionReset();
                }
            }
        }
    }

    //侵蝕度増加
    private void ErosionIncrease()
    {
        float before = state.erosionLevel;
        state.erosionLevel += Time.deltaTime;

        float delta = state.erosionLevel - before;
        erosionIncreaseCounter += delta;

        //侵蝕度増加分が 5 を超えたら正気度 -1
        while (erosionIncreaseCounter >= 5f)
        {
            state.sanityLevel--;
            erosionIncreaseCounter -= 5f;
        }
    }

    //侵蝕度リセット
    private void ErosionReset()
    {
        state.erosionLevel = 0;
        erosionIncreaseCounter = 0f;
    }

    //エリア切り替え時の正気度最大値減少
    public void SanityMaxDecrease()
    {
        sanityMax -= 5;
    }

    public void SanityDecreaseEvent(int n)
    {
        state.sanityLevel -= n;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SanityHealItem"))
        {
            state.sanityLevel += 5;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Fog"))  //霧の中
        {
            inFog = true;
            ErosionIncrease();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Fog"))  //霧から出た
        {
            inFog = false;
        }
    }
}
