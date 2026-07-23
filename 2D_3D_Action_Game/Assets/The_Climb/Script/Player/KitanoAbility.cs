using UnityEngine;

public class KitanoAbility : MonoBehaviour
{
    public SkilTransparent skilTransparent;
    public TempDisableColliders tempDisableColliders;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(PlayerPrefs.GetInt("Kitano") == 1)
        {
            skilTransparent.enabled = true;
            tempDisableColliders.enabled = true;
        }
        else
        {
            skilTransparent.enabled = false;
            tempDisableColliders.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
