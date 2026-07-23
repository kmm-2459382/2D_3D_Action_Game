using UnityEngine;

public class Back_Button : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private CustomButton button;
    void Start()
    {
        button.onClickCallBack = () =>
        {
            UI.SetActive(false);
        };
    }

}
