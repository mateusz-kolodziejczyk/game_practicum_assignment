using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    GameManagement gameManagement;
    private Button startButton;
    void Start()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        startButton = GetComponent<Button>();
        startButton.onClick.AddListener(gameManagement.StartGame);
    }

}
