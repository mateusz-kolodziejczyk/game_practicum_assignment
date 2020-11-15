using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnToMenuButton : MonoBehaviour
{
    private GameManagement gameManagement;
    // Start is called before the first frame update
    void Awake()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        var returnToMenuButton = GetComponent<Button>();
        returnToMenuButton.onClick.AddListener(gameManagement.LoadMainMenu);
    }

}
