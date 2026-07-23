using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class KeyBindUmeda : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpAction;

    [Header("移動キー")]
    public KeyCode playerLMove = KeyCode.A;
    public KeyCode playerRMove = KeyCode.D;
    public KeyCode moveFront = KeyCode.W;   // 前進
    public KeyCode moveBack = KeyCode.S;    // 後退

    [Header("アクションキー")]
    public KeyCode playerJump = KeyCode.Space;
    public KeyCode timeSwitch = KeyCode.Q;

    public void Save()
    {
        PlayerPrefs.SetString("moveLeft", playerLMove.ToString());
        PlayerPrefs.SetString("moveRight", playerRMove.ToString());
        PlayerPrefs.SetString("moveFront", moveFront.ToString());
        PlayerPrefs.SetString("moveBack", moveBack.ToString());
        PlayerPrefs.SetString("playerJump", playerJump.ToString());
        PlayerPrefs.SetInt("timetravel", (int)timeSwitch);
    }

    public void Load()
    {
        playerLMove = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveLeft", playerLMove.ToString()));
        playerRMove = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveRight", playerRMove.ToString()));
        moveFront = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveFront", moveFront.ToString()));
        moveBack = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveBack", moveBack.ToString()));
        playerJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("playerJump", playerJump.ToString()));
        timeSwitch = (KeyCode)PlayerPrefs.GetInt("timetravel", (int)timeSwitch);
    }
}
