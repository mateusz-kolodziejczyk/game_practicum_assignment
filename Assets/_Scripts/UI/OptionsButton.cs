using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsButton : MonoBehaviour
{
    GameObject optionsMenu;
    void Start()
    {
        var gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        optionsMenu = GameObject.FindWithTag("OptionsMenu");
        optionsMenu.SetActive(false);
        var button = GetComponent<Button>();
        button.onClick.AddListener(LoadOptionsMenu);
        button.onClick.AddListener(delegate ()
        {
            gameManagement.GetComponent<AudioSource>().Play();
        });

    }

    private void LoadOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

}
