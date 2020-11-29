using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsReturnGameButton : MonoBehaviour
{
    GameManagement gameManagement;
    void Start()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        var button = GetComponent<Button>();
        button.onClick.AddListener(gameManagement.PauseGame);
    }
}
