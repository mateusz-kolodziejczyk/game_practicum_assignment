using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteToggle : MonoBehaviour
{
    GameManagement gameManagement;
    void Start()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        var muteToggle = GetComponent<Toggle>();
        muteToggle.onValueChanged.AddListener(gameManagement.Mute);
    }
}
