using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsButton : MonoBehaviour
{
    GameManagement gameManagement;
    void Start()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        var instructionsButton = GetComponent<Button>();
        instructionsButton.onClick.AddListener(gameManagement.LoadInstructionScreen);
        instructionsButton.onClick.AddListener(delegate ()
        {
            gameManagement.GetComponent<AudioSource>().Play();
        });
    }

}
