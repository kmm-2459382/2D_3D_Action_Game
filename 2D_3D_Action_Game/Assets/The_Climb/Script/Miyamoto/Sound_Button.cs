using UnityEngine;

public class Sound_Button : MonoBehaviour
{
    [SerializeField] private CustomButton button;
    [SerializeField] private GameObject UI;
    void Start()
    {
        button.onClickCallBack = () =>
       {
           UI.SetActive(true);
       };
    }

}

