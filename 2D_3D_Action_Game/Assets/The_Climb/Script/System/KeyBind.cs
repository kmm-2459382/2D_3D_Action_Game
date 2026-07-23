using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class KeyBind : MonoBehaviour
{
    //ーーーーーーーー仮途中ーーーーーーーーーー
    [SerializeField] private InputActionReference jumpAction;

    public KeyCode playerLMove = KeyCode.A;
    public KeyCode playerRMove = KeyCode.D;
    public KeyCode playerJump = KeyCode.Space;
    public KeyCode highJump = KeyCode.W;
    public KeyCode meteorDrop = KeyCode.S;
    public KeyCode timeSwitch = KeyCode.Q;

    public void Save()
    {
        PlayerPrefs.SetString("moveLeft", playerLMove.ToString());
        PlayerPrefs.SetString("moveRight", playerRMove.ToString());
        PlayerPrefs.SetString("playerJump", playerJump.ToString());
        PlayerPrefs.SetString("highJump", highJump.ToString());
        PlayerPrefs.SetString("meteorDrop", meteorDrop.ToString());
        PlayerPrefs.SetInt("timetravel", (int)timeSwitch);
    }

    public void Load()
    {
        playerLMove = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveLeft", playerLMove.ToString()));
        playerRMove = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveRight", playerRMove.ToString()));
        playerJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("playerJump", playerJump.ToString()));
        highJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("highJump", highJump.ToString()));
        meteorDrop = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("meteorDrop", meteorDrop.ToString()));
        timeSwitch = (KeyCode)PlayerPrefs.GetInt("timetravel", (int)timeSwitch);
    }

    //public KeyCode playerLMove = KeyCode.A;
    //public KeyCode playerRMove = KeyCode.D;
    //public KeyCode playerJump = KeyCode.Space;
    //public KeyCode highJump = KeyCode.W;
    //public KeyCode meteorDrop = KeyCode.S;

    //public void Save()
    //{
    //    PlayerPrefs.SetString("moveLeft", playerLMove.ToString());
    //    PlayerPrefs.SetString("moveRight", playerRMove.ToString());
    //    PlayerPrefs.SetString("playerJump", playerJump.ToString());
    //    PlayerPrefs.SetString("highJump", highJump.ToString());
    //    PlayerPrefs.SetString("meteorDrop", meteorDrop.ToString());
    //}

    //public void Load()
    //{
    //    playerLMove = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveLeft", playerLMove.ToString()));
    //    playerRMove = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveRight", playerRMove.ToString()));
    //    playerJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("playerJump", playerJump.ToString()));
    //    highJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("highJump", highJump.ToString()));
    //    meteorDrop = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("meteorDrop", meteorDrop.ToString()));
    //}
}
