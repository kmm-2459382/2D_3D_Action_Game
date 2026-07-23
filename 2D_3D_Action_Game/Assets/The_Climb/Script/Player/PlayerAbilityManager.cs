using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerAbilityManager : MonoBehaviour
{
    public GameObject umeAbi;
    public GameObject kitaAbi;
    public GameObject nakaAbi;
    public GameObject nishiAbi;
    public GameObject matsuAbi;
    public GameObject miyamotoyuoAbi;

    //[Header("デバッグ用")]
    //public bool AbilityOn;

    int NowAbilityNo = 0;

    // 現在のスキル番号を公開するプロパティ(SkillUIManagerから参照)
    public int CurrentSkillNumber => NowAbilityNo;

    void Awake()
    {
        GameObject[] abilityBools = {nakaAbi, matsuAbi, umeAbi, miyamotoyuoAbi, kitaAbi, nishiAbi};
        string[] abilityNames = { "Nakamura", "Matsuyama", "Umeda", "Yuoka", "Kitano", "Nisiyama", };

        //// デバッグ用で全アビリティオンオフ切り替えれるようにしてる
        //for (int i = 0; i < abilityBools.Length; i++)
        //{
        //    if (AbilityOn == true)
        //    {
        //        PlayerPrefs.SetInt($"{abilityNames[i]}", 1);
        //    }
        //    else
        //    {
        //        PlayerPrefs.SetInt($"{abilityNames[i]}", 0);
        //    }
        //}

        for (int i = 0; i < abilityBools.Length; i++)
        {
            if (PlayerPrefs.GetInt($"{abilityNames[i]}") == 0)
            {
                abilityBools[i].SetActive(false);
                Debug.Log("出すスキルを表示する");
                Debug.Log($"{abilityNames[i]} / active={abilityBools[i].activeSelf}");

            }else
            {
                abilityBools[i].SetActive(true);
                Debug.Log($"{abilityNames[i]} / active={abilityBools[i].activeSelf}");
            }
        }

        AbilityChange(0); // スタート時のアビリティを設定
    }

    void Update()
    {
        // アビリティ変更ボタンを押したときにアビリティの装備状況を変える
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            AbilityChange(0);
        }
    }

    // アビリティ変更の関数
    void AbilityChange(int count)
    {
        if (count != 3)
        { 
            switch (NowAbilityNo)
            {
                case 0: // 梅田君のアビリティ
                    NowAbilityNo++;
                    if (PlayerPrefs.GetInt("Umeda") == 1) //実行可能状態なら切り替える
                    {
                        PlayerPrefs.SetInt("UmedaAbi", 1);
                        PlayerPrefs.SetInt("KitanoAbi", 0);
                        PlayerPrefs.SetInt("NisiyamaAbi", 0);
                        Debug.Log("梅田君起動");
                    }
                    else
                    {
                        AbilityChange(count + 1); // 不可能状態なら次のアビリティへ移動
                    }
                    break;
                case 1: // 北野君のアビリティ
                    NowAbilityNo++;
                    if (PlayerPrefs.GetInt("Kitano") == 1)
                    {
                        PlayerPrefs.SetInt("UmedaAbi", 0);
                        PlayerPrefs.SetInt("KitanoAbi", 1);
                        PlayerPrefs.SetInt("NisiyamaAbi", 0);
                        Debug.Log("北野君起動");
                    }
                    else
                    {
                        AbilityChange(count + 1);
                    }
                    break;
                case 2: // 西山君のアビリティ
                    NowAbilityNo = 0;
                    if (PlayerPrefs.GetInt("Nisiyama") == 1)
                    {
                        PlayerPrefs.SetInt("UmedaAbi", 0);
                        PlayerPrefs.SetInt("KitanoAbi", 0);
                        PlayerPrefs.SetInt("NisiyamaAbi", 1);
                        Debug.Log("西山君起動");
                    }
                    else
                    {
                        AbilityChange(count + 1);
                    }
                    break;

            }
        }
        else
        {
            Debug.Log("発動できるスキルがありません");
            // 全てのアビリティをOFFにする
            PlayerPrefs.SetInt("UmedaAbi", 0);
            PlayerPrefs.SetInt("KitanoAbi", 0);
            PlayerPrefs.SetInt("NisiyamaAbi", 0);
        }
       
    }
}
